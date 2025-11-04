using UnityEngine;

public class TimeScale : MonoBehaviour
{
    [SerializeField] private float timeScale;

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
