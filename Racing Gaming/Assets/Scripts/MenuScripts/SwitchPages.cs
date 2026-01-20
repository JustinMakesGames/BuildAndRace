using UnityEngine;

public class SwitchPages : MonoBehaviour, IPressButton
{
    [SerializeField] protected Transform nextPage;
    [SerializeField] protected Transform selectionFolder;

    
    public virtual void Press(Transform player)
    {
        if (player.TryGetComponent(out MenuPlayerHandler menuPlayerHandler))
        {
            menuPlayerHandler.SetGameScreen();
        }
        MenuManager.Instance.MoveUIScreens(nextPage, selectionFolder);
        
    }
}
