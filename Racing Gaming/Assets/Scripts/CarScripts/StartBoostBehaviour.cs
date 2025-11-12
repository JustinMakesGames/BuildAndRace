using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartBoostBehaviour : MonoBehaviour
{

    public BoostType boostType;
    [SerializeField] private Slider boostSlider;
    [SerializeField] private float sliderMultiplier;
    [SerializeField] private RectTransform greenPanel;
    private bool _canMoveSlider;
    private bool _isMovingSlider;

    public void MoveSlider(InputAction.CallbackContext context)
    {
        if (!_canMoveSlider) return;
        if (context.started)
        {
            _isMovingSlider = true;
        }

        if (context.canceled)
        {
            _isMovingSlider = false;
        }
    }
    public void ActivateBoostSlider()
    {
        if (!boostSlider) return;
        boostSlider.gameObject.SetActive(true);
        _canMoveSlider = true;
    }

    private void Update()
    {
        if (_canMoveSlider)
        {
            if (_isMovingSlider)
            {
                print("yay workie");
                boostSlider.value += sliderMultiplier * Time.deltaTime;
            }

            else
            {
                boostSlider.value -= sliderMultiplier * Time.deltaTime;
            }
        }
    }

    public void GiveBoost()
    {

        if (!boostSlider) return;
        RectTransform rectTransform = boostSlider.transform.GetComponent<RectTransform>();

        float width = rectTransform.rect.width;
        float minimumBoostValue = greenPanel.anchoredPosition.x - greenPanel.rect.width / 2;
        float maximumBoostValue = greenPanel.anchoredPosition.x + greenPanel.rect.width / 2;
        float sliderMinimumBoost = minimumBoostValue / width;
        float sliderMaximumBoost = maximumBoostValue / width;

        if (boostSlider.value >= sliderMinimumBoost && boostSlider.value <= sliderMaximumBoost)
        {
            print("WOOOSHHHHHH");
            
            if (TryGetComponent(out ArcadeCarController carController))
            {
                carController.StartBoost(boostType);
            }

        }

        else
        {
            print("NO I DO NOT HAVE THE BOOST WAAAAA");
        }

        boostSlider.gameObject.SetActive(false);
    }
}
