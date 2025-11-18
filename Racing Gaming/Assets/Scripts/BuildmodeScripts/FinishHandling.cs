using UnityEngine;

public class FinishHandling : MonoBehaviour
{
    public static FinishHandling Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void HandleFinish()
    {
        
    }
}
