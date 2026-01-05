using UnityEngine;

public class ShootItem : MonoBehaviour, IItemUse
{
    public int UseAmount { get; set; } = 3;

    [SerializeField] private GameObject bullet;

    public void UseItem(Transform motor)
    {
        GameObject bulletClone = Instantiate(bullet, motor.position, motor.rotation);

        if (bulletClone.TryGetComponent(out BulletHandler bulletHandler))
        {
            bulletHandler.SetUsedMotor(motor);
        }
    } 
}
