using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(ArcadeCarController))]
public class AICarController : MonoBehaviour
{
    [Header("WayPointHandling")]
    [SerializeField] private Transform wayPointFolder;
    [SerializeField] private List<Transform> wayPoints = new List<Transform>();
    [SerializeField] private float maxWayPointDistance;

    [SerializeField] private float miniWayPointDistance;
    private float _offset;
    private Vector3 _destination;
    private Transform _wayPoint;
    private float _wayPointDistance;
    private int _currentIndex;

    [Header("SteeringHandling")]
    [SerializeField] private float reverseDistance;
    
    private ArcadeCarController _carController;
    private float _accelerationInput;
    private float _steeringInput;
    private float _angleToDir;
    private bool _isDrifting;

    [Header("DriftingHandling")]
    [SerializeField] private float minimumAngleDrift;
    [SerializeField] private float normalDriftAngle;
    [SerializeField] private float minimumAngleCancelDrift;

    private bool _stayDrifting;

    [Header("Avoidance Handling")]
    [SerializeField] private float detectionDistance;
    [SerializeField] private LayerMask obstacleLayer;

    private float _avoidanceDir;

    private Transform _previousWayPoint;
    private bool _isAvoiding;
    [Header("Randomness Handling")]
    [SerializeField] private float steeringRandomness;
    [SerializeField] private float randomnessInterval;
    private float _steerRandomness;

    [Header("Stop Check")]

    [SerializeField] private float checkInterval;
    [SerializeField] private float maxPositionDistance;
    [SerializeField] private float reverseInterval;
    private float _checkTime;
    private float _reverseTime;
    private Vector3 _previousPosition;
    private bool _isReversing;

    [Header("ItemBox Targeting")]
    [SerializeField] private float itemDetectDistance = 20f;
    [SerializeField] private float itemDetectRadius = 1.5f;
    [SerializeField] private LayerMask itemLayer;

    private Transform _itemTarget;
    private bool _hasItemTarget;

    private void Start()
    {
        _carController = GetComponent<ArcadeCarController>();

        for (int i = 0; i < wayPointFolder.childCount; i++)
        {
            wayPoints.Add(wayPointFolder.GetChild(i));
        }

        _previousPosition = transform.position;

        GetWayPoint();
        StartCoroutine(HandleRandomness());
    }

    public void SetWaypoints()
    {
        wayPoints.Clear();
        for (int i = 0; i < wayPointFolder.childCount; i++)
        {
            wayPoints.Add(wayPointFolder.GetChild(i));
        }

        
    }

    public void ResetWaypoint()
    {
        _currentIndex = 0;
        GetWayPoint();
    }
    private void Update()
    {
        DrawWayPointLines();
        CheckWayPointsDistance();
        HandleItemBoxTargeting();
        HandleMiniWayPoints();
        HandleReversing();
        HandleAccelerating();
        HandleSteering();
        Reverse();
        //HandleAvoidance();
        _carController.SetInput(_accelerationInput, _steeringInput);

        if (!_stayDrifting)
        {
            HandleNormalDrifting();
        }

        else
        {
            HandleAlwaysDrifting();
        }
        


    }

    private void HandleMiniWayPoints()
    {
        Vector3 dirToWayPoint = (_wayPoint.position - transform.position).normalized;

        // Raycast offsets
        float sideOffset = 0.5f; // how far left/right the side rays are

        // Cast rays
        bool obstacleLeft = Physics.Raycast(transform.position + Vector3.up, transform.forward - transform.right * sideOffset, detectionDistance, obstacleLayer);
        bool obstacleRight = Physics.Raycast(transform.position + Vector3.up, transform.forward + transform.right * sideOffset, detectionDistance, obstacleLayer);
        Debug.DrawRay(transform.position + Vector3.up, (transform.forward - transform.right * sideOffset).normalized * detectionDistance, obstacleLeft ? Color.red : Color.green);
        Debug.DrawRay(transform.position + Vector3.up, (transform.forward + transform.right * sideOffset).normalized * detectionDistance, obstacleRight ? Color.red : Color.green);

        // Decide avoidance direction
        if (obstacleLeft && !obstacleRight)
        {
            _avoidanceDir = 1; // steer right
        }
        else if (!obstacleLeft && obstacleRight)
        {
            _avoidanceDir = -1; // steer left
        }
        else if (!obstacleLeft && !obstacleRight)
        {
            // Clear path, reset avoidance
            _avoidanceDir = 0;
        }

        // Apply avoidance offset
        float avoidanceStrength = 10f;
        Vector3 offset = transform.right * _avoidanceDir * avoidanceStrength;

        // 🔹 ItemBox override
        if (_hasItemTarget && _itemTarget != null)
        {
            _destination = _itemTarget.position + offset;
            Debug.DrawLine(transform.position, _destination, Color.magenta);
            return;
        }

        // 🔹 Default waypoint destination
        _destination = transform.position + dirToWayPoint * miniWayPointDistance + offset;
        Debug.DrawLine(transform.position, _destination, Color.blue);

    }

    private void HandleItemBoxTargeting()
    {
        if (_isReversing) return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;

        if (Physics.SphereCast(
            origin,
            itemDetectRadius,
            transform.forward,
            out hit,
            itemDetectDistance,
            itemLayer))
        {
            if (hit.collider.CompareTag("ItemBox"))
            {
                Vector3 dirToItem = (hit.transform.position - transform.position).normalized;

                // Must be in front of the car
                if (Vector3.Dot(transform.forward, dirToItem) > 0.25f)
                {
                    _itemTarget = hit.transform;
                    _hasItemTarget = true;
                    return;
                }
            }
        }

        _hasItemTarget = false;
        _itemTarget = null;
    }

    private void HandleReversing()
    {
        if (Vector3.Distance(transform.position, _previousPosition) < maxPositionDistance && !_isReversing)
        {
            _checkTime += Time.deltaTime;

            if (_checkTime > checkInterval)
            {
                if (TryGetComponent(out RespawnScript respawnScript))
                {
                    respawnScript.Respawn();
                }
            }
        }

        else
        {
            _checkTime = 0;
            _previousPosition = transform.position;
        }

        
    }
    
    private void Reverse()
    {
        if (_isReversing)
        {
            _accelerationInput = -1;
            _reverseTime += Time.deltaTime;

            if (_reverseTime > reverseInterval)
            {
                _reverseTime = 0;
                _isReversing = false;
                
                if (_isAvoiding)
                {
                    _isAvoiding = false;
                    _wayPoint = _previousWayPoint;
                }
            }

        }
        
    }

   

    private IEnumerator HandleRandomness()
    {
        while (true)
        {
            _steerRandomness = Random.Range(-steeringRandomness, steeringRandomness);
            yield return new WaitForSeconds(randomnessInterval);
        }
    }



    private void CheckWayPointsDistance()
    {
        _wayPointDistance = Vector3.Distance(transform.position, _wayPoint.position);

        if (_wayPointDistance < maxWayPointDistance)
        {
            CheckWayPoints();

        }


    }

    private void CheckWayPoints()
    {

        if (_isAvoiding)
        {
            _wayPoint = _previousWayPoint;
            _isAvoiding = false;
            return;
        }
        if (_currentIndex + 1 < wayPoints.Count)
        {
            _currentIndex++;

        }
        else
        {
            _currentIndex = 0;
        }

        GetWayPoint();
    }

    private void GetWayPoint()
    {
        if (wayPoints[_currentIndex].childCount == 0)
        {
            _wayPoint = wayPoints[_currentIndex];
            return;
        }
        _wayPoint = wayPoints[_currentIndex].GetComponent<WayPointHandler>().ReturnWayPoint(transform);
        CheckNewCPUBehaviour(wayPoints[_currentIndex].GetComponent<WayPointHandler>().GetCPUBehaviour());

        if (_isDrifting && !_stayDrifting)
        {
            _isDrifting = false;
            _carController.CancelDriftInput();
        }
    }

    private void CheckNewCPUBehaviour(CPUBehaviour state)
    {
        switch (state)
        {
            case CPUBehaviour.NoDifference:
                _stayDrifting = false;
                break;
            case CPUBehaviour.Drift:
                _stayDrifting = true;
                break;
        }
    }
    private void HandleAccelerating()
    {

        if (_isReversing) return;
        Vector3 dirToMove = (_destination - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMove);

        if (dot > 0)
        {
            _accelerationInput = Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, detectionDistance, obstacleLayer) 
                ? 0.5f : 1;
        }

        else
        {
            _accelerationInput = _wayPointDistance > reverseDistance ? 1 : -1;
        }

    }

    private void HandleSteering()
    {

        
        Vector3 dirToMove = (_destination - transform.position).normalized;

        Vector3 flatDir = Vector3.ProjectOnPlane(dirToMove, transform.up).normalized;

        _angleToDir = Vector3.SignedAngle(transform.forward, flatDir, transform.up);

        if (_isDrifting)
        {
            if (Mathf.Abs(_angleToDir) > minimumAngleDrift)
            {
                _steeringInput = 1 * Mathf.Sign(_angleToDir);
            }

            else if (Mathf.Abs(_angleToDir) < minimumAngleDrift && Mathf.Abs(_angleToDir) > normalDriftAngle) 
            {
                _steeringInput = 0;
            }

            else
            {
                _steeringInput = -1 * Mathf.Sign(_angleToDir);
            }

            _steeringInput += _steerRandomness;
        }

        else
        {
            if (Mathf.Abs(_angleToDir) < 2f)
            {
                _steeringInput = 0;
            }

            else
            {
                _steeringInput = _angleToDir > 0 ? 1 : -1;
            }
        }

        if (_isReversing) _steeringInput *= -1;
        
        
    }

    private void HandleNormalDrifting()
    {
        if (!_isDrifting && Mathf.Abs(_angleToDir) > minimumAngleDrift)
        {
            _isDrifting = true;
            _carController.StartDriftInput();
        }

        else if (_isDrifting)
        {
            bool changingTurn = Mathf.Sign(_angleToDir) != Mathf.Sign(_steeringInput);

            if (Mathf.Abs(_angleToDir) < minimumAngleCancelDrift || changingTurn)
            {
                _isDrifting = false;
                _carController.CancelDriftInput();
            }
        }
    }

    private void DrawWayPointLines()
    {

        if (wayPoints == null || wayPoints.Count == 0)
        {
            return;
        }
        for (int i = 0; i < wayPoints.Count - 1; i++)
        {
            Debug.DrawLine(wayPoints[i].position, wayPoints[i + 1].position, Color.yellow);
        }

        // Optional loop closure
        if (wayPoints.Count > 1)
        {
            Debug.DrawLine(wayPoints[wayPoints.Count - 1].position, wayPoints[0].position, Color.cyan);
        }
    }

    private void HandleObstacles()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, detectionDistance, obstacleLayer) && !_isAvoiding)
        {
            _isAvoiding = true;

            float smallestDistance = Mathf.Infinity;
            int index = 0;

            for (int i = 0; i < hit.transform.childCount; i++)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.GetChild(i).position);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    index = i;
                }
            }

            _previousWayPoint = _wayPoint;
            _wayPoint = hit.transform.GetChild(index);
        }
    }

    private void HandleAlwaysDrifting()
    {
        if (!_isDrifting && Mathf.Abs(_angleToDir) > minimumAngleDrift)
        {
            _isDrifting = true;
            _carController.StartDriftInput();
        }
    }


    private void OnDrawGizmos()
    {
        if (wayPointFolder == null) return;

        // Gather waypoints if not yet assigned (works in Editor mode too)
        List<Transform> tempWaypoints = new List<Transform>();
        for (int i = 0; i < wayPointFolder.childCount; i++)
        {
            tempWaypoints.Add(wayPointFolder.GetChild(i));
        }

        // Draw lines between each waypoint
        Gizmos.color = Color.yellow;
        for (int i = 0; i < tempWaypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(tempWaypoints[i].position, tempWaypoints[i + 1].position);
        }

        // Optionally connect the last waypoint back to the first
        if (tempWaypoints.Count > 1)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(tempWaypoints[tempWaypoints.Count - 1].position, tempWaypoints[0].position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return;
        if (other.CompareTag("WaypointTrigger") || other.CompareTag("FinishLine"))
        {
            if (other.transform.parent == wayPoints[_currentIndex])
            {
                CheckWayPoints();
            }
        }
    }

}


