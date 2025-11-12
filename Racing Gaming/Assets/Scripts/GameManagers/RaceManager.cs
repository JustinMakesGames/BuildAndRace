using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{

    public static RaceManager Instance;
    [SerializeField] private int maxLapCount;
    [SerializeField] private List<Transform> finishedCars = new List<Transform>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public int GetMaxLapCount()
    {
        return maxLapCount;
    }

    public void FinishPlayer(Transform car)
    {
        finishedCars.Add(car);

        print($"{car.name} is number {finishedCars.IndexOf(car) + 1}");
    }
}
