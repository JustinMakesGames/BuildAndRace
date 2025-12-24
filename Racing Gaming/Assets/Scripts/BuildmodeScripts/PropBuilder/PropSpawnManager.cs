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
    private Transform _spawnCollider; 



    //Handle Player
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform camPosition;
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
        for (int i = 0; i < trackTiles.Count; i++)
        {
            this.trackTiles.Add(trackTiles[i]);
        }
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
        propBuilderMovement.SetProp(prop.transform, camPosition);
        propBuilderMovement.SetTracktile(currentTrackTile, _spawnCollider);
    }

    public void SetNextTracktile()
    {
        print("Next tracktile");
        if (_trackTileIndex + 1 >= trackTiles.Count) return;
        _trackTileIndex++;
        MovePropToTile();
    }

    public void SetPreviousTracktile()
    {
        print("Previous tracktile");
        if (_trackTileIndex - 1 < 0) return;
        _trackTileIndex--;
        MovePropToTile();
        
    }

    private void MovePropToTile()
    {
        SetPropBuildMode();
        prop.transform.rotation = currentTrackTile.rotation;
        prop.transform.position = ReturnSpawnPosition();
    }

    public void SetTracktile(Transform tracktile)
    {
        currentTrackTile = tracktile;
        _spawnCollider = currentTrackTile.GetComponent<TracktileHandler>().ReturnSpawnCollider();

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
        cam.Target.TrackingTarget = camPosition;
        camPosition.position = prop.transform.position;
    }
    private Vector3 ReturnSpawnPosition()
    {
        currentTrackTile.TryGetComponent(out TracktileHandler trackTileHandler);

        _spawnCollider = trackTileHandler
         .ReturnSpawnCollider();

        Vector3 rayStart = Vector3.zero;

        if (trackTileHandler.ReturnSpawnPosition() != null)
        {
            rayStart = trackTileHandler.ReturnSpawnPosition().position;
        }

        else
        {
            rayStart = _spawnCollider.GetComponent<Collider>().bounds.center;
        }
        

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

        ChangePlayerTurn();
    }

    private void ChangePlayerTurn()
    {
        var oldPlayer = currentPlayer.GetComponent<PropBuilderPlayerMovement>();
        oldPlayer.DisablePlayerTurn();
        _playerIndex = (_playerIndex >= _players.Count - 1) ? 0 : _playerIndex + 1;
        SetPropBuildMode();
    }

    public void EndPlayerTurns()
    {
        var player = currentPlayer.GetComponent<PropBuilderPlayerMovement>();
        player.DisablePlayerTurn();
    }

    
}
