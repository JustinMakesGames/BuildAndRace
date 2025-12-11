using UnityEngine;
using UnityEngine.EventSystems;
public class PropButtonHandler : MonoBehaviour, ISelectHandler
{

    [SerializeField] private PropScriptableObject propScriptableObject;

    public void OnSelect(BaseEventData eventData)
    {
        PropSpawnManager.Instance.SetProp(propScriptableObject.propGameObject);
        PropSpawnManager.Instance.SetPropBuildMode();
    }

    public void SpawnObject()
    {
        PropSpawnManager.Instance.SpawnProp();
    }

}
