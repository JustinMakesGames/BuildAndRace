using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildManager : MonoBehaviour
{

    public static BuildManager Instance;

    //Lists and Folders
    [SerializeField] private List<TrackTile> spawnedTrackTiles = new List<TrackTile>();
    [SerializeField] private List<GameObject> spawnedObjects = new List<GameObject>();
    [SerializeField] private Transform trackFolder;
    [SerializeField] private GameObject uiList;

    //Camera
    [SerializeField] private Transform cameraTracker;
    [SerializeField] private bool needCamera;

    //Current Tracktile Variables
    [SerializeField] private bool isInRaceScene;
    private TrackTile _currentTrackTile;
    private GameObject _shownTracktile;
    private List<Transform> _currentConnectionTiles = new List<Transform>();
    private int _currentConnectionIndex;

    //Previous Tracktile Variables
    [SerializeField] private Transform starterConnectionTile;
    private Transform _previousConnectionTile;

    //Handle Position And Rotation
    private GameObject _connectionTileParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Cursor.lockState = CursorLockMode.Locked;
        _previousConnectionTile = starterConnectionTile;
    }

    public void ShowNewTile(TrackTile tile, bool hasNewTile)
    {
        DeleteOldTile(hasNewTile);
        _currentTrackTile = tile;
        _shownTracktile = Instantiate(tile.trackTile, transform.position, transform.rotation); 
        if (_shownTracktile.TryGetComponent(out TracktileHandler trackTileHandler))
        {
            _currentConnectionTiles = trackTileHandler.GetConnectionPoints();
                      
        }
        RotateAndPositionTrackTile(_currentConnectionTiles[_currentConnectionIndex]);

        if (needCamera)
        {
            cameraTracker.position = trackTileHandler.GetCameraPoint().position;
            cameraTracker.rotation = Quaternion.FromToRotation(cameraTracker.up, _previousConnectionTile.up) * cameraTracker.rotation;
        }
        
    }

    public void RemovePiece()
    {
        if (spawnedObjects.Count <= 0) return;
        GameObject oldObject = spawnedObjects[spawnedObjects.Count - 1];
        spawnedTrackTiles.RemoveAt(spawnedTrackTiles.Count - 1);
        spawnedObjects.Remove(oldObject);

        _previousConnectionTile = oldObject.GetComponent<TracktileHandler>().GetConnectedPoint();

        Destroy(oldObject);

        ShowNewTile(_currentTrackTile, false);
    }


    private void DeleteOldTile(bool hasNewTile)
    {
        if (!hasNewTile)
        {
            Destroy(_shownTracktile);
        }
    }

    public void GetSpawnNewTileInput()
    {
        SpawnNewTile(_currentConnectionIndex);
    }

    public void SpawnNewTile(int index)
    {
        spawnedTrackTiles.Add(_currentTrackTile);
        spawnedObjects.Add(_shownTracktile);

        _shownTracktile.GetComponent<TracktileHandler>().SetConnectedPoint(_previousConnectionTile);
        _shownTracktile.GetComponent<TracktileHandler>().SetConnectionPointIndex(_currentConnectionIndex);
        _shownTracktile.transform.parent = trackFolder;
        _previousConnectionTile = _currentConnectionTiles[ReturnNumber(index)];

        if (_shownTracktile.GetComponent<TracktileHandler>().CheckIfFinishLine())
        {
            if (!isInRaceScene)
            {
                BuildGameplay.Instance.SaveBuild(spawnedTrackTiles, spawnedObjects);
                SceneManager.LoadScene("RaceScene");
            }
            
        }

        else
        {
            ShowNewTile(_currentTrackTile, true);
        }
    }

    public void SpawnTileInGame(TrackTile trackTile, int connectionPoint)
    {
        ShowNewTile(trackTile, false);
        RotateAndPositionTrackTile(_currentConnectionTiles[connectionPoint]);
        SpawnNewTile(connectionPoint);
    }

    public void Rotate()
    {
        _currentConnectionIndex = ReturnNumber(_currentConnectionIndex);
        RotateAndPositionTrackTile(_currentConnectionTiles[_currentConnectionIndex]);
    }

    private int ReturnNumber(int number)
    {
        if (number == 0) return 1;

        return 0;
    }
    private void RotateAndPositionTrackTile(Transform connectionTile)
    {

        if (_connectionTileParent != null)
        {
            _shownTracktile.transform.parent = null;
            Destroy(_connectionTileParent);
        }
        _connectionTileParent = Instantiate(connectionTile.gameObject, connectionTile.position, connectionTile.rotation);
        _shownTracktile.transform.parent = _connectionTileParent.transform;

        _connectionTileParent.transform.position = _previousConnectionTile.position;
        _connectionTileParent.transform.rotation = _previousConnectionTile.rotation * Quaternion.Euler(0, 180, 0);
    }


}
