using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class BuildPlayerManagement : MonoBehaviour
{
    public static BuildPlayerManagement Instance;

    [SerializeField] private Transform playerFolder;
    [SerializeField] private List<Transform> buildPlayerPrefabs = new List<Transform>();
    [SerializeField] private List<Transform> spawnedBuildPlayers = new List<Transform>();

    private int _currentPlayerIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void SpawnPlayers(List<InputDevice> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Transform spawnedPlayer = Instantiate(buildPlayerPrefabs[i], Vector3.zero, Quaternion.identity, playerFolder);
            spawnedBuildPlayers.Add(spawnedPlayer);

            if (spawnedBuildPlayers[i].TryGetComponent(out PlayerInput playerInput))
            {
                ConnectPlayerDevice(playerInput, players[i]);
            }
            
        }

        Transform player = spawnedBuildPlayers[_currentPlayerIndex];
        if (player.TryGetComponent(out BuildPlayerMovement buildPlayerMovement))
        {
            buildPlayerMovement.GivePlayerTurn();
            player.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    private void ConnectPlayerDevice(PlayerInput playerInput, InputDevice inputDevice)
    {
        playerInput.SwitchCurrentControlScheme(inputDevice);
    }
    public void GiveTurnToNextPlayer()
    {
        Transform oldPlayer = spawnedBuildPlayers[_currentPlayerIndex];
        GameObject oldPlayerCanvas = oldPlayer.GetChild(0).gameObject;

        _currentPlayerIndex = (_currentPlayerIndex >= spawnedBuildPlayers.Count - 1) ? 0 : _currentPlayerIndex + 1;

        print(_currentPlayerIndex);
        Transform newPlayer = spawnedBuildPlayers[_currentPlayerIndex];
        GameObject newPlayerCanvas = newPlayer.GetChild(0).gameObject;

        oldPlayerCanvas.SetActive(false);
        newPlayerCanvas.SetActive(true);

        oldPlayer.GetComponent<BuildPlayerMovement>().RemovePlayerTurn();
        newPlayer.GetComponent<BuildPlayerMovement>().GivePlayerTurn();

       

    }

    
}
