using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private void Start()
    {
        cameraRotating = GameObject.FindGameObjectWithTag("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();
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
        cameraRotating.VerticalAxis.Value += _cameraInput.y * _cameraSpeed * Time.deltaTime;

        cameraRotating.VerticalAxis.Value = Mathf.Clamp(cameraRotating.VerticalAxis.Value,
            cameraRotating.VerticalAxis.Range.x, cameraRotating.VerticalAxis.Range.y);
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
