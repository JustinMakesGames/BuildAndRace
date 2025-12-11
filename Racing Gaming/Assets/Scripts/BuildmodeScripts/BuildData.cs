using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class BuildData
{
    public int[] trackTiles;
    public int[] trackTileConnectionPoints;

    public int[] tracktilePropAmount;
    public int[] propIndexes;
    public float[] positionsX;
    public float[] positionsY;
    public float[] positionsZ;

    public BuildData(BuildGameplay buildGameplay)
    {
        trackTiles = buildGameplay.trackTiles;
        trackTileConnectionPoints = buildGameplay.trackTileConnectionPoints;
        tracktilePropAmount = buildGameplay.propAmountPerTracktile;
        propIndexes = buildGameplay.propIndexes;
        positionsX = buildGameplay.positionsX;
        positionsY = buildGameplay.positionsY;
        positionsZ = buildGameplay.positionsZ;

    }
}