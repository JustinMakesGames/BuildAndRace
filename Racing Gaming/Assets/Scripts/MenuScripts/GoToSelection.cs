using UnityEngine;

public class GoToSelection : SwitchPages, IPressButton
{
    public override void Press(Transform player)
    {
        base.Press(player);

        MenuManager.Instance.HandleSelectionScreen();
        MenuManager.Instance.ShowPlayerSelectScreen();
    }
}
