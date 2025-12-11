using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropInGameSpawner : MonoBehaviour
{
    public static PropInGameSpawner Instance;

    [SerializeField] private List<PropScriptableObject> allProps = new List<PropScriptableObject>();
    [SerializeField] private List<Vector3> positions = new List<Vector3>();

    private List<GameObject> tracktiles = new List<GameObject>();
    private List<int> _propIndexes = new List<int>();
    private List<int> _propAmountPerTracktile = new List<int>();
    private int _currentPropPerTracktile;
    private int _currentTracktile;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
    }
    public void LoadProps(List<GameObject> tracktiles)
    {
        BuildData newData = BuildSaveSystem.LoadBuild();
        this.tracktiles = tracktiles;
        _propIndexes = newData.propIndexes.ToList();
        _propAmountPerTracktile = newData.tracktilePropAmount.ToList();
        
        for (int i = 0; i < newData.positionsX.Length; i++)
        {
            positions.Add(new Vector3(newData.positionsX[i], newData.positionsY[i], newData.positionsZ[i]));
        }

        SpawnTheProps();

        
    }

    private void SpawnTheProps()
    {
        for (int i = 0; i < _propIndexes.Count; i++)
        {
            for (int j = 0; j < allProps.Count; j++)
            {
                if (_propIndexes[i] == allProps[j].index)
                {
                    SetTracktile();
                    SpawnProp(allProps[j], tracktiles[_currentTracktile], i);
                    break;
                }
             }
        }
    }

    private void SetTracktile()
    {    
        if (_currentPropPerTracktile >= _propAmountPerTracktile[_currentTracktile])
        {
            _currentTracktile++;
            _currentPropPerTracktile = 0;

        }
    }
    private void SpawnProp(PropScriptableObject propScriptableObject, GameObject tracktile, int index)
    {
        Transform prop = Instantiate(propScriptableObject.propGameObject, Vector3.zero, Quaternion.identity, tracktile.transform).transform;
        prop.localPosition = positions[index];
        _currentPropPerTracktile++;
    }
}
