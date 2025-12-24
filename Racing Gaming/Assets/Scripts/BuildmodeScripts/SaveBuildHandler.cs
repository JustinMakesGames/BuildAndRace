using TMPro;
using UnityEngine;

public class SaveBuilderHandler : MonoBehaviour
{
    public static SaveBuilderHandler Instance;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenCanvas()
    {
        canvas.SetActive(true);
    }
    public void SaveGame()
    {
        LevelName.Instance.SetLevelName(inputField.text);
        FinishHandling.Instance.FinishBuild();
    }
}
