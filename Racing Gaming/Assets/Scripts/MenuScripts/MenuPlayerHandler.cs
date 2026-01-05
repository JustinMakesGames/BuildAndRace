using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPlayerHandler : MonoBehaviour
{
    //Sliders on character selectscreen
    public Slider maxSpeedSlider;
    public Slider accelerationSlider;
    public Slider handlingSlider;

    [SerializeField] private Transform menuScreen;
    [SerializeField] private Transform selectionFolder;
    [SerializeField] private int playerIndex;
    [SerializeField] private Transform playerCursor;
    [SerializeField] private CarStats carStats;
    [SerializeField] private Animator selectionAnimator;
    

    private IPressButton _selectedButton;
    private List<Transform> _selections = new List<Transform>();

    private int _currentLocationIndex;
    private float _movement;

    private bool _hasSelected;
    private bool _canSelect;

    private bool _isPlayerOne;

    private bool _isOnMainScreen;

    private List<Transform> _previousGameScreens = new List<Transform>();
    private List<Transform> _previousCursorFolders = new List<Transform>();
    

    public void MoveInput(InputAction.CallbackContext context)
    {
        if (context.started && !_hasSelected && _canSelect)
        {
            _movement = context.ReadValue<Vector2>().x;
            MoveCursor();
        }
    }

    public void SelectInput(InputAction.CallbackContext context)
    {
        if (context.started && !_hasSelected && _canSelect)
        {
            SelectButton();
        }
    }

    public void CancelInput(InputAction.CallbackContext context)
    {
        if (context.started && _hasSelected && _canSelect)
        {
            CancelMotorSelection();
        }

        else if (context.started)
        {
            CancelScreen();
        }


    }

    //Sets the last previous canvas here, that's why i am doing .count - 1
    private void CancelScreen()
    {
        if (_previousGameScreens.Count > 0)
        {
            var previousScreen = _previousGameScreens[_previousGameScreens.Count - 1];
            var cursorFolder = _previousCursorFolders[_previousCursorFolders.Count - 1];
            MenuManager.Instance.MoveUIScreens(previousScreen, cursorFolder);
            SetPlayerUI(previousScreen, cursorFolder, 0, playerCursor);
            _previousGameScreens.Remove(previousScreen);
            _previousCursorFolders.Remove(cursorFolder);
        }
    }

    public void SetGameScreen()
    {
        _previousGameScreens.Add(menuScreen);
        _previousCursorFolders.Add(selectionFolder);

    }
    private void SelectButton()
    {
        if (_selectedButton == null) return;
        _selectedButton.Press(transform);
    }

    public void CancelMotorSelection()
    {
        _hasSelected = false;
        selectionAnimator.SetTrigger("PlayAnimation");
    }
    private void MoveCursor()
    {
        if (_movement == 0) return;
        int cursorMovement = (int)Mathf.Sign(_movement);

        
        if (cursorMovement > 0)
        {
            if (_currentLocationIndex + 1 >= selectionFolder.childCount) return;

            _currentLocationIndex++;
        }

        else
        {
            if (_currentLocationIndex - 1 < 0) return;
            _currentLocationIndex--;
        }

        SetPosition();
    }

    public void SetPlayer(int playerIndex)
    {
        this.playerIndex = playerIndex;

        if (playerIndex == 0)
        {
            _isPlayerOne = true;
        }

        maxSpeedSlider = SelectStatsSelectFolder.Instance.SetMaxSpeedSlider(playerIndex);
        accelerationSlider = SelectStatsSelectFolder.Instance.SetAccelerationSlider(playerIndex);
        handlingSlider = SelectStatsSelectFolder.Instance.SetHandlingSlider(playerIndex);
    } 
    public void SetPlayerUI(Transform menuScreen, Transform selectionFolder, int playerIndex, Transform playerCursor)
    {
        this.menuScreen = menuScreen;
        this.selectionFolder = selectionFolder;
        this.playerIndex = playerIndex;
        this.playerCursor = playerCursor;
        if (playerCursor.TryGetComponent(out Animator animator)) selectionAnimator = animator;

        _selections.Clear();
        for (int i = 0; i <  selectionFolder.childCount; i++)
        {
            _selections.Add(selectionFolder.GetChild(i));
        }

        _currentLocationIndex = 0;
        SetPosition();
        StartCoroutine(MakeCanSelectFrameLater());
    }

    private IEnumerator MakeCanSelectFrameLater()
    {
        yield return null;
        _canSelect = true;
    }

    public void SetSliders(int maxSpeed, int acceleration, int handling)
    {
        maxSpeedSlider.value = maxSpeed;
        accelerationSlider.value = acceleration;
        handlingSlider.value = handling;
    }

    private void SetPosition()
    {
        playerCursor.position = _selections[_currentLocationIndex].GetChild(playerIndex).position;
        _selectedButton = _selections[_currentLocationIndex].GetComponent<IPressButton>();

        if (_selections[_currentLocationIndex].TryGetComponent(out IHandleSelection handleSelection))
        {
            handleSelection.OnSelected(transform);
        }
    }

    public void SetCarStats(CarStats stats)
    {
        carStats = stats;
    }

    public void CloseSelection()
    {
        _hasSelected = true;
        selectionAnimator.SetTrigger("StopAnimation");
    }   
}
