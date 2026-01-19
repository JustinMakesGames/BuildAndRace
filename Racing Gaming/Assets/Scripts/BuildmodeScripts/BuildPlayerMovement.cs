using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BuildPlayerMovement : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow cameraRotating;

    [SerializeField] private float controllerCameraSpeed;
    [SerializeField] private float mouseCameraSpeed;
    private float _cameraSpeed;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float _scrollInput;

    private Vector2 _cameraInput;

    private bool _isPlayerTurn;

    //Handle Tabs
    [SerializeField] private Transform tabsFolder;
    [SerializeField] private MultiplayerEventSystem _eventSystem;
    [SerializeField] private float verticalOffset;
    private List<Transform> _tabs = new List<Transform>();
    private int _currentTabIndex;

    private Transform _currentTab;
    private Transform _previousTab;

    private void Awake()
    {
        for (int i = 0; i < tabsFolder.childCount; i++)
        {
            _tabs.Add(tabsFolder.GetChild(i));
        }

        
    }

    private void Start()
    {
        cameraRotating = GameObject.FindGameObjectWithTag("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();
        HandleNewTab();
    }
    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    
    public void RotateCamera(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        _cameraInput = context.ReadValue<Vector2>();

        InputDevice device = context.control.device;

        if (device is Gamepad)
        {
            _cameraSpeed = controllerCameraSpeed;
        }

        else
        {
            _cameraSpeed = mouseCameraSpeed;
        }

        
    }


    private void HandleCameraRotation()
    {
        cameraRotating.HorizontalAxis.Value += _cameraInput.x * _cameraSpeed * Time.deltaTime;
        cameraRotating.VerticalAxis.Value -= _cameraInput.y * _cameraSpeed * Time.deltaTime;

        cameraRotating.VerticalAxis.Value = Mathf.Clamp(cameraRotating.VerticalAxis.Value,
            cameraRotating.VerticalAxis.Range.x, cameraRotating.VerticalAxis.Range.y);

        cameraRotating.Radius -= _scrollInput * scrollSpeed;
    }
    public void Scrolling(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        _scrollInput = context.ReadValue<float>();
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        if (context.started)
        {
            BuildManager.Instance.Rotate();
        }
    }

    public void DeleteInput(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        if (context.started)
        {
            BuildManager.Instance.RemovePiece();
        }
    }

    public void SwitchNextTab(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        if (context.started && _currentTabIndex + 1 < _tabs.Count)
        {
            _currentTabIndex++;
            HandleNewTab();
        }
    }

    public void SwitchPreviousTab(InputAction.CallbackContext context)
    {
        if (!_isPlayerTurn) return;
        if (context.started && _currentTabIndex - 1 >= 0)
        {
            _currentTabIndex--;
            HandleNewTab();
        }
    }

    private void HandleNewTab()
    {
        _currentTab = _tabs[_currentTabIndex];
        var tabScript = _currentTab.GetComponent<TabsManagement>();

        GameObject gamePage = tabScript.ReturnGamepage();
        GameObject selectedButton = tabScript.ReturnSelectedButton();
        gamePage.SetActive(true);
        _eventSystem.SetSelectedGameObject(selectedButton);

        _currentTab.position += new Vector3(0, verticalOffset, 0);

        if (_previousTab == null)
        {
            _previousTab = _currentTab;
            return;
        }
        _previousTab.GetComponent<TabsManagement>().ReturnGamepage().SetActive(false);
        _previousTab.position -= new Vector3(0, verticalOffset, 0);

        _previousTab = _currentTab;
        
    }

    public void GivePlayerTurn()
    {
        _isPlayerTurn = true;
    }

    public void RemovePlayerTurn()
    {
        _isPlayerTurn = false;
        _cameraInput = Vector2.zero;
    }

  

    
}
