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

public class ArcadeCarController : MonoBehaviour
{

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

    private bool[] _isWheelGrounded = new bool[4];
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
    private int _direction = 1;
    private bool _isDrifting;

    [Header("Gravity")]
    [SerializeField] private float gravityForce;

    [Header("Boost")]
    [SerializeField] private float boostAccelerationSpeed;
    [SerializeField] private float boostMaxSpeed;
    [SerializeField] private float cameraBoostFOV;
    [SerializeField] private float cameraFOVSpeed;
    [SerializeField] private CinemachineCamera cam;
    private float _normalAccelerationSpeed;
    private float _normalMaxSpeed;
    private float _normalSteeringSpeed;

    private BoostType _usedBoost;
    private bool _isBoosting;
    private float _boostTimer;

    [Header("Random Values")]
    [SerializeField] private float minMaxSpeed;
    [SerializeField] private float maxMaxSpeed;
    [SerializeField] private float minSteerSpeed;
    [SerializeField] private float maxSteerSpeed;
    [SerializeField] private bool shouldRandomizeValues;

    private void Start()
    {
        if (shouldRandomizeValues)
        {
            InitializeRandomVariables();
        }
        _normalAccelerationSpeed = accelerationSpeed;
        _normalMaxSpeed = maxSpeed;
        _normalSteeringSpeed = steeringStrength;
    }

    private void Update()
    {
        CheckIfGrounded();
        CalculateCarVelocity();
        HandleDrifting();
        HandleGravity();
        BoostHandling();
    }

    private void FixedUpdate()
    {
        CheckSuspension();
        MoveCheck();
        SteeringHandling();
        SideDrag();
        ChargeDriftingBoost();
    }

    private void InitializeRandomVariables()
    {
        maxSpeed = UnityEngine.Random.Range(minMaxSpeed, maxMaxSpeed);
        steeringStrength = UnityEngine.Random.Range(minSteerSpeed, maxSteerSpeed);
    }


    public void SetInput(float accelerationInput, float steeringInput)
    {
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

    private void StartDrift()
    {
        if (currentCarLocalVelocity.z >= mininumDriftSpeed && _steeringInput != 0)
        {
            _isDrifting = true;
            _direction = (int)Mathf.Sign(_steeringInput);

        }
    }

    private void CancelDrift()
    {
        if (!_isDrifting) return;
        _usedBoost = boostTypes[currentBoostingIndex];
        _isDrifting = false;
        steeringStrength = _normalSteeringSpeed;
        driftingTime = 0;
        boostTypes[currentBoostingIndex].boostParticles.SetActive(false);
        currentBoostingIndex = -1;
        StartBoost();
    }

    private void HandleGravity()
    {
        Vector3 gravityDirection = -transform.up;

        carRB.AddForce(gravityDirection * gravityForce, ForceMode.Acceleration);
    }

    private void ChargeDriftingBoost()
    {
        if (_isDrifting)
        {
            float timeMultiplier = 1.25f + 0.75f * (_steeringInput * _direction);

            driftingTime += timeMultiplier * Time.deltaTime;

            if (currentBoostingIndex + 1 < boostTypes.Count && driftingTime >= boostTypes[currentBoostingIndex + 1].boostEndTime)
            {
                HandleBoostChange();
            }
        }
    }

    private void HandleBoostChange()
    {
        if (currentBoostingIndex != -1) boostTypes[currentBoostingIndex].boostParticles.SetActive(false);
        currentBoostingIndex++;
        boostTypes[currentBoostingIndex].boostParticles.SetActive(true);

    }


    private void HandleDrifting()
    {
        if (_isDrifting)
        {
            if (currentCarLocalVelocity.z < mininumDriftSpeed)
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

        if (groundedWheelsAmount > 1)
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



    private void StartBoost()
    {
        _boostTimer = 0;
        _isBoosting = true;

        accelerationSpeed = boostAccelerationSpeed;
        maxSpeed = boostMaxSpeed;

    }

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


}

