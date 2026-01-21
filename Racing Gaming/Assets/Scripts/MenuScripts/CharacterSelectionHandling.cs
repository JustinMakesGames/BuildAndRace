using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectionHandling : MonoBehaviour, IPressButton, IHandleSelection
{

    [SerializeField] private CarStats carStats;
    [SerializeField] private Slider maxSpeedSlider;
    [SerializeField] private Slider accelerationSlider;
    [SerializeField] private Slider steeringSlider;

    public void OnSelected(Transform player)
    {
        if (player.TryGetComponent(out PlayerMotorRotationHandling playerScript))
        {
            playerScript.SetMotor(carStats.motorModel);
            
        }

       
        
    }
    public void Press(Transform player)
    { 
        
        if (player.TryGetComponent(out MenuPlayerHandler menuPlayerHandler))
        {
            menuPlayerHandler.SetCarStats(carStats);
            menuPlayerHandler.CloseSelection();
            PlayerManagement.Instance.SelectPlayerCar(player.GetComponent<PlayerInput>().devices[0], carStats);
            MenuManager.Instance.AddPlayerSelection(player, carStats);
        }    
    }
}
