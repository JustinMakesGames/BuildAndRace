using System.Collections.Generic;
using UnityEngine;

public class SetRespawnScript : MonoBehaviour
{
    [SerializeField] private List<Transform> deathPlanes = new List<Transform>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetDeathPlanes();
            
            other.GetComponent<DeathplanePlayerHandler>().SetDeathPlane(deathPlanes);
            other.GetComponent<RespawnScript>().SetRespawnPosition(transform);
        }
    }

    private void SetDeathPlanes()
    {
        foreach (Transform deathPlane in deathPlanes)
        {
            deathPlane.GetComponent<DeathPlaneHandler>().SetRespawnScriptTransform(transform);

        }
    }
}
