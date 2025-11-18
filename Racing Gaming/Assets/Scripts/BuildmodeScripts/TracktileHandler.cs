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
}
