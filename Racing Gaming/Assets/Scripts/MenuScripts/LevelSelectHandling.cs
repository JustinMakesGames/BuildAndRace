using UnityEngine;

public class LevelSelectHandling : SceneSwitch, IPressButton
{
    public override void Press(Transform player)
    {
        int index = transform.GetSiblingIndex();

        LevelName.Instance.PlayLevel(index);
        base.Press(player);
    }
}
