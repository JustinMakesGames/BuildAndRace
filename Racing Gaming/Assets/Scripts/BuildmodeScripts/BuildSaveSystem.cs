using UnityEngine;
using System.IO;

public static class BuildSaveSystem
{
    public static void SaveBuild(BuildGameplay build, string levelName)
    {
        string path = Application.persistentDataPath + $"/{levelName}.json";
        BuildData data = new BuildData(build);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved Data");
    }

    public static BuildData LoadBuild(string levelName)
    {
        string path = Application.persistentDataPath + $"/{levelName}.json";
        if (!File.Exists(path))
        {
            Debug.Log("No save file found at: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        BuildData data = JsonUtility.FromJson<BuildData>(json);

        Debug.Log("Loaded Savedata");
        return data;
    }

    public static void DeleteBuild(string levelName)
    {
        string path = Application.persistentDataPath + $"{levelName}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
