using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PropBuilderPlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform currentProp;
    [SerializeField] private GameObject buildCanvas;
    [SerializeField] private GameObject propCanvas;
    [SerializeField] private MultiplayerEventSystem eventSystem;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private LayerMask colliderMask;

    
    [Header("Handle Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform camPosition;
    private Vector2 _movement;

    [Header("Handle Tracktile")]
    private Transform _currentTracktile;

    private Transform _currentCollider;
    private Transform _cam;

    private bool _isPlayerTurn;

    [Header("Handle Rotation")]
    [SerializeField] private float rotationSpeed;
    private bool _isRotating;
    private float _yaw;

    [Header("Handle Camera Movement")]
    [SerializeField] private CinemachineOrbitalFollow cameraRotating;
    [SerializeField] private float controllerCameraSpeed;
    [SerializeField] private float mouseCameraSpeed;
    private float _cameraSpeed;
    private Vector2 _cameraInput;

    [Header("Handle Tabs")]
    [SerializeField] private Transform tabsFolder;
    [SerializeField] private MultiplayerEventSystem _eventSystem;
    [SerializeField] private float verticalOffset;
    private List<Transform> _tabs = new List<Transform>();
    private int _currentTabIndex;

    private Transform _currentTab;
    private Transform _previousTab;



    private void Awake()
    {
        _cam = Camera.main.transform;
        GameObject.FindGameObjectWithTag("CinemachineCamera").TryGetComponent(out CinemachineOrbitalFollow orbitalFollow);
        cameraRotating = orbitalFollow;

        for (int i = 0; i < tabsFolder.childCount; i++)
        {
            _tabs.Add(tabsFolder.GetChild(i));
        }
    }

    

    public void MoveProp(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();     
    }

    public void MoveNextTracktile(InputAction.CallbackContext context)
    {
        if (context.performed  && _isPlayerTurn)
        {
            PropSpawnManager.Instance.SetNextTracktile();
        }
    }

    public void MovePreviousTracktile(InputAction.CallbackContext context)
    {
        if (context.performed && _isPlayerTurn)
        {
            PropSpawnManager.Instance.SetPreviousTracktile();
        }
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

    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        cameraRotating.HorizontalAxis.Value += _cameraInput.x * _cameraSpeed * Time.deltaTime;
        cameraRotating.VerticalAxis.Value += _cameraInput.y * _cameraSpeed * Time.deltaTime;

        cameraRotating.VerticalAxis.Value = Mathf.Clamp(cameraRotating.VerticalAxis.Value,
            cameraRotating.VerticalAxis.Range.x, cameraRotating.VerticalAxis.Range.y);
    }

    public void RotateInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isRotating = true;
        }

        if (context.canceled)
        {
            _isRotating = false;
        }
    }


   
    public void SetPlayerTurn()
    {
        print("Set player turn");
        buildCanvas.SetActive(false);
        propCanvas.SetActive(true);

        HandleNewTab();
        _isPlayerTurn = true;
    }

    public void DisablePlayerTurn()
    {
        print("Disable player turn");
        buildCanvas.SetActive(false);
        propCanvas.SetActive(false);

        _isPlayerTurn = false;
    }
    public void SetProp(Transform currentProp, Transform camPosition)
    {
        this.currentProp = currentProp;
        this.camPosition = camPosition;
    }

    public void SetTracktile(Transform tracktile, Transform spawnCollider)
    {
        _currentTracktile = tracktile;
        _currentCollider = spawnCollider;
       
    }

    private void Update()
    {
        if (!_isPlayerTurn) return;
        
        HandlePropMovement();
        RotateProp();
    }

    private void HandlePropMovement()
    {
        if (!currentProp || !_currentCollider) return;

        float h = _movement.x;
        float v = _movement.y;


        Vector3 camForward = _cam.forward;
        Vector3 camRight = _cam.right;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * v + camRight * h;


        Vector3 proposedPosition = currentProp.position + move * moveSpeed * Time.deltaTime;


        Ray ray = new Ray(proposedPosition + currentProp.up, -currentProp.up);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, colliderMask))
        {

            if (hit.collider.transform == _currentCollider)
            {

                currentProp.position = hit.point;


                Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Quaternion yawRotation = Quaternion.AngleAxis(_yaw, hit.normal);
                Quaternion yawRotationCamPosition = Quaternion.AngleAxis(0, hit.normal);

                currentProp.rotation = yawRotation * surfaceRotation;

                camPosition.position = currentProp.position;
                camPosition.rotation = yawRotationCamPosition * surfaceRotation;
            }
            else
            {

                return;
            }
        }
        else
        {

            return;
        }
    }

    private void RotateProp()
    {
        if (_isRotating)
        {
            print("is rotating");
            _yaw += rotationSpeed * Time.deltaTime;
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

        if (_previousTab == null || _previousTab == _currentTab)
        {
            _previousTab = _currentTab;
            return;
        }
        _previousTab.GetComponent<TabsManagement>().ReturnGamepage().SetActive(false);
        _previousTab.position -= new Vector3(0, verticalOffset, 0);

        _previousTab = _currentTab;

    }
}
