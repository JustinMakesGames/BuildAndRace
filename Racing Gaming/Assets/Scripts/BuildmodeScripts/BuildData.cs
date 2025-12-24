using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class BuildData
{
    public string levelName;
    public int[] trackTiles;
    public int[] trackTileConnectionPoints;

    public int[] tracktilePropAmount;
    public int[] propIndexes;
    public float[] positionsX;
    public float[] positionsY;
    public float[] positionsZ;
    public float[] eulerAnglesX;
    public float[] eulerAnglesY;
    public float[] eulerAnglesZ;

    public BuildData(BuildGameplay buildGameplay)
    {
        levelName = buildGameplay.levelName;
        trackTiles = buildGameplay.trackTiles;
        trackTileConnectionPoints = buildGameplay.trackTileConnectionPoints;
        tracktilePropAmount = buildGameplay.propAmountPerTracktile;
        propIndexes = buildGameplay.propIndexes;
        positionsX = buildGameplay.positionsX;
        positionsY = buildGameplay.positionsY;
        positionsZ = buildGameplay.positionsZ;
        eulerAnglesX = buildGameplay.eulerAnglesX;
        eulerAnglesY = buildGameplay.eulerAnglesY;
        eulerAnglesZ = buildGameplay.eulerAnglesZ;

    }
}