using System.Collections.Generic;
using UnityEngine;


public class PropTracktileManager : MonoBehaviour
{
    [SerializeField] private List<PropPlacement> propPlacements = new List<PropPlacement>();
    [SerializeField] private int maxPropAmount;
    private int _propAmount;

    public bool CanSpawn()
    {
        if (_propAmount >= maxPropAmount)
        {
            return false;
        }

        return true;
    }
    public void AddProp(PropScriptableObject prop, Vector3 position, Vector3 rotation)
    {
        PropPlacement propPlacement = new PropPlacement(prop, position, rotation);

        propPlacements.Add(propPlacement);
        _propAmount++;
    }

    public void RemoveProp()
    {
        propPlacements.RemoveAt(propPlacements.Count - 1);
        _propAmount--;
    }

    public List<PropPlacement> GetProp()
    {
        return propPlacements;
    }
}
