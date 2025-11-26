using System.Collections.Generic;
using UnityEngine;

public class DeathplanePlayerHandler : MonoBehaviour
{
    public List<Transform> deathPlanes = new List<Transform>();
    public Transform respawnPosition;

    public List<Transform> ReturnDeathPlanes()
    {
        return deathPlanes;
    }

    public void SetDeathPlane(List<Transform> deathPlanes)
    {
        this.deathPlanes = deathPlanes;
    }
}
