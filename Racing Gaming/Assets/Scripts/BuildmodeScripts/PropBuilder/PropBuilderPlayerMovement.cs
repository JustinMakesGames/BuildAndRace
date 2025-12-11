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
        if (context.started)
        {
            PropSpawnManager.Instance.SetNextTracktile();
        }
    }

    public void MovePreviousTracktile(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PropSpawnManager.Instance.SetPreviousTracktile();
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


                currentProp.up = hit.normal;
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




}
