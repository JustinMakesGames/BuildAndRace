using UnityEngine;

public class RotateMotorObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Transform _parentTransform;

    private void Start()
    {
        _parentTransform = transform.parent;
        transform.parent = null;
    }
    private void Update()
    {
        UpdatePosition();
        RotateTowardsParent();
    }

    private void UpdatePosition()
    {
        transform.position = _parentTransform.position;
    }
    private void RotateTowardsParent()
    {
        Quaternion rotation = Quaternion.Lerp(transform.rotation, _parentTransform.rotation, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }
}
