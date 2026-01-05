using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float groundOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float destroyTime;

    private Transform _usedMotor;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 20f, groundLayer))
        {
            _rb.MovePosition(hit.point + hit.normal * groundOffset);
            _rb.MoveRotation(Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, hit.normal),
                hit.normal));
        }
        _rb.MovePosition(_rb.position + transform.forward * speed * Time.deltaTime);
    }

    public void SetUsedMotor(Transform usedMotor)
    {
        _usedMotor = usedMotor;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform == _usedMotor) return;
            if (other.TryGetComponent(out ArcadeCarController controller))
            {
                controller.HandleMotorHit();
                Destroy(gameObject);
            }
        }
    }
}
