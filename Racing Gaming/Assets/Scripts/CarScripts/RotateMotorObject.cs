using UnityEngine;

public class RotateMotorObject : MonoBehaviour
{
    [SerializeField] private float positionSpeed;
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
        transform.position = Vector3.Lerp(transform.position, _parentTransform.position, positionSpeed * Time.deltaTime);
    }
    private void RotateTowardsParent()
    {
        Quaternion rotation = Quaternion.Lerp(transform.rotation, _parentTransform.rotation, rotationSpeed * Time.deltaTime);
        rotation.y = _parentTransform.rotation.y;
        transform.rotation = rotation;
    }
}
