using UnityEngine;

public class SpikeballObjectHandling : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out ArcadeCarController carController))
            {
                carController.HandleMotorHit();
            }

            Destroy(gameObject);
        }
    }
}
