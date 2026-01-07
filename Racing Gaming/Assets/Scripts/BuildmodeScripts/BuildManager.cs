using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildManager : MonoBehaviour
{

    public static BuildManager Instance;

    [Header("Lists and Folders")]
    //Lists and Folders
    [SerializeField] private List<TrackTile> spawnedTrackTiles = new List<TrackTile>();
    [SerializeField] private List<GameObject> spawnedObjects = new List<GameObject>();
    [SerializeField] private Transform trackFolder;
    [SerializeField] private GameObject uiList;

    [Header("Camera")]
    //Camera
    [SerializeField] private Transform cameraTracker;
    [SerializeField] private bool needCamera;

    [Header("Current Tracktile Variables")]
    //Current Tracktile Variables
    [SerializeField] private bool isInRaceScene;
    private TrackTile _currentTrackTile;
    private GameObject _shownTracktile;
    private List<Transform> _currentConnectionTiles = new List<Transform>();
    private int _currentConnectionIndex;

    [Header("Previous Tracktile Variables")]
    //Previous Tracktile Variables
    [SerializeField] private Transform starterConnectionTile;
    private Transform _previousConnectionTile;

    [Header("Handle Position And Rotation")]
    //Handle Position And Rotation
    private GameObject _connectionTileParent;

    [Header("Collider Variables")]
    //Check Colliders
    [SerializeField] private List<Transform> colliderTransforms = new List<Transform>();

    [Header("Spawn Renderer")]
    [SerializeField] private Material previewMaterial;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cam;



    private void Awake()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if (Instance != this)
        {
            Instance = this;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        _previousConnectionTile = starterConnectionTile;
        
    }

    public void SetCameraTracker()
    {
        cam.Target.TrackingTarget = cameraTracker;
    }
    public void ShowNewTile(TrackTile tile, bool hasNewTile)
    {
        DeleteOldTile(hasNewTile);
        _currentTrackTile = tile;
        _shownTracktile = Instantiate(tile.trackTile, transform.position, transform.rotation); 
        if (_shownTracktile.TryGetComponent(out TracktileHandler trackTileHandler))
        {
            _currentConnectionTiles = trackTileHandler.GetConnectionPoints();

            if (_currentConnectionTiles.Count == 1)
            {
                _currentConnectionIndex = 0;
            }
                      
        }
        RotateAndPositionTrackTile(_currentConnectionTiles[_currentConnectionIndex]);

        if (needCamera)
        {
            cameraTracker.position = trackTileHandler.GetCameraPoint().position;
            cameraTracker.rotation = Quaternion.FromToRotation(cameraTracker.up, _previousConnectionTile.up) * cameraTracker.rotation;
        }

        GetNewColliders();
        ShowSpawnRenderer();
        
    }

    private void ShowSpawnRenderer()
    {
        Renderer[] renderers = _shownTracktile.GetComponentsInChildren<Renderer>();
        

        foreach (Renderer r in renderers)
        {
            Material[] materials = r.materials;
            Material[] newMaterials = new Material[materials.Length + 1];

            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }

            newMaterials[newMaterials.Length - 1] = previewMaterial;
            r.materials = newMaterials;
        }
    }

    private void ShowNormalRenderer()
    {
        Renderer[] renderers = _shownTracktile.GetComponentsInChildren<Renderer>();


        foreach (Renderer r in renderers)
        {
            Material[] materials = r.materials;
            Material[] newMaterials = new Material[materials.Length - 1];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }

            r.materials = newMaterials;
        }
    }

    private void GetNewColliders()
    {
        colliderTransforms = _shownTracktile.GetComponent<TracktileHandler>().ReturnColliderTransforms();
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
        if (!CanTheTrackSpawn()) return;
        spawnedTrackTiles.Add(_currentTrackTile);
        spawnedObjects.Add(_shownTracktile);

        _shownTracktile.GetComponent<TracktileHandler>().SetConnectedPoint(_previousConnectionTile);
        _shownTracktile.GetComponent<TracktileHandler>().SetConnectionPointIndex(_currentConnectionIndex);
        _shownTracktile.transform.parent = trackFolder;

        ShowNormalRenderer();

        if (_shownTracktile.GetComponent<TracktileHandler>().CheckIfFinishLine())
        {
            if (!isInRaceScene)
            {
                //BuildGameplay.Instance.SaveBuild(spawnedTrackTiles, spawnedObjects);
                //SceneManager.LoadScene("RaceScene");
                FinishHandling.Instance.SetTracktiles(spawnedTrackTiles, spawnedObjects);
                PropSpawnManager.Instance.SetVariables(spawnedObjects);
                PropSpawnManager.Instance.SetPropBuildMode();
            }

            else
            {
                WaypointBuildHandler.Instance.SetWaypointFolder();
            }
            
        }

        else
        {
            _previousConnectionTile = _currentConnectionTiles[ReturnNumber(index)];
            ShowNewTile(_currentTrackTile, true);
        }
    }

    private bool CanTheTrackSpawn()
    {
        for (int i = 0; i < colliderTransforms.Count; i++)
        {
            Collider[] colliders = Physics.OverlapBox(colliderTransforms[i].position, colliderTransforms[i].localScale / 2, Quaternion.identity);

            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].CompareTag("TrackCollider") && colliders[j].transform.parent != colliderTransforms[i].parent)
                {
                    return false;
                }
            }
        }

        return true;
    }
    public void SpawnTileInGame(TrackTile trackTile, int connectionPointIndex)
    {
        ShowNewTile(trackTile, false);
        RotateAndPositionTrackTile(_currentConnectionTiles[connectionPointIndex]);
        RotateDeathTrigger(connectionPointIndex);
        WaypointBuildHandler.Instance.MakeWaypointFolder(_shownTracktile.GetComponent<TracktileHandler>().GetWaypointFolder(), connectionPointIndex);
        SpawnNewTile(connectionPointIndex);
    }

    public void SendDataToPropSpawner()
    {
        PropInGameSpawner.Instance.LoadProps(spawnedObjects);
    }

    public void Rotate()
    {
        if (_currentConnectionTiles.Count == 1) return;
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

    private void RotateDeathTrigger(int number)
    {
        if (number == 1)
        {
            List<Transform> deathTriggers = _shownTracktile.GetComponent<TracktileHandler>().ReturnDeathTriggers();

            foreach (Transform t in deathTriggers)
            {
                if (t.TryGetComponent(out SetRespawnScript respawnScript))
                {
                    t.localEulerAngles += new Vector3(0, 180, 0);
                }
                
            }
        }
    }


}
