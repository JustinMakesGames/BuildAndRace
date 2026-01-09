using UnityEngine;

public class PlayerMotorRotationHandling : MonoBehaviour
{
    private HandleShownMotor _handleShownMotor;
    private int _index;
    private void Start()
    {
        
        print("handle yeah");
    }

    public void SetMotor(GameObject motor)
    {
        Transform motorSpawns = GameObject.FindGameObjectWithTag("MotorSpawns").transform;

        _handleShownMotor = motorSpawns.GetChild(_index).GetComponent<HandleShownMotor>();
        _handleShownMotor.ChangeMotor(motor);
    }
    public void SetIndex(int index)
    {
        print("SetMotor");
        _index = index;
    }
}
