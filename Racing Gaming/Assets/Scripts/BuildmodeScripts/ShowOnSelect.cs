using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Image _image;

    private void Awake()
    {
        if (TryGetComponent(out Image image))
        {
            _image = image;
        }

        if (TryGetComponent(out Button button)) 
        {
            button.onClick.AddListener(BuildManager.Instance.GetSpawnNewTileInput);
            button.onClick.AddListener(BuildPlayerManagement.Instance.GiveTurnToNextPlayer);
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        _image.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _image.enabled = false;
    }
}
