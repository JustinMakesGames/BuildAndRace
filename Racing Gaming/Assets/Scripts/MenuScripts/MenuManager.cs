using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{

    public static MenuManager Instance;

    [SerializeField] private Transform canvas;

    [Header("Player Cursors")]
    [SerializeField] private List<Transform> players = new List<Transform>();
    [SerializeField] private List<Transform> playerSelectionCursors = new List<Transform>();
    [SerializeField] private Transform playerCursorFolder;
    
    [Header("Main Menu")]
    [SerializeField] private Transform menu;
    [SerializeField] private Transform menuSelectionFolder;


    [Header("Character Select Screen")]
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private GameObject characterSelectionFolder;
    [SerializeField] private List<Transform> selectedPlayers = new List<Transform>();
    [SerializeField] private List<CarStats> selectedCars = new List<CarStats>();
    private bool _isInCharacterSelectScreen;

    [Header("Build Mode Screen")]
    [SerializeField] private Transform gameModeSelectScreen;
    [SerializeField] private Transform gameModeSelectCursorFolder;

    private int _currentPlayerCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

  

    public void ShowPlayerSelectScreen()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerSelectionCursors[i].gameObject.SetActive(true);

            players[i].GetComponent<MenuPlayerHandler>().SetPlayerUI(characterSelectScreen.transform, characterSelectionFolder.transform, i, playerSelectionCursors[i]);       
        }
    }

    public void ShowPlayerOneSelectionScreen(Transform menuScreen, Transform selectFolder)
    {
        playerSelectionCursors[0].gameObject.SetActive(true);

        players[0].GetComponent<MenuPlayerHandler>().SetPlayerUI(menuScreen, selectFolder, 0, playerSelectionCursors[0]);
    }
    public void PlayerJoined(PlayerInput playerInput)
    {
        players.Add(playerInput.transform);
        
        if (playerInput.TryGetComponent(out MenuPlayerHandler playerHandler))
        {
            playerHandler.SetPlayer(_currentPlayerCount);
        }

        if (playerInput.TryGetComponent(out PlayerMotorRotationHandling motorRotationHandling))
        {
            motorRotationHandling.SetIndex(_currentPlayerCount);
        }

        if (_currentPlayerCount == 0)
        {
            MoveUIScreens(menu, menuSelectionFolder);
        }
 
        if (_isInCharacterSelectScreen)
        {
            playerSelectionCursors[_currentPlayerCount].gameObject.SetActive(true);

            playerHandler.SetPlayerUI(characterSelectScreen.transform, characterSelectionFolder.transform, _currentPlayerCount,
                playerSelectionCursors[_currentPlayerCount]);
            
        }
        _currentPlayerCount++;

    }

    public void AddPlayerSelection(Transform player, CarStats carStats)
    {
        selectedPlayers.Add(player);
        selectedCars.Add(carStats);

        if (selectedPlayers.Count >= _currentPlayerCount)
        {

            StartCoroutine(WaitForSelection());
        }
    }

    

    private IEnumerator WaitForSelection()
    {
        yield return new WaitForSeconds(1);
        MoveUIScreens(gameModeSelectScreen, gameModeSelectCursorFolder);
        CancelPlayerSelectedButton();
        
        if (TryGetComponent(out PlayerManagement playerManagement))
        {
            
        }
    }

    private void CancelPlayerSelectedButton()
    {
        foreach (var player in players)
        {
            if (player.TryGetComponent(out MenuPlayerHandler playerHandler))
            {
                playerHandler.CancelMotorSelection();
            }
        }
    }

    public void MoveUIScreens(Transform menuScreen, Transform cursorSelectFolder)
    {
        foreach (Transform screen in canvas)
        {
            bool isMenuScreen = (screen == menuScreen || screen == playerCursorFolder) ? true : false;

            screen.gameObject.SetActive(isMenuScreen);
        }

        ShowPlayerOneSelectionScreen(menuScreen, cursorSelectFolder);
    }

    public void HandleSelectionScreen()
    {
        _isInCharacterSelectScreen = true;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].TryGetComponent(out MenuPlayerHandler playerHandler))
            {
                playerHandler.SetPlayerUI(characterSelectScreen.transform, characterSelectionFolder.transform, i, playerSelectionCursors[i]);
            }
        }
    }
}
