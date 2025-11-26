using System.Collections.Generic; 
using UnityEngine;

public class WaypointBuildHandler : MonoBehaviour
{
    public static WaypointBuildHandler Instance;

    [SerializeField] private List<Transform> waypointList = new List<Transform>();
    [SerializeField] private PlacementManagement placementManagement;
    [SerializeField] private Transform waypointFolder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void MakeWaypointFolder(Transform trackTileWaypoints, int rotationIndex)
    {
        if (waypointFolder == null) return;
        List<Transform> trackTileWaypointList = new List<Transform>();

        for (int i = 0; i < trackTileWaypoints.childCount; i++)
        {
            trackTileWaypointList.Add(trackTileWaypoints.GetChild(i));
        }

        if (rotationIndex == 1) trackTileWaypointList.Reverse();

        for (int i = 0; i < trackTileWaypointList.Count; i++)
        {
            trackTileWaypointList[i].parent = waypointFolder;
            waypointList.Add(trackTileWaypointList[i]);
        }
    }

    public void SetWaypointFolder()
    {
        placementManagement.SetPlacement(waypointList);
    }
}
