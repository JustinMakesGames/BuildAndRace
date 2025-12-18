using UnityEngine;

public class HandleShownMotor : MonoBehaviour
{
    [SerializeField] private GameObject motor;

    //Rotation Management
    [SerializeField] private float rotationSpeed;
    public void ChangeMotor(GameObject newMotor)
    {
        if (motor)
        {
            Destroy(motor);
        }

        motor = Instantiate(newMotor, transform.position, transform.rotation, transform);
    }

    public void DestroyMotor()
    {
        Destroy(motor);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
