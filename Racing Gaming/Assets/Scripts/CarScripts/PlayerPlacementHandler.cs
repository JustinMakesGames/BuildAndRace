using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPlacementHandler : MonoBehaviour
{
    [Header("Checkpoint Handling")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private TMP_Text placementText;
    [SerializeField] private TMP_Text lapText;
    [SerializeField] private int _currentLap;
    private Transform _currentWaypoint;
    private int _currentIndex;
    

    [Header("Finish Handling")]
    [SerializeField] private GameObject finishText;
    private bool _hasFinished;

    

    

    private void GetNextCheckpoint()
    {
        _currentIndex = (_currentIndex + 1 < waypoints.Count) ? _currentIndex + 1 : 0;
        _currentWaypoint = waypoints[_currentIndex];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine") && other.transform.parent == _currentWaypoint)
        {
            SetLap();
        }

        if ((other.CompareTag("WaypointTrigger") || other.CompareTag("FinishLine")) && other.transform.parent == _currentWaypoint)
        {
            GetNextCheckpoint();
        }

    }

    private void SetLap()
    {

        if (!RaceManager.Instance || _hasFinished) return;
        print("NEW LAPPPP");

        _currentLap++;

        
        if (_currentLap > RaceManager.Instance.GetMaxLapCount())
        {
            _hasFinished = true;
            RaceManager.Instance.FinishPlayer(transform);
            StartCoroutine(HandleFinish());
        }

        else
        {
            lapText.text = $"Lap: {_currentLap}/{RaceManager.Instance.GetMaxLapCount()}";
        }
    }

    private IEnumerator HandleFinish()
    {
        if (!finishText) yield break;
        finishText.SetActive(true);
        yield return new WaitForSeconds(1);
        finishText.SetActive(false);
    }

    public void SetList(List<Transform> waypointList)
    {
        for (int i = 0; i < waypointList.Count; i++)
        {
            waypoints.Add(waypointList[i]);
        }

        if (waypoints.Count == 0) return;
        _currentWaypoint = waypoints[_currentIndex];
    }

    public int ReturnLapCount()
    {
        return _currentLap;
    }
    public int ReturnWayPointIndex()
    {
        return _currentIndex;
    }
    public float ReturnDistance()
    {
        float distance = Vector3.Distance(transform.position, _currentWaypoint.position);

        return distance;
    }

    public void SetPlacement(int placement)
    {
        placementText.text = placement.ToString();
    } 


}
