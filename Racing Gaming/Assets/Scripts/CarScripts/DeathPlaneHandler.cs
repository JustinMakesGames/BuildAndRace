using System.Collections.Generic;
using UnityEngine;

public class DeathPlaneHandler : MonoBehaviour
{
    private Transform _respawnScriptTransform;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.TryGetComponent(out DeathplanePlayerHandler deathPlayerHandler)) return;
            if (!IsDeathPlane(deathPlayerHandler)) return;
            
            if (other.TryGetComponent(out Rigidbody rb))
            {
                rb.position = _respawnScriptTransform.position;
                rb.rotation = _respawnScriptTransform.rotation;
                rb.linearVelocity = Vector3.zero;
            }
            
        }
    }

    private bool IsDeathPlane(DeathplanePlayerHandler player)
    {
        List<Transform> deathPlanes = player.ReturnDeathPlanes();

        for (int i = 0; i < deathPlanes.Count; i++)
        {
            if (deathPlanes[i] == transform)
            {
                return true;
            }
        }

        return false;
    }
    public void SetRespawnScriptTransform(Transform respawnScriptTransform)
    {
        _respawnScriptTransform = respawnScriptTransform;
    }
}
