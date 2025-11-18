using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class BuildSaveSystem
{
    public static void SaveBuild(BuildGameplay build)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveData.epic";
        FileStream stream = new FileStream(path, FileMode.Create);

        BuildData data = new BuildData(build);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static BuildData LoadBuild()
    {
        string path = Application.persistentDataPath + "/saveData.epic";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            BuildData data = formatter.Deserialize(stream) as BuildData;
            stream.Close();

            return data;
        }

        return null;

    }
}
