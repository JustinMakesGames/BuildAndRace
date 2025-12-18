using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BuildingPlacer : EditorWindow
{
    public List<GameObject> buildingPrefabs = new();

    [Header("Boundary")]
    public Vector2 boundarySize = new Vector2(200, 200);
    public Vector3 areaCenter = Vector3.zero;

    [Header("Grid Settings")]
    public float baseTileSize = 10f;
    public float spacingRandomness = 2f;

    [Header("Spots")]
    public int spotCount = 5;
    public float spotRadius = 30f;

    [Header("Random Scatter")]
    public int randomBuildingCount = 20;
    public float minBuildingDistance = 8f;

    [Header("Terrain")]
    public float terrainYOffset = 0f;

    List<Vector3> placedPositions = new();

    [MenuItem("Tools/Building Placer")]
    public static void ShowWindow()
    {
        GetWindow<BuildingPlacer>("Building Placer");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Building Prefabs", EditorStyles.boldLabel);

        int newSize = Mathf.Max(0,
            EditorGUILayout.IntField("Number of Prefabs", buildingPrefabs.Count));

        while (newSize > buildingPrefabs.Count) buildingPrefabs.Add(null);
        while (newSize < buildingPrefabs.Count) buildingPrefabs.RemoveAt(buildingPrefabs.Count - 1);

        for (int i = 0; i < buildingPrefabs.Count; i++)
        {
            buildingPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                $"Prefab {i + 1}", buildingPrefabs[i], typeof(GameObject), false);
        }

        EditorGUILayout.Space();

        areaCenter = EditorGUILayout.Vector3Field("Area Center", areaCenter);
        boundarySize = EditorGUILayout.Vector2Field("Boundary Size (X/Z)", boundarySize);

        EditorGUILayout.Space();

        baseTileSize = EditorGUILayout.FloatField("Base Tile Size", baseTileSize);
        spacingRandomness = EditorGUILayout.FloatField("Spacing Randomness", spacingRandomness);

        EditorGUILayout.Space();

        spotCount = EditorGUILayout.IntField("Spot Count", spotCount);
        spotRadius = EditorGUILayout.FloatField("Spot Radius", spotRadius);

        EditorGUILayout.Space();

        randomBuildingCount = EditorGUILayout.IntField("Random Building Count", randomBuildingCount);
        minBuildingDistance = EditorGUILayout.FloatField("Min Building Distance", minBuildingDistance);

        EditorGUILayout.Space();

        terrainYOffset = EditorGUILayout.FloatField("Terrain Y Offset", terrainYOffset);

        EditorGUILayout.Space();

        if (GUILayout.Button("Place Buildings"))
        {
            PlaceBuildings();
        }
    }

    void PlaceBuildings()
    {
        if (buildingPrefabs.Count == 0 || buildingPrefabs.Exists(p => p == null))
        {
            Debug.LogWarning("Please assign all building prefabs.");
            return;
        }

        placedPositions.Clear();
        GameObject parent = new GameObject("Buildings");

        // ---------- SPOT CLUSTERS ----------
        for (int s = 0; s < spotCount; s++)
        {
            Vector3 spotCenter = areaCenter + new Vector3(
                Random.Range(-boundarySize.x * 0.5f, boundarySize.x * 0.5f),
                0f,
                Random.Range(-boundarySize.y * 0.5f, boundarySize.y * 0.5f)
            );

            int gridSize = Mathf.CeilToInt(spotRadius / baseTileSize);

            for (int x = -gridSize; x <= gridSize; x++)
            {
                for (int z = -gridSize; z <= gridSize; z++)
                {
                    Vector3 offset = new Vector3(
                        x * baseTileSize + Random.Range(-spacingRandomness, spacingRandomness),
                        0f,
                        z * baseTileSize + Random.Range(-spacingRandomness, spacingRandomness)
                    );

                    if (offset.magnitude > spotRadius)
                        continue;

                    Vector3 position = spotCenter + offset;

                    if (!IsInsideBoundary(position))
                        continue;

                    if (!IsPositionValid(position))
                        continue;

                    PlaceBuilding(position, parent);
                }
            }
        }

        // ---------- RANDOM SCATTER ----------
        int attempts = 0;
        int placed = 0;

        while (placed < randomBuildingCount && attempts < randomBuildingCount * 10)
        {
            attempts++;

            Vector3 position = areaCenter + new Vector3(
                Random.Range(-boundarySize.x * 0.5f, boundarySize.x * 0.5f),
                0f,
                Random.Range(-boundarySize.y * 0.5f, boundarySize.y * 0.5f)
            );

            if (!IsPositionValid(position))
                continue;

            PlaceBuilding(position, parent);
            placed++;
        }

        Debug.Log($"Placed buildings: {spotCount} spots + {placed} random.");
    }

    void PlaceBuilding(Vector3 position, GameObject parent)
    {
        GameObject prefab =
            buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];

        float rotationY = Random.Range(0, 4) * 90f;
        Quaternion rotation = Quaternion.Euler(0f, rotationY, 0f);

        position.y = GetTerrainHeight(position);

        GameObject building =
            (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        building.transform.SetPositionAndRotation(position, rotation);
        building.transform.SetParent(parent.transform);
        building.isStatic = true;

        placedPositions.Add(position);
    }

    float GetTerrainHeight(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
            return position.y;

        float height = terrain.SampleHeight(position);
        return height + terrain.transform.position.y + terrainYOffset;
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (var placed in placedPositions)
        {
            if (Vector3.Distance(position, placed) < minBuildingDistance)
                return false;
        }
        return true;
    }

    bool IsInsideBoundary(Vector3 position)
    {
        return Mathf.Abs(position.x - areaCenter.x) <= boundarySize.x * 0.5f &&
               Mathf.Abs(position.z - areaCenter.z) <= boundarySize.y * 0.5f;
    }
}
