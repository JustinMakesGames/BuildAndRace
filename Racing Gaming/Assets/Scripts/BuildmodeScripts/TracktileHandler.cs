using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class TracktileHandler : MonoBehaviour
{
    public TrackTile trackTile;
    [SerializeField] private List<Transform> connectionPoints = new List<Transform>();
    
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private bool isFinishLine;

    private Transform _connectedPoint;
    private int _usedConnectionPointIndex;

    [SerializeField] private Transform colliderFolder;
    [SerializeField] private Transform waypointFolder;

    [SerializeField] private Transform deathFolder;
    [SerializeField] private Transform spawnCollider;
    [SerializeField] private Transform spawnPosition;

    //Setters
    public void SetConnectedPoint(Transform connectedPoint)
    {
        _connectedPoint = connectedPoint;
    }

    public void SetConnectionPointIndex(int usedConnectionPointIndex)
    {
        _usedConnectionPointIndex = usedConnectionPointIndex;
    }

    //Getters
    public List<Transform> GetConnectionPoints()
    {
        return connectionPoints;
    }

    public Transform GetCameraPoint()
    {
        return cameraPoint;
    }


    public Transform GetConnectedPoint()
    {
        return _connectedPoint;
    }

    public bool CheckIfFinishLine()
    {
        return isFinishLine;
    }

    public int GetUsedConnectionPointIndex()
    {
        return _usedConnectionPointIndex;
    }

    public Transform GetWaypointFolder()
    {
        return waypointFolder;
    }

    public List<Transform> ReturnColliderTransforms()
    {
        List<Transform> colliderTransforms = new List<Transform>();

        for (int i = 0; i < colliderFolder.childCount; i++)
        {
            colliderTransforms.Add(colliderFolder.GetChild(i));
        }

        return colliderTransforms;
    }

    public List<Transform> ReturnDeathTriggers()
    {
        List<Transform> deathTransforms = new List<Transform>();

        for (int i = 0; i < deathFolder.childCount; i++)
        {
            deathTransforms.Add(deathFolder.GetChild(i));
        }

        return deathTransforms;
    }

    public Transform ReturnSpawnCollider()
    {
        return spawnCollider;
    }

    public Transform ReturnSpawnPosition()
    {
        return spawnPosition;
    }
}
