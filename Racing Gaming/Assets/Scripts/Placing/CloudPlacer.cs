/*using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CloudPlacerEditor : EditorWindow
{
    public List<GameObject> cloudPrefabs = new();
    public int count = 1000;
    public Vector3 areaSize = new Vector3(500, 100, 500);
    public Vector3 areaCenter = Vector3.zero;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    [MenuItem("Tools/Cloud Placer")]
    public static void ShowWindow()
    {
        GetWindow<CloudPlacerEditor>("Cloud Placer");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Cloud Prefabs", EditorStyles.boldLabel);

        int newSize = Mathf.Max(0, EditorGUILayout.IntField("Number of Prefabs", cloudPrefabs.Count));
        while (newSize > cloudPrefabs.Count) cloudPrefabs.Add(null);
        while (newSize < cloudPrefabs.Count) cloudPrefabs.RemoveAt(cloudPrefabs.Count - 1);

        for (int i = 0; i < cloudPrefabs.Count; i++)
        {
            cloudPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                $"Prefab {i + 1}", cloudPrefabs[i], typeof(GameObject), false);
        }

        EditorGUILayout.Space();

        count = EditorGUILayout.IntField("Cloud Count", count);
        areaCenter = EditorGUILayout.Vector3Field("Area Center", areaCenter);
        areaSize = EditorGUILayout.Vector3Field("Area Size (X = diameter)", areaSize);
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);

        if (GUILayout.Button("Place Clouds"))
        {
            PlaceClouds();
        }
    }

    void PlaceClouds()
    {
        if (cloudPrefabs.Count == 0 || cloudPrefabs.Exists(p => p == null))
        {
            Debug.LogWarning("Please assign all prefab slots.");
            return;
        }

        GameObject cloudParent = new GameObject("Clouds");

        float radius = areaSize.x * 0.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Count)];

            // 🔵 Circle placement on XZ plane
            Vector2 circlePoint = Random.insideUnitCircle * radius;

            Vector3 position = areaCenter + new Vector3(
                circlePoint.x,
                Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f),
                circlePoint.y
            );

            GameObject cloud =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            cloud.transform.SetPositionAndRotation(position, Quaternion.identity);

            float scale = Random.Range(minScale, maxScale);
            cloud.transform.localScale = Vector3.one * scale;

            cloud.transform.SetParent(cloudParent.transform);
            cloud.isStatic = true;
        }

        Debug.Log($"Placed {count} clouds in a circular area.");
    }
}*/
