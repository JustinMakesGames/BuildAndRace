using UnityEngine;

public class PlayerMotorRotationHandling : MonoBehaviour
{
    private HandleShownMotor _handleShownMotor;
    private int _index;
    private void Start()
    {
        Transform motorSpawns = GameObject.FindGameObjectWithTag("MotorSpawns").transform;

        _handleShownMotor = motorSpawns.GetChild(_index).GetComponent<HandleShownMotor>();
    }

    public void SetMotor(GameObject motor)
    {
        _handleShownMotor.ChangeMotor(motor);
    }
    public void SetIndex(int index)
    {
        _index = index;
    }
}
