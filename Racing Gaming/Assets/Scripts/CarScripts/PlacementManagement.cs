using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CarPlacementData
{
    public Transform car;
    public int lapCount;
    public int currentIndex;
    public float currentDistance;

    public CarPlacementData(Transform car, int lapCount, int currentIndex, float currentDistance)
    {
        this.car = car;
        this.lapCount = lapCount;
        this.currentIndex = currentIndex;
        this.currentDistance = currentDistance;
    } 
}
public class PlacementManagement : MonoBehaviour
{
    [SerializeField] private Transform carFolder;
    [SerializeField] private List<Transform> cars = new List<Transform>();

    [SerializeField] private Transform waypointFolder;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    private void Awake()
    {
        for (int i = 0; i < carFolder.childCount; i++)
        {
            cars.Add(carFolder.GetChild(i));
        }
    }

    private void Start()
    {
        Ticker.Instance.onTick += HandlePlacements;
    }

    public void SetPlacement(List<Transform> waypoints)
    {
        this.waypoints = waypoints;

        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].TryGetComponent(out PlayerPlacementHandler placementHandler))
            {
                placementHandler.SetList(waypoints);
            }

            if (cars[i].TryGetComponent(out AICarController carController))
            {
                carController.SetWaypoints();
            }
        }

    }

    private void HandlePlacements()
    {
        List<CarPlacementData> carPlacementDataList = new List<CarPlacementData>();
        carPlacementDataList = ReturnCarPlacementDataList(carPlacementDataList);
        carPlacementDataList = SortThePlacementList(carPlacementDataList);
        SendPlacementsToPlayers(carPlacementDataList);   
    }

    private List<CarPlacementData> ReturnCarPlacementDataList(List<CarPlacementData> carPlacementDataList)
    {
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].TryGetComponent(out PlayerPlacementHandler handler))
            {
                CarPlacementData newCarPlacementData = new CarPlacementData(cars[i], handler.ReturnLapCount(), handler.ReturnWayPointIndex(), handler.ReturnDistance());
                carPlacementDataList.Add(newCarPlacementData);
            }

        }

        return carPlacementDataList;
    }

    private List<CarPlacementData> SortThePlacementList(List<CarPlacementData> carPlacementDataList)
    {
        carPlacementDataList = carPlacementDataList.OrderByDescending(c => c.lapCount)
            .ThenByDescending(c => c.currentIndex == 0 ? int.MaxValue : c.currentIndex)
            .ThenBy(c => c.currentDistance)
            .ToList();

        return carPlacementDataList;
    }

    private void SendPlacementsToPlayers(List<CarPlacementData> carPlacementDataList)
    {
        for (int i = 0; i < carPlacementDataList.Count; i++)
        {
            
            if (carPlacementDataList[i].car.TryGetComponent(out PlayerPlacementHandler handler))
            {
                handler.SetPlacement(i + 1);
            }
        }
    } 
}

