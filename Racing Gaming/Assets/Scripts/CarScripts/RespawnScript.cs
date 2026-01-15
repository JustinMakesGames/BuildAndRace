using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] private Transform beginRespawnPosition;
    private Transform _respawnPosition;
    private bool _canRespawn;
    public void RespawnInput(InputAction.CallbackContext context)
    {
        if (context.started && _canRespawn)
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
        

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.position = _respawnPosition.position;
            rb.rotation = _respawnPosition.rotation;
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

    public void SetRespawn()
    {
        _canRespawn = true;
    }

    public void SetRespawnOff()
    {
        _canRespawn = false;
    }

    public bool CanRespawn()
    {
        return _canRespawn;
    }
}
