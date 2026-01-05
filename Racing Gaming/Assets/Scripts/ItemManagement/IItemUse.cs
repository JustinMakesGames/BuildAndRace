using UnityEngine;

public interface IItemUse
{
    public int UseAmount { get; set; }
    public void UseItem(Transform motor);
}
