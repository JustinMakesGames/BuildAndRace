using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManagement : MonoBehaviour
{
    [SerializeField] private List<InputDevice> players = new List<InputDevice>();
    [SerializeField] private Dictionary<Transform, CarStats> playerCars = new Dictionary<Transform, CarStats>(); //Player with car

    //Build Mode Management
    [SerializeField] private string buildScene;

    //Race Scene Management
    [SerializeField] private string raceScene;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    public void PlayerJoined(PlayerInput playerInput)
    {
        if (SceneManager.GetActiveScene().name == buildScene) return;
        players.Add(playerInput.user.pairedDevices[0]);
 
    }

    public void SelectPlayerCar(Transform player, CarStats carStats)
    {
        playerCars.Add(player, carStats);
    }

    public void RemovePlayerCar(Transform player)
    {
        playerCars.Remove(player);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode) 
    {
        if (scene.name == buildScene)
        {
            BuildPlayerManagement.Instance.SpawnPlayers(players);
        }

        else if (scene.name == raceScene)
        {
            PlayerCarManagement.Instance.SetPlayers(players, carStats);
        }
    }



}
