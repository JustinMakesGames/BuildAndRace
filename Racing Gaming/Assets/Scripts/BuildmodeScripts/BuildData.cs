using UnityEngine;

[System.Serializable]
public class BuildData
{
    public int[] trackTiles;
    public int[] trackTileConnectionPoints;

    public BuildData(BuildGameplay buildGameplay)
    {
        trackTiles = buildGameplay.trackTiles;
        trackTileConnectionPoints = buildGameplay.trackTileConnectionPoints;
    }
}
