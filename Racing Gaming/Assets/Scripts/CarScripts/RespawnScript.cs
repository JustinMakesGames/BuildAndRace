using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnScript : MonoBehaviour
{

    private Transform _respawnPosition;
    public void RespawnInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (!_respawnPosition) return;
        transform.position = _respawnPosition.position;
        transform.rotation = _respawnPosition.rotation;
    }

    public void SetRespawnPosition(Transform respawnPosition)
    {
        _respawnPosition = respawnPosition;
    }
}
