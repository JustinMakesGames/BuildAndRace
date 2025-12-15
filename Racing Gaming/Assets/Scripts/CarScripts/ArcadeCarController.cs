using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Cinemachine;

[Serializable]
public struct BoostType
{
    public string boostTypeName;
    public float boostEndTime;
    public float boostTime;
    public GameObject boostParticles;
}

public enum PlayerState
{
    Player,
    CPU
}

public class ArcadeCarController : MonoBehaviour
{
    

    public PlayerState state;
    [Header("Reference Variables")]
    [SerializeField] private Rigidbody carRB; //Rigidbody of the car
    [SerializeField] private List<Transform> wheelRaycasts = new List<Transform>(); //Raycasts that are supposed to be the wheels
    [SerializeField] private LayerMask groundLayer; //Ground layer

    [Header("Suspension Variables")]
    [SerializeField] private float restLength; //The length of the suspension spring when it is resting
    [SerializeField] private float dampStiffness; //How much the spring gets damped
    [SerializeField] private float springStiffness;
    [SerializeField] private float springTravel; //How far the spring could travel
    [SerializeField] private float wheelRadius; //The radius of the wheel

    private bool[] _isWheelGrounded = new bool[8];
    [Header("Car Velocity Check")]
    [SerializeField] private Vector3 currentCarLocalVelocity;
    [SerializeField] private float velocityRatio;

    [Header("Acceleration Variables")]
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float backwardsMaxSpeed;
    [SerializeField] private Transform accelerationPosition;
    private float _accelerationInput;
    private bool _isGrounded;

    [Header("Steering Variables")]
    [SerializeField] private float steeringStrength;
    [SerializeField] private float dragCoefficient;
    private float _steeringInput;


    [Header("Drifting")]
    [SerializeField] private List<BoostType> boostTypes = new List<BoostType>();
    [SerializeField] private int currentBoostingIndex;
    [SerializeField] private float driftingTime;
    [SerializeField] private float mininumDriftSpeed;

    [SerializeField] private float minSteeringStrength;
    [SerializeField] private float maxSteeringStrength;
    [SerializeField] private float driftDragCoefficient;
    [SerializeField] private float driftDragCoefficientMultiplier;
    private int _direction = 1;
    private bool _isDrifting;

    [Header("Gravity")]
    [SerializeField] private float gravityForce;
    private Vector3 _gravityDirection;

    [Header("Boost")]
    [SerializeField] private float boostAccelerationSpeed;
    [SerializeField] private float boostMaxSpeed;
    [SerializeField] private float cameraBoostFOV;
    [SerializeField] private float cameraFOVSpeed;
    [SerializeField] private CinemachineCamera cam;
    private float _normalAccelerationSpeed;
    private float _normalMaxSpeed;
    private float _normalSteeringSpeed;
    private float _normalDragCoefficient;

    private BoostType _usedBoost;
    private bool _isBoosting;
    private float _boostTimer;

    [Header("Random Values")]
    [SerializeField] private float minMaxSpeed;
    [SerializeField] private float maxMaxSpeed;
    [SerializeField] private float minSteerSpeed;
    [SerializeField] private float maxSteerSpeed;
    [SerializeField] private bool shouldRandomizeValues;

    [Header("StartHandling")]
    [SerializeField] private bool canDrive;

    [Header("TeleporterHandling")]
    public CinemachineFollow cameraFollow;

    [Header("Handle Car Hit")]
    [SerializeField] private Transform motorModel;
    [SerializeField] private float hitTime;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float motorDampingValue;
    private bool _isHit;

    [Header("Upright Stabilization")]
    [SerializeField] private float uprightStrength = 20f;
    [SerializeField] private float uprightDamping = 5f;
    [SerializeField] private float maxUprightAngle = 60f;



    public void SetVerhicleOn()
    {
        
        canDrive = true;
    }

    public void SetVerhicleOff()
    {
        canDrive = false;
    }
    private void Start()
    {
        _gravityDirection = -transform.up;
        //SetPlayer(state);
        if (shouldRandomizeValues)
        {
            InitializeRandomVariables();
        }
        _normalAccelerationSpeed = accelerationSpeed;
        _normalMaxSpeed = maxSpeed;
        _normalSteeringSpeed = steeringStrength;
        _normalDragCoefficient = dragCoefficient;
    }

    //Sets player to Player or CPU
    public void SetPlayer(PlayerState state)
    {
        if (state == PlayerState.Player)
        {
            GetComponent<PlayerCarController>().enabled = true;
            GetComponent<AICarController>().enabled = false;
            
        }

        else
        {
            GetComponent<PlayerCarController>().enabled = false;
            GetComponent<AICarController>().enabled = true;
        }

        this.state = state;
    }
    private void Update()
    {
        CheckIfGrounded();
        CalculateCarVelocity();
        HandleDrifting();
        HandleGravity();
        BoostHandling();
        RotateMotorHit();
    }

    private void FixedUpdate()
    {
        CheckSuspension();
        MoveCheck();
        SteeringHandling();
        SideDrag();
        ChargeDriftingBoost();
        StabilizeUpright();
    }

    public void SetVariables(CarStats stats)
    {
        maxSpeed = stats.maxSpeed;
        accelerationSpeed = stats.accelerationSpeed;
        steeringStrength = stats.steeringSpeed;
    }

    //Make Random Variables
    private void InitializeRandomVariables()
    {
        maxSpeed = UnityEngine.Random.Range(minMaxSpeed, maxMaxSpeed);
        steeringStrength = UnityEngine.Random.Range(minSteerSpeed, maxSteerSpeed);
    }


    public void SetInput(float accelerationInput, float steeringInput)
    {
        if (!canDrive || _isHit) return;
        _accelerationInput = accelerationInput;
        _steeringInput = steeringInput;

    }

    public void StartDriftInput()
    {
        StartDrift();
    }

    public void CancelDriftInput()
    {
        CancelDrift();
    }

    //Calculates the direction of the drift
    private void StartDrift()
    {
        if (currentCarLocalVelocity.magnitude >= mininumDriftSpeed && _steeringInput != 0)
        {
            _isDrifting = true;
            _direction = (int)Mathf.Sign(_steeringInput);
            dragCoefficient = driftDragCoefficient;

        }
    }

    //Cancels the drift and gets the boost
    private void CancelDrift()
    {
        if (!_isDrifting) return;
        StartBoost(boostTypes[currentBoostingIndex]);
        _isDrifting = false;
        steeringStrength = _normalSteeringSpeed;
        driftingTime = 0;
        boostTypes[currentBoostingIndex].boostParticles.SetActive(false);
        currentBoostingIndex = -1;
        dragCoefficient = _normalDragCoefficient;
    }

    //Handles the gravity
    private void HandleGravity()
    {
        if (_isGrounded)
        {
            _gravityDirection = -transform.up;
        }

        carRB.AddForce(_gravityDirection * gravityForce, ForceMode.Acceleration);
    }

    //Calculates the charging of the driftingboost depending on how far the player is drifting to the direction they are going.
    private void ChargeDriftingBoost()
    {
        if (_isDrifting)
        {
            float timeMultiplier = 1.25f + 0.75f * (_steeringInput * _direction);

            driftingTime += timeMultiplier * Time.deltaTime;
            dragCoefficient += driftDragCoefficientMultiplier * Time.deltaTime;

            if (currentBoostingIndex + 1 < boostTypes.Count && driftingTime >= boostTypes[currentBoostingIndex + 1].boostEndTime)
            {
                HandleBoostChange();
            }
        }
    }

    //Handles the boost change.
    private void HandleBoostChange()
    {
        if (currentBoostingIndex != -1) boostTypes[currentBoostingIndex].boostParticles.SetActive(false);
        currentBoostingIndex++;
        boostTypes[currentBoostingIndex].boostParticles.SetActive(true);

    }

    //Calculates the drifting physics.
    private void HandleDrifting()
    {
        if (_isDrifting)
        {
            if (currentCarLocalVelocity.magnitude < mininumDriftSpeed)
            {
                CancelDrift();
                return;
            }

            float driftSteeringStrength = Mathf.Lerp(minSteeringStrength, maxSteeringStrength, (_steeringInput + 1f) / 2f);

            if (_direction < 0)
            {
                driftSteeringStrength = Mathf.Lerp(minSteeringStrength, maxSteeringStrength, 1f - ((_steeringInput + 1f) / 2f));
            }
            steeringStrength = driftSteeringStrength * _direction;
        }
    }
    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        velocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    private void CheckIfGrounded()
    {
        int groundedWheelsAmount = 0;
        for (int i = 0; i < _isWheelGrounded.Length; i++)
        {
            if (_isWheelGrounded[i])
            {
                groundedWheelsAmount++;
            }
        }

        if (groundedWheelsAmount > 3)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void MoveCheck()
    {
        if (_isGrounded)
        {
            CheckForDeceleration();
            AccelerationHandling();
            AccelerationBackwardsHandling();
        }
    }

    private void CheckForDeceleration()
    {
        if (_isHit) return;

        if (_accelerationInput == 0 || currentCarLocalVelocity.z > maxSpeed)
        {
            carRB.linearDamping = 1;
        }

        else
        {
            carRB.linearDamping = 0;
        }
    }

    private void AccelerationHandling()
    {
        if (currentCarLocalVelocity.z < maxSpeed && _accelerationInput > 0)
        {
            carRB.AddForceAtPosition(transform.forward * _accelerationInput * accelerationSpeed * Time.fixedDeltaTime, accelerationPosition.position, ForceMode.Acceleration);
        }

    }

    private void AccelerationBackwardsHandling()
    {

        if (currentCarLocalVelocity.z > backwardsMaxSpeed && _accelerationInput < 0)
        {
            carRB.AddForceAtPosition(transform.forward * _accelerationInput * decelerationSpeed * Time.fixedDeltaTime, accelerationPosition.position, ForceMode.Acceleration);
        }
    }

    private void SteeringHandling()
    {

        if (!_isDrifting)
        {
            carRB.AddRelativeTorque(steeringStrength * _steeringInput * Mathf.Sign(velocityRatio) * Vector3.up, ForceMode.Acceleration);

        }

        else
        {
            carRB.AddRelativeTorque(steeringStrength * Mathf.Sign(velocityRatio) * Vector3.up, ForceMode.Acceleration);
        }
    }

    private void SideDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);

    }

    //Calculates the suspension by using raycasts.
    private void CheckSuspension()
    {
        for (int i = 0; i < wheelRaycasts.Count; i++)
        {
            RaycastHit hit;
            
            float maxDistance = restLength + springTravel;

            if (Physics.Raycast(wheelRaycasts[i].position, -wheelRaycasts[i].up, out hit, maxDistance + wheelRadius, groundLayer))
            {
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(wheelRaycasts[i].position), wheelRaycasts[i].up);
                float dampForce = dampStiffness * springVelocity;
                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * hit.normal, wheelRaycasts[i].position);



                _isWheelGrounded[i] = true;
                Debug.DrawLine(wheelRaycasts[i].position, wheelRaycasts[i].position + -wheelRaycasts[i].up * (maxDistance + wheelRadius), Color.green);
            }

            else
            {
                _isWheelGrounded[i] = false;
                Debug.DrawLine(wheelRaycasts[i].position, wheelRaycasts[i].position + -wheelRaycasts[i].up * (maxDistance + wheelRadius), Color.red);
            }
        }
    }


    //Start by giving the player the boost speed stats.
    public void StartBoost(BoostType boostType)
    {
        _usedBoost = boostType;
        _boostTimer = 0;
        _isBoosting = true;

        accelerationSpeed = boostAccelerationSpeed;
        maxSpeed = boostMaxSpeed;

    }

    //Handle the feeling of the boost.
    private void BoostHandling()
    {
        if (_isBoosting)
        {
            _boostTimer += Time.deltaTime;

            if (_boostTimer >= _usedBoost.boostEndTime)
            {
                CancelBoost();
            }

            if (cam == null) return;
            if (cam.Lens.FieldOfView < cameraBoostFOV)
            {
                cam.Lens.FieldOfView += cameraFOVSpeed * Time.deltaTime;
            }
        }
    }

    private void CancelBoost()
    {
        _isBoosting = false;
        accelerationSpeed = _normalAccelerationSpeed;
        maxSpeed = _normalMaxSpeed;

        if (cam == null) return;
        StartCoroutine(ReturnCameraFOV());

    }

    private IEnumerator ReturnCameraFOV()
    {
        while (cam.Lens.FieldOfView >= 60 && !_isBoosting)
        {
            cam.Lens.FieldOfView -= cameraFOVSpeed * Time.deltaTime;
            yield return null;
        }

        if (_isBoosting) yield break;
        cam.Lens.FieldOfView = 60;
    }

    private void AlignToGround()
    {
        if (!_isGrounded) return;

        // Average ground normal from all grounded wheels
        Vector3 averageNormal = Vector3.zero;
        int groundedCount = 0;

        for (int i = 0; i < wheelRaycasts.Count; i++)
        {
            RaycastHit hit;
            float maxDistance = restLength + springTravel + wheelRadius;

            if (Physics.Raycast(wheelRaycasts[i].position, -wheelRaycasts[i].up, out hit, maxDistance, groundLayer))
            {
                averageNormal += hit.normal;
                groundedCount++;
            }
        }

        if (groundedCount == 0) return;
        averageNormal.Normalize();

        // Smoothly rotate the car to match the ground normal
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;

        // Prevent small tilting when roughly upright
        float angleFromUp = Vector3.Angle(Vector3.up, averageNormal);
        if (angleFromUp < 10f)
        {
            // Blend back toward upright orientation
            targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.1f);
        }

        // Smooth rotation for stability
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
    }

    public void HandleMotorHit()
    {
        if (_isHit) return;

        StartCoroutine(HandleHitTime());
    }

    private IEnumerator HandleHitTime()
    {
        _isHit = true;
        carRB.linearDamping = motorDampingValue;
        _steeringInput = 0;
        yield return new WaitForSeconds(hitTime);
        _isHit = false;
        motorModel.localEulerAngles = Vector3.zero;
    }

    private void RotateMotorHit()
    {
        if (_isHit)
        {
            motorModel.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    private void StabilizeUpright()
    {
        // Desired up direction (anti-gravity aware)
        Vector3 desiredUp = -_gravityDirection;

        // Current up
        Vector3 currentUp = transform.up;

        // Axis needed to rotate currentUp to desiredUp
        Vector3 correctionAxis = Vector3.Cross(currentUp, desiredUp);

        float angle = Vector3.Angle(currentUp, desiredUp);

        // Allow some tilt, but stop extreme flipping
        if (angle > maxUprightAngle)
        {
            // Remove yaw influence (VERY important)
            correctionAxis = Vector3.ProjectOnPlane(correctionAxis, transform.up);

            // Counteract angular velocity except yaw
            Vector3 angVel = carRB.angularVelocity;
            Vector3 angVelNoYaw = Vector3.ProjectOnPlane(angVel, transform.up);

            carRB.AddTorque(
                correctionAxis.normalized * uprightStrength * angle
                - angVelNoYaw * uprightDamping,
                ForceMode.Acceleration
            );
        }
    }


    public PlayerState ReturnPlayerState()
    {
        return state;
    }

}

