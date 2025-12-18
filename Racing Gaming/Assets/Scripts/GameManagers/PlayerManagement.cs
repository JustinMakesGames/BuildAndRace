using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement Instance;

    [SerializeField] private List<InputDevice> players = new List<InputDevice>();
    [SerializeField] private List<CarStats> carStatsList = new List<CarStats>();

    //Build Mode Management
    [SerializeField] private string buildScene;

    //Race Scene Management
    [SerializeField] private string raceScene;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    

    public void SelectPlayerCar(InputDevice player, CarStats carStats)
    {
        players.Add(player);
        carStatsList.Add(carStats);
    }

    public void RemovePlayerCar(int index)
    {
        players.RemoveAt(index);
        carStatsList.RemoveAt(index);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode) 
    {
        if (scene.name == buildScene)
        {
            BuildPlayerManagement.Instance.SpawnPlayers(players);
        }

        else if (scene.name == raceScene)
        {
            PlayerCarManagement.Instance.SetPlayers(players, carStatsList);
        }
    }



}
