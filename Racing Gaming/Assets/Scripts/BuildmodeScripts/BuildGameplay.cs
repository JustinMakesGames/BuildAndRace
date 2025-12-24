using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildGameplay : MonoBehaviour
{

    public static BuildGameplay Instance;
    public string levelName;
    public int[] trackTiles;
    public int[] trackTileConnectionPoints;
    public int[] propAmountPerTracktile;
    public int[] propIndexes;
    public float[] positionsX;
    public float[] positionsY;
    public float[] positionsZ;
    public float[] eulerAnglesX;
    public float[] eulerAnglesY;
    public float[] eulerAnglesZ;

    [SerializeField] private List<TrackTile> tiles = new List<TrackTile>();
    [SerializeField] private bool shouldLoad;
    private List<TrackTile> _spawnedTiles = new List<TrackTile>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        if (shouldLoad) LoadBuild();
    }

    public void SetLevelName(string levelName)
    {
        this.levelName = levelName;
    }
    public void SaveBuild(List<TrackTile> trackTileList, List<GameObject> trackTileGameObjects, List<PropPlacement> props, List<int> propAmountPerTracktile)
    {
        List<int> trackTileIndexes = new List<int>();
        List<int> trackTileConnectionPointList = new List<int>();
        List<int> propIndexes = new List<int>();
        List<Vector3> positions = new List<Vector3>();
        List<Vector3> eulerAngles = new List<Vector3>();
        

        for (int i = 0; i < trackTileList.Count; i++)
        {
            trackTileIndexes.Add(trackTileList[i].index);
            trackTileConnectionPointList.Add(trackTileGameObjects[i].GetComponent<TracktileHandler>().GetUsedConnectionPointIndex());
            
        }

        for (int i = 0; i < props.Count; i++)
        {
            propIndexes.Add(props[i].prop.index);
            positions.Add(props[i].position);
            eulerAngles.Add(props[i].rotation);
        }

        trackTiles = trackTileIndexes.ToArray();
        trackTileConnectionPoints = trackTileConnectionPointList.ToArray();
        this.propIndexes = propIndexes.ToArray();
        this.propAmountPerTracktile = propAmountPerTracktile.ToArray();


        positionsX = new float[positions.Count];
        positionsY = new float[positions.Count];
        positionsZ = new float[positions.Count];
        eulerAnglesX = new float[eulerAngles.Count];
        eulerAnglesY = new float[eulerAngles.Count];
        eulerAnglesZ = new float[eulerAngles.Count];


        for (int i = 0; i < positions.Count; i++)
        {
            positionsX[i] = positions[i].x;
            positionsY[i] = positions[i].y;
            positionsZ[i] = positions[i].z;
            eulerAnglesX[i] = eulerAngles[i].x;
            eulerAnglesY[i] = eulerAngles[i].y;
            eulerAnglesZ[i] = eulerAngles[i].z;
        }


        BuildSaveSystem.SaveBuild(this, levelName);
    }

    private void LoadBuild()
    {
        LevelData levelData = SaveLevelName.LoadBuild();

        string[] levelNames = levelData.levelNames;
        int index = levelData.levelIndex;
        BuildData buildData = BuildSaveSystem.LoadBuild(levelNames[index]);

        if (buildData == null) return;

        trackTiles = buildData.trackTiles;
        trackTileConnectionPoints = buildData.trackTileConnectionPoints;

        for (int i = 0; i < trackTiles.Length; i++)
        {
            for (int j = 0; j < tiles.Count; j++)
            {
                if (trackTiles[i] == tiles[j].index)
                {
                    _spawnedTiles.Add(tiles[j]);
                    break;
                }
            }
        }

        for (int i = 0; i < _spawnedTiles.Count; i++)
        {
            Debug.Log($"TrackConnectionPoint {trackTileConnectionPoints[i]}");
            Debug.Log($"SpawnedTile {_spawnedTiles[i]}");
            BuildManager.Instance.SpawnTileInGame(_spawnedTiles[i], trackTileConnectionPoints[i]);
        }

        BuildManager.Instance.SendDataToPropSpawner();

    }
}
