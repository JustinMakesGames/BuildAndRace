using System.Collections.Generic;
using UnityEngine;

public class LevelSelectHandling : SceneSwitch, IPressButton
{

    [SerializeField] private List<Transform> levels = new List<Transform>();
    public override void Press(Transform player)
    {
        Traverse(transform.parent.parent);
        int index = levels.IndexOf(transform);

        LevelName.Instance.PlayLevel(index);
        base.Press(player);
    }

    private void Traverse(Transform parent)
    {

        
        foreach (Transform child in parent)
        {
            foreach (Transform grandChild in child)
            {
                levels.Add(grandChild);
            }
        }
    }
}
