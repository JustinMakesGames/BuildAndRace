using System.IO;
using UnityEngine;

public static class SaveLevelName
{
    private static string path = Application.persistentDataPath + "/saveData.json";
    public static void SaveLevel(LevelName levelName)
    {
        LevelData data = new LevelData(levelName);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved Data");
    }

    public static LevelData LoadBuild()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file found at: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        Debug.Log("Loaded Savedata");
        return data;
    }


}
