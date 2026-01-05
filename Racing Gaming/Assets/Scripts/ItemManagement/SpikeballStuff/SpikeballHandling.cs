using UnityEngine;

public class SpikeballHandling : MonoBehaviour, IItemUse
{

    public int UseAmount { get; set; } = 1;

    [SerializeField] private GameObject spikeball;
    public void UseItem(Transform car)
    {
        SpawnSpikeBall(car);
    }

    private void SpawnSpikeBall(Transform car)
    {
        Vector3 spawnPosition = car.position + -car.forward * 3f;
        Instantiate(spikeball, spawnPosition, car.rotation);
    }
}
