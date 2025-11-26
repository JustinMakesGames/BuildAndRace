using UnityEngine;
using System.IO;

public static class BuildSaveSystem
{
    private static string path = Application.persistentDataPath + "/saveData.json";
    public static void SaveBuild(BuildGameplay build)
    {
        BuildData data = new BuildData(build);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved Data");
    }

    public static BuildData LoadBuild()
    {
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
}
