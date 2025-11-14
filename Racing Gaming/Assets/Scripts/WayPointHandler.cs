using System.Collections.Generic;
using UnityEngine;

public enum WaypointKind
{
    Random,
    Middle,
    ClosestDistance
}

public enum CPUBehaviour
{
    NoDifference,
    Drift
}
public class WayPointHandler : MonoBehaviour
{
    public WaypointKind wayPointKind;
    public CPUBehaviour cpuBehaviour;
    [SerializeField] private List<Transform> wayPoints = new List<Transform>();
    public Transform ReturnWayPoint(Transform car)
    {
        switch (wayPointKind)
        {
            case WaypointKind.Random:

                Transform wayPoint = ReturnRandomWayPoint();
                return wayPoint;

            case WaypointKind.Middle:
                return wayPoints[0];

            case WaypointKind.ClosestDistance:
                return ReturnClosestWayPoint(car);

        }

        return wayPoints[0];
    }

    private Transform ReturnRandomWayPoint()
    {
        int randomValue = Random.Range(0, wayPoints.Count);
        return wayPoints[randomValue];
    }

    private Transform ReturnClosestWayPoint(Transform car)
    {
        float smallestDistance = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (Vector3.Distance(car.position, wayPoints[i].position) < smallestDistance)
            {
                index = i;
                smallestDistance = Vector3.Distance(car.position, wayPoints[i].position);
            }
        }

        return wayPoints[index];
    }

    public CPUBehaviour GetCPUBehaviour()
    {
        return cpuBehaviour;
    }
}
