using System.Collections.Generic;
using UnityEngine;

public class LevelName : MonoBehaviour
{
    public static LevelName Instance;
    public string[] levelNames;
    public int index;
    private List<string> levelNameList = new List<string>();

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        LevelData levelData = SaveLevelName.LoadBuild();

        if (levelData == null) return;
        levelNames = levelData.levelNames;

        for (int i = 0; i < levelNames.Length; i++)
        {
            levelNameList.Add(levelNames[i]);
        }
    }

    public void SetLevelName(string levelName)
    {
        levelNameList.Add(levelName);
        levelNames = levelNameList.ToArray();
        index = levelNames.Length - 1;
        BuildGameplay.Instance.SetLevelName(levelName);
        SaveLevelName.SaveLevel(this);
    }

    public void PlayLevel(int index)
    {
        this.index = index;
        SaveLevelName.SaveLevel(this);
    }
}
