using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectionHandling : MonoBehaviour, IPressButton, IHandleSelection
{

    [SerializeField] private CarStats carStats;

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
