using UnityEngine;
using UnityEngine.UI;

public class SelectStatsSelectFolder : MonoBehaviour
{
    public static SelectStatsSelectFolder Instance;
    [SerializeField] private Transform selectionFolder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Slider SetMaxSpeedSlider(int index)
    {
        return selectionFolder.GetChild(index).Find("MaxSpeed").GetComponentInChildren<Slider>();
    }

    public Slider SetAccelerationSlider(int index)
    {
        return selectionFolder.GetChild(index).Find("Acceleration").GetComponentInChildren<Slider>();
    }

    public Slider SetHandlingSlider(int index)
    {
        return selectionFolder.GetChild(index).Find("Handling").GetComponentInChildren<Slider>();
    }
}
