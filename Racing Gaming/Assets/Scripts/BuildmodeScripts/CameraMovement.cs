using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Orbit camera around a target using mouse drag + zoom with scroll.
/// Rotation is RELATIVE to target's transform.up (local up).
/// Uses new Input System (CallbackContext).
/// </summary>
[RequireComponent(typeof(Camera))]
public class MouseOrbitZoom_RelativeToTargetUp : MonoBehaviour
{
    [Header("Target & References")]
    [Tooltip("The object to orbit around")]
    [SerializeField] private Transform target;

    [Header("Orbit Settings")]
    [SerializeField, Range(0.1f, 20f)] private float orbitSensitivity = 5f;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    [Header("Zoom Settings")]
    [SerializeField, Range(0.5f, 50f)] private float minDistance = 2f;
    [SerializeField, Range(1f, 100f)] private float maxDistance = 20f;
    [SerializeField, Range(0.1f, 50f)] private float zoomSensitivity = 10f;
    [SerializeField] private bool invertZoom = false;

    [Header("Smoothing")]
    [SerializeField] private bool smoothOrbit = true;
    [SerializeField, Range(1f, 30f)] private float orbitSmoothSpeed = 12f;
    [SerializeField] private bool smoothZoom = true;
    [SerializeField, Range(1f, 30f)] private float zoomSmoothSpeed = 10f;

    // Internal state
    private float _currentDistance;
    private float _targetDistance;
    private float _currentYaw;     // degrees around target's up
    private float _currentPitch;   // degrees from target's forward plane
    private float _targetYaw;
    private float _targetPitch;

    // Input Actions
    [Header("Input Actions")]
    public InputActionReference orbitAction;
    public InputActionReference zoomAction;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();

        if (target == null)
        {
            Debug.LogError("MouseOrbitZoom: No target assigned!");
            enabled = false;
            return;
        }

        // Initialize distance
        _currentDistance = _targetDistance = Vector3.Distance(transform.position, target.position);

        // Initialize yaw/pitch from current camera → target vector
        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 targetUp = target.up;

        // Project toTarget onto plane perpendicular to targetUp
        Vector3 flatDir = Vector3.ProjectOnPlane(toTarget, targetUp).normalized;
        _currentYaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;

        // Pitch: angle between toTarget and flatDir
        _currentPitch = Vector3.Angle(flatDir, toTarget);
        if (toTarget.y < flatDir.y) _currentPitch = -_currentPitch;

        _targetYaw = _currentYaw;
        _targetPitch = _currentPitch;
    }

    private void OnEnable()
    {
        if (orbitAction != null)
        {
            orbitAction.action.Enable();
            orbitAction.action.performed += OnOrbitPerformed;
            orbitAction.action.canceled += OnOrbitPerformed;
        }

        if (zoomAction != null)
        {
            zoomAction.action.Enable();
            zoomAction.action.performed += OnZoomPerformed;
        }
    }

    private void OnDisable()
    {
        if (orbitAction != null)
        {
            orbitAction.action.performed -= OnOrbitPerformed;
            orbitAction.action.canceled -= OnOrbitPerformed;
            orbitAction.action.Disable();
        }

        if (zoomAction != null)
        {
            zoomAction.action.performed -= OnZoomPerformed;
            zoomAction.action.Disable();
        }
    }

    private void OnOrbitPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 delta = ctx.ReadValue<Vector2>();
        if (delta.sqrMagnitude < 0.01f) return;

        float x = delta.x * orbitSensitivity * (invertX ? -1f : 1f);
        float y = delta.y * orbitSensitivity * (invertY ? 1f : -1f);

        _targetYaw += x;
        _targetPitch -= y;
    }

    private void OnZoomPerformed(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>();
        if (Mathf.Abs(scroll) < 0.01f) return;

        float delta = scroll * zoomSensitivity * (invertZoom ? -1f : 1f);
        _targetDistance = Mathf.Clamp(_targetDistance - delta, minDistance, maxDistance);
    }

    private void LateUpdate()
    {
        // === SMOOTH ORBIT ===
        if (smoothOrbit)
        {
            _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, orbitSmoothSpeed * Time.deltaTime);
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, orbitSmoothSpeed * Time.deltaTime);
        }
        else
        {
            _currentYaw = _targetYaw;
            _currentPitch = _targetPitch;
        }

        // === SMOOTH ZOOM ===
        if (smoothZoom)
        {
            _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, zoomSmoothSpeed * Time.deltaTime);
        }
        else
        {
            _currentDistance = _targetDistance;
        }

        // === BUILD ROTATION AROUND TARGET'S UP ===
        Vector3 up = target.up;

        // Start with yaw rotation around target's up
        Quaternion yawRot = Quaternion.AngleAxis(_currentYaw, up);

        // Define local forward in the yaw plane
        Vector3 localForward = Vector3.forward;
        Vector3 worldForward = yawRot * localForward;

        // Apply pitch around the axis perpendicular to up and worldForward
        Vector3 pitchAxis = Vector3.Cross(up, worldForward).normalized;
        if (pitchAxis.sqrMagnitude < 0.001f)
            pitchAxis = Vector3.Cross(Vector3.up, worldForward).normalized; // fallback

        Quaternion pitchRot = Quaternion.AngleAxis(_currentPitch, pitchAxis);
        Quaternion finalRot = pitchRot * yawRot;

        // === POSITION ===
        Vector3 offset = finalRot * Vector3.back * _currentDistance;
        Vector3 desiredPos = target.position + offset;

        transform.position = desiredPos;
        transform.rotation = finalRot;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        minDistance = Mathf.Min(minDistance, maxDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(target.position, 0.5f);
        Gizmos.DrawRay(target.position, target.up * 2f);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, transform.position);
        }
    }
#endif
}