using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArcadeCarController))]
public class AICarController : MonoBehaviour
{
    [Header("WayPointHandling")]
    [SerializeField] private Transform wayPointFolder;
    [SerializeField] private List<Transform> wayPoints = new List<Transform>();
    [SerializeField] private float maxWayPointDistance;

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

    [Header("Avoidance Handling")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask obstacleLayer;

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

    private void Update()
    {
        DrawWayPointLines();
        CheckWayPoints();
        HandleReversing();
        HandleAccelerating();
        HandleSteering();
        HandleObstacles();
        Reverse();
        //HandleAvoidance();
        _carController.SetInput(_accelerationInput, _steeringInput);
        HandleDrifting();

        if (_angleToDir > 30)
        {
            print(_angleToDir);
        }
    }

    private void HandleReversing()
    {
        if (Vector3.Distance(transform.position, _previousPosition) < maxPositionDistance && !_isReversing)
        {
            _checkTime += Time.deltaTime;

            if (_checkTime > checkInterval)
            {
                _isReversing = true;
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

    

    private void CheckWayPoints()
    {
        _wayPointDistance = Vector3.Distance(transform.position, _wayPoint.position);

        if (_wayPointDistance < maxWayPointDistance)
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


    }

    private void GetWayPoint()
    {
        if (wayPoints[_currentIndex].childCount == 0)
        {
            _wayPoint = wayPoints[_currentIndex];
            return;
        }

        float smallestDistance = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < wayPoints[_currentIndex].childCount; i++)
        {
            if (Vector3.Distance(transform.position, wayPoints[_currentIndex].GetChild(i).position) < smallestDistance)
            {
                index = i;
                smallestDistance = Vector3.Distance(transform.position, wayPoints[_currentIndex].GetChild(i).position);
            }
        }

        _wayPoint = wayPoints[_currentIndex].GetChild(index);
    }
    private void HandleAccelerating()
    {

        if (_isReversing) return;
        Vector3 dirToMove = (_wayPoint.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMove);

        if (dot > 0)
        {
            _accelerationInput = Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, detectionRange, obstacleLayer) 
                ? 0.5f : 1;
        }

        else
        {
            _accelerationInput = _wayPointDistance > reverseDistance ? 1 : -1;
        }

    }

    private void HandleSteering()
    {

        if (_isReversing)
        {
            _steeringInput = 0;
            return;
        }
        Vector3 dirToMove = (_wayPoint.position - transform.position).normalized;

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
        
        
    }

    private void HandleDrifting()
    {
        if (!_isDrifting && Mathf.Abs(_angleToDir) > minimumAngleDrift)
        {
            Debug.Log("Yes drift: " + _steeringInput);
            Debug.Log("This is the angle: " + _angleToDir);
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
            Debug.LogWarning("NO WAYPOINTS NOOB");
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

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, detectionRange, obstacleLayer) && !_isAvoiding)
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
}


