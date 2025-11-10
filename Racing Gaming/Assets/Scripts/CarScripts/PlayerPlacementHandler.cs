using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerPlacementHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private TMP_Text placementText;

    [SerializeField] private int _currentLap;
    private Transform _currentWaypoint;
    private int _currentIndex;

    

    

    private void GetNextCheckpoint()
    {
        print("GOT CHECKPOINTTT");

        _currentIndex = (_currentIndex + 1 < waypoints.Count) ? _currentIndex + 1 : 0;
        _currentWaypoint = waypoints[_currentIndex];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine") && other.transform.parent == _currentWaypoint)
        {
            _currentLap++;
        }

        if ((other.CompareTag("WaypointTrigger") || other.CompareTag("FinishLine")) && other.transform.parent == _currentWaypoint)
        {
            GetNextCheckpoint();
        }

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
