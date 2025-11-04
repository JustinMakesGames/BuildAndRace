using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ArcadeCarController))]
public class PlayerCarController : MonoBehaviour
{
    private ArcadeCarController _carController;
    private float _accelerationInput;
    private float _steeringInput;
    private void Start()
    {
        _carController = GetComponent<ArcadeCarController>();
    }


    public void AccelerationInput(InputAction.CallbackContext context)
    {
        _accelerationInput = context.ReadValue<float>();
    }

    public void SteeringInput(InputAction.CallbackContext context)
    {
        _steeringInput = context.ReadValue<float>();
    }

    public void DriftingInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _carController.StartDriftInput();
        }

        if (context.canceled)
        {
            _carController.CancelDriftInput();
        }
    }

    private void Update()
    {
        _carController.SetInput(_accelerationInput, _steeringInput);
    }
}
