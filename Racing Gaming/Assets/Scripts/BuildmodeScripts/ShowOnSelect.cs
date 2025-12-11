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
