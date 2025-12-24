using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string[] levelNames;
    public int levelIndex;

    public LevelData(LevelName levelName)
    {
        levelNames = levelName.levelNames;
        levelIndex = levelName.index;
    }
}
