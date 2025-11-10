using UnityEngine;

public class BonkHandler : MonoBehaviour
{
    [Header("Collision Bonk")]
    [SerializeField] private LayerMask bonkLayerMask = ~0; // which layers to bonk (default: everything)
    [SerializeField] private float bonkForce = 8f;         // impulse applied to the other car
    [SerializeField] private float bonkUpwards = 1.2f;     // adds some upward push
    [SerializeField] private float bonkCooldown = 0.2f;    // avoid spamming bonks each physics frame
    [SerializeField] private float selfRecoil = 0f;        // optional recoil on this car (negative impulse)
    [SerializeField] private Rigidbody carRB;
    private float _lastBonkTime = -10f;
    private void OnCollisionEnter(Collision collision)
    {
        // Only bonk if enough time passed (cooldown)
    if (Time.time - _lastBonkTime < bonkCooldown) return;

    // Quick layer check
    if ((bonkLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

    // Get other rigidbody
    Rigidbody otherRb = collision.transform.GetComponent<Rigidbody>();
    if (otherRb == null) return; // nothing to apply force to

    // Avoid bonking static objects heavily (e.g., walls) - check isKinematic or mass
    if (otherRb.isKinematic) return;

    // Compute an impulse direction from our center to contact point
    ContactPoint contact = collision.contacts[0];
    Vector3 impulseDir = (otherRb.worldCenterOfMass - transform.position).normalized;

    // If very small direction (overlapping centers), fall back to collision normal
    if (impulseDir.sqrMagnitude < 0.001f)
        impulseDir = contact.normal;

    Vector3 impulse = impulseDir * bonkForce + Vector3.up * bonkUpwards;

    otherRb.AddForce(impulse, ForceMode.VelocityChange);

    // Optionally apply recoil to this car (feel of impact)
    if (selfRecoil != 0f && carRB != null)
    {
        carRB.AddForce(-impulse * selfRecoil, ForceMode.VelocityChange);
    }

    _lastBonkTime = Time.time;
    }
}
