using UnityEngine;

public class BoostPadHandler : MonoBehaviour
{
    [SerializeField] private BoostType boostType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out ArcadeCarController controller))
            {
                controller.StartBoost(boostType);
            }
        }
    }
}
