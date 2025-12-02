using UnityEngine;

public class CharacterSelectionHandling : MonoBehaviour, IPressButton
{

    [SerializeField] private CarStats carStats;
    public void Press(Transform player)
    { 
        
        if (player.TryGetComponent(out MenuPlayerHandler menuPlayerHandler))
        {
            menuPlayerHandler.SetCarStats(carStats);
            menuPlayerHandler.CloseSelection();
            MenuManager.Instance.AddPlayerSelection(player);
        }

        
        
    }
}
