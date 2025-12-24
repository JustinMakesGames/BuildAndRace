using UnityEngine;
using UnityEngine.EventSystems;

public class PropFinishButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] private PropScriptableObject propScriptableObject;

    public void OnSelect(BaseEventData eventData)
    {
        PropSpawnManager.Instance.SetProp(propScriptableObject.propGameObject);
        PropSpawnManager.Instance.SetPropBuildMode();
    }

    public void FinishProps()
    {
        FinishHandling.Instance.SetProps();
        PropSpawnManager.Instance.EndPlayerTurns();
        SaveBuilderHandler.Instance.OpenCanvas();
    }
}
