using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishHandling : MonoBehaviour
{
    public static FinishHandling Instance;

    [SerializeField] private List<TrackTile> tiles = new List<TrackTile>();
    [SerializeField] private List<GameObject> trackTileObjects = new List<GameObject>();

    [SerializeField] private List<PropPlacement> props = new List<PropPlacement>();
    [SerializeField] private List<int> propAmountPerTracktile = new List<int>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetTracktiles(List<TrackTile> tiles, List<GameObject> trackTileObjects)
    {
        this.tiles = tiles;
        this.trackTileObjects = trackTileObjects;
    }

    public void SetProps()
    {
        for (int i = 0; i < trackTileObjects.Count; i++)
        {
            List<PropPlacement> propPlacements = trackTileObjects[i].GetComponent<PropTracktileManager>().GetProp();

            propAmountPerTracktile.Add(propPlacements.Count);
            for (int j = 0; j < propPlacements.Count; j++)
            {
                props.Add(propPlacements[j]);
            }

        }
    }

    public void FinishBuild()
    {
        BuildGameplay.Instance.SaveBuild(tiles, trackTileObjects, props, propAmountPerTracktile);
        SceneManager.LoadScene("RaceScene");
        /*BuildData newData = BuildSaveSystem.LoadBuild();
        print($"Tracktiles: {newData.trackTiles.Length}");
        print($"ConnectionPoints: {newData.trackTileConnectionPoints.Length}");
        print($"TracktilePropAmount: {newData.tracktilePropAmount.Length}");
        print($"Positions: {newData.positionsX.Length} {newData.positionsY.Length} {newData.positionsZ.Length}");
        print($"PropIndexes: {newData.propIndexes.Length}");*/
    }
}
