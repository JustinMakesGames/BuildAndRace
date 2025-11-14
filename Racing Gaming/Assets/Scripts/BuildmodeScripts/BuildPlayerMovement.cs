using UnityEngine;
using UnityEngine.InputSystem;

public class BuildPlayerMovement : MonoBehaviour
{
    public void CameraRotate(InputAction.CallbackContext context)
    {
        print(context.ReadValue<Vector2>());
    }
}
