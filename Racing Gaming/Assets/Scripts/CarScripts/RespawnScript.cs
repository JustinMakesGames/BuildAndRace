using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] private Transform beginRespawnPosition;
    private Transform _respawnPosition;
    public void RespawnInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Respawn();
        }
    }

    private void Start()
    {
        _respawnPosition = beginRespawnPosition;
    }

    public void Respawn()
    {
        if (!_respawnPosition) return;
        transform.position = _respawnPosition.position;
        transform.rotation = _respawnPosition.rotation;

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (TryGetComponent(out ArcadeCarController controller))
        {
            controller.SetGravity(-_respawnPosition.up);
        }
    }

    public void SetRespawnPosition(Transform respawnPosition)
    {
        _respawnPosition = respawnPosition;
    }
}
