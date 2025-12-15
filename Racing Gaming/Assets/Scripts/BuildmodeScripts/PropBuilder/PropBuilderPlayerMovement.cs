using System.Runtime.CompilerServices;
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
    

    private void Awake()
    {
        _cam = Camera.main.transform;
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

        eventSystem.SetSelectedGameObject(firstButton);
        _isPlayerTurn = true;
    }

    public void DisablePlayerTurn()
    {
        print("Disable player turn");
        buildCanvas.SetActive(false);
        propCanvas.SetActive(false);

        _isPlayerTurn = false;
    }
    public void SetProp(Transform currentProp)
    {
        this.currentProp = currentProp;
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

                currentProp.rotation = yawRotation * surfaceRotation;
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




}
