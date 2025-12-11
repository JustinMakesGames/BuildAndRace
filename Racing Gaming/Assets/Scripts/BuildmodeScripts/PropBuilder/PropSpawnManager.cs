using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public class PropSpawnManager : MonoBehaviour
{

    public static PropSpawnManager Instance;

    //Handle Tiles
    [SerializeField] private List<GameObject> trackTiles = new List<GameObject>();
    [SerializeField] private Transform currentTrackTile;
    [SerializeField] private LayerMask colliderMask;
    private int _trackTileIndex;
    private int _previousTracktileIndex;
    private Transform _spawnCollider; 



    //Handle Player
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform currentPlayer;
    [SerializeField] private Transform playerFolder;
    private List<Transform> _players = new List<Transform>();
    private int _playerIndex;


    //Handle props
    [SerializeField] private GameObject prop;
    [SerializeField] private GameObject propPrefab;




    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
    }

    public void SetVariables(List<GameObject> trackTiles)
    {
        this.trackTiles = trackTiles;
        this.trackTiles.RemoveAt(this.trackTiles.Count - 1);

        for (int i = 0; i < playerFolder.childCount; i++)
        {
            _players.Add(playerFolder.GetChild(i));
        }
    }

    public void SetPropBuildMode()
    {
        SetTracktile(trackTiles[_trackTileIndex].transform);
        SetPlayer();
        SetCamera();
        

    }

    public void SetPlayer()
    {
        currentPlayer = _players[_playerIndex];
        var propBuilderMovement = currentPlayer.GetComponent<PropBuilderPlayerMovement>();

        propBuilderMovement.SetPlayerTurn();
        propBuilderMovement.SetProp(prop.transform);
        propBuilderMovement.SetTracktile(currentTrackTile, _spawnCollider);
    }

    public void SetNextTracktile()
    {
        print("Next tracktile");
        if (_trackTileIndex + 1 >= trackTiles.Count) return;
        _trackTileIndex++;
        SetPropBuildMode();
        prop.transform.position = ReturnSpawnPosition();
    }

    public void SetPreviousTracktile()
    {
        print("Previous tracktile");
        if (_trackTileIndex - 1 < 0) return;
        _trackTileIndex--;
        SetPropBuildMode();
        prop.transform.position = ReturnSpawnPosition();
    }

    public void SetTracktile(Transform tracktile)
    {
        currentTrackTile = tracktile;

    } 

    public void SetProp(GameObject prop)
    {
        if (this.prop == null)
        {
            print("NEW");
            this.prop = Instantiate(prop, ReturnSpawnPosition(), currentTrackTile.rotation);
        }

        else
        {
            print("Previous object destroyed, new object spawned");
            Vector3 position = this.prop.transform.position;
            Destroy(this.prop);
            this.prop = Instantiate(prop, position, currentTrackTile.rotation);
        }
    }

    public void SetCamera()
    {
        cam.Target.TrackingTarget = prop.transform;
    }
    private Vector3 ReturnSpawnPosition()
    {
        _spawnCollider = currentTrackTile
         .GetComponent<TracktileHandler>()
         .ReturnSpawnCollider();

        Vector3 rayStart = _spawnCollider.GetComponent<Collider>().bounds.center;

        Vector3 spawnPosition = _spawnCollider.GetComponent<Collider>().ClosestPoint(rayStart) + currentTrackTile.up * 10f;
        if (Physics.Raycast(spawnPosition, -currentTrackTile.up, out RaycastHit hit, Mathf.Infinity, colliderMask))
        {
            print("touched");
            return hit.point;
        }

        print("didn't touch");

        return spawnPosition;
    }

    public void SpawnProp()
    {

        var propTracktileManager = currentTrackTile.GetComponent<PropTracktileManager>();

        if (!propTracktileManager.CanSpawn()) return;
        Transform spawnedProp = Instantiate(prop, prop.transform.position, prop.transform.rotation, currentTrackTile).transform;

        propTracktileManager.AddProp(spawnedProp.GetComponent<PropHandler>().propScriptableObject, spawnedProp.localPosition, spawnedProp.localEulerAngles);
    }

    
}
