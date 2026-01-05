using UnityEngine;

public class BoostItem : MonoBehaviour, IItemUse
{

    public int UseAmount { get; set; } = 1;
    [SerializeField] private BoostType boostType;
    public void UseItem(Transform car)
    {
        car.GetComponent<ArcadeCarController>().StartBoost(boostType);
    }
}
