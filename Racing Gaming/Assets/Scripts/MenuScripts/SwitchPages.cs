using UnityEngine;

public class SwitchPages : MonoBehaviour, IPressButton
{
    [SerializeField] private Transform nextPage;
    [SerializeField] private Transform selectionFolder;

    [SerializeField] private Transform previousPage;
    [SerializeField] private Transform previousSelectionFolder;
    public virtual void Press(Transform player)
    {
        if (player.TryGetComponent(out MenuPlayerHandler menuPlayerHandler))
        {
            menuPlayerHandler.SetGameScreen();
        }
        MenuManager.Instance.MoveUIScreens(nextPage, selectionFolder);
        
    }
}
