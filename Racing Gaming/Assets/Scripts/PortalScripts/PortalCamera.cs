using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform portal;
    [SerializeField] private Transform otherPortal;

    public void SetOtherPortal(Transform newPortal)
    {
        otherPortal = newPortal;
    }

    private void Update()
    {
        if (!otherPortal) return;
        // Get player pos in otherPortal local space
        Vector3 localPos = otherPortal.InverseTransformPoint(playerCamera.position);

        // Two candidate positions:
        // A = direct (no mirror)
        Vector3 worldPosA = portal.TransformPoint(localPos);

        // B = mirrored forward (z) — common portal convention
        Vector3 localPosMirrored = new Vector3(localPos.x, localPos.y, -localPos.z);
        Vector3 worldPosB = portal.TransformPoint(localPosMirrored);

        // --- ROTATION candidates ---
        // Put player rotation into otherPortal local space
        Quaternion localRot = Quaternion.Inverse(otherPortal.rotation) * playerCamera.rotation;

        // Candidate A: do NOT apply 180 flip in local space
        Quaternion rotA = portal.rotation * localRot;

        // Candidate B: apply 180° flip in otherPortal-local up (this flips forward)
        // Using Vector3.up here is correct because localRot is in otherPortal-local space.
        Quaternion rotB = portal.rotation * (Quaternion.AngleAxis(180f, Vector3.up) * localRot);

        // Evaluate which candidate faces the *outside* of the exit portal.
        // We expect after mapping the camera's forward should align with portal.forward (i.e. look out of the portal)
        Vector3 fA = rotA * Vector3.forward;
        Vector3 fB = rotB * Vector3.forward;

        float dotA = Vector3.Dot(fA.normalized, portal.forward.normalized);
        float dotB = Vector3.Dot(fB.normalized, portal.forward.normalized);

        // Choose the candidate that has a larger dot with portal.forward (i.e. faces the same side as the portal)
        if (dotB > dotA)
        {
            // Use the mirrored version (B)
            transform.position = worldPosB;
            transform.rotation = rotB;
        }
        else
        {
            // Use the non-mirrored version (A)
            transform.position = worldPosA;
            transform.rotation = rotA;

        }
    }
}
