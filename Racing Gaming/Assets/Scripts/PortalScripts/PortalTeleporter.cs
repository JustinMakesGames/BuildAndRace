using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private Transform receiver;  // The paired portal
    [SerializeField] private Transform portalRenderer;

    // Track which players have already teleported so we don't double teleport
    private HashSet<Transform> teleportedPlayers = new HashSet<Transform>();

    private void Awake()
    {
        receiver = GameObject.FindGameObjectWithTag("Receiver").transform;
        GameObject.FindGameObjectWithTag("PortalCamera").GetComponent<PortalCamera>().SetOtherPortal(portalRenderer);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !teleportedPlayers.Contains(other.transform))
        {
            TeleportPlayer(other.transform);
            teleportedPlayers.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset teleport state when leaving so player can teleport again
        if (other.CompareTag("Player") && teleportedPlayers.Contains(other.transform))
        {
            teleportedPlayers.Remove(other.transform);
        }
    }

    private void TeleportPlayer(Transform player)
    {
        
        CinemachineFollow camera = player.GetComponent<ArcadeCarController>().cameraFollow;


        Vector3 originalSettings = Vector3.zero;
        if (camera != null)
        {
            originalSettings = camera.TrackerSettings.RotationDamping;
            camera.TrackerSettings.RotationDamping = Vector3.zero;
        }
        
        
        Vector3 localPos = transform.InverseTransformPoint(player.position);
        Vector3 newWorldPos = receiver.TransformPoint(localPos);

        // --- YAW PRESERVATION (robust, avoids Euler gimballing) ---
        // Get player's forward in entry-portal local space
        Vector3 localForward = transform.InverseTransformDirection(player.forward);

        // Compute local yaw angle (degrees) from that forward vector (X,Z plane)
        float localYaw = Mathf.Atan2(localForward.x, localForward.z) * Mathf.Rad2Deg;

        // Build new rotation: start with receiver rotation (includes its X/Z tilt), then apply the player's local yaw
        Quaternion newRotation = receiver.rotation * Quaternion.Euler(0f, localYaw, 0f);

        player.position = newWorldPos;
        player.rotation = newRotation;

        // --- PRESERVE VELOCITY IF RIGIDBODY ---
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // rotate velocity by the full portal-to-portal rotation
            Quaternion rotOffset = receiver.rotation * Quaternion.Inverse(transform.rotation);
            rb.linearVelocity = rotOffset * rb.linearVelocity;
            rb.angularVelocity = rotOffset * rb.angularVelocity;
        }

        if (camera != null)
        {
            StartCoroutine(PutCameraToNormal(camera, originalSettings));

        }

        player.GetComponent<AICarController>().ResetWaypoint();
        player.GetComponent<PlayerPlacementHandler>().ResetLap();
    }

    private IEnumerator PutCameraToNormal(CinemachineFollow camera, Vector3 originalSettings)
    {
        yield return null;
        camera.TrackerSettings.RotationDamping = originalSettings;
    }
}
