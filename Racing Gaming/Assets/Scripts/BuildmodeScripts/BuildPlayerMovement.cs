using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class BuildPlayerMovement : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow cameraRotating;
    [SerializeField] private float scrollSpeed;

    [SerializeField] private float _scrollInput;
   

    public void Scrolling(InputAction.CallbackContext context)
    {
        _scrollInput = context.ReadValue<float>();
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BuildManager.Instance.Rotate();
        }
    }

    public void DeleteInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BuildManager.Instance.RemovePiece();
        }
    }

  

    
}
