using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayerHandler : MonoBehaviour
{
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
            CancelHandling();
        }
    }
    private void SelectButton()
    {
        if (_selectedButton == null) return;
        _selectedButton.Press(transform);
    }

    public void CancelHandling()
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
    } 
    public void SetPlayerUI(Transform selectionFolder, int playerIndex, Transform playerCursor)
    {
        this.selectionFolder = selectionFolder;
        this.playerIndex = playerIndex;
        this.playerCursor = playerCursor;
        if (playerCursor.TryGetComponent(out Animator animator)) selectionAnimator = animator;

        _selections.Clear();
        for (int i = 0; i <  selectionFolder.childCount; i++)
        {
            _selections.Add(selectionFolder.GetChild(i));
        }

        SetPosition();
        StartCoroutine(MakeCanSelectFrameLater());
    }

    private IEnumerator MakeCanSelectFrameLater()
    {
        yield return null;
        _canSelect = true;
    }

    private void SetPosition()
    {
        playerCursor.position = _selections[_currentLocationIndex].GetChild(playerIndex).position;
        _selectedButton = _selections[_currentLocationIndex].GetComponent<IPressButton>();
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
