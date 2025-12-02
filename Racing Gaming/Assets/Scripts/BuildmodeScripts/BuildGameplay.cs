using System.Collections.Generic;
using UnityEngine;

public class BuildGameplay : MonoBehaviour
{

    public static BuildGameplay Instance;
    public int[] trackTiles;
    public int[] trackTileConnectionPoints;
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

    public void SaveBuild(List<TrackTile> trackTileList, List<GameObject> trackTileGameObjects)
    {
        List<int> trackTileIndexes = new List<int>();
        List<int> trackTileConnectionPointList = new List<int>();

        for (int i = 0; i < trackTileList.Count; i++)
        {
            trackTileIndexes.Add(trackTileList[i].index);
            trackTileConnectionPointList.Add(trackTileGameObjects[i].GetComponent<TracktileHandler>().GetUsedConnectionPointIndex());
        }

        trackTiles = trackTileIndexes.ToArray();
        trackTileConnectionPoints = trackTileConnectionPointList.ToArray();
        BuildSaveSystem.SaveBuild(this);
    }

    private void LoadBuild()
    {
        BuildData buildData = BuildSaveSystem.LoadBuild();

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
    }
}
