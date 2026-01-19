using UnityEngine;

[CreateAssetMenu(fileName = "CarStats", menuName = "Scriptable Objects/CarStats")]
public class CarStats : ScriptableObject
{
    public GameObject motorModel;
    public int motorModelIndex;
    public int carIndex;
    public float maxSpeed;
    public float accelerationSpeed;
    public float steeringSpeed;
    public int maxSpeedSlider;
    public int accelerationSlider;
    public int steeringSlider;
}
