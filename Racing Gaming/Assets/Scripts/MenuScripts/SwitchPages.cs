using UnityEngine;

public class SwitchPages : MonoBehaviour, IPressButton
{
    [SerializeField] private Transform nextPage;
    [SerializeField] private Transform selectionFolder;

    
    public virtual void Press(Transform player)
    {
        if (player.TryGetComponent(out MenuPlayerHandler menuPlayerHandler))
        {
            menuPlayerHandler.SetGameScreen();
        }
        MenuManager.Instance.MoveUIScreens(nextPage, selectionFolder);
        
    }
}
