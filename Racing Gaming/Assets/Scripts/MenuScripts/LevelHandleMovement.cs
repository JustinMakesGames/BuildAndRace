using UnityEngine;

public class LevelHandleMovement : MonoBehaviour
{
    [SerializeField] private Transform levelSelectScreen;

    [SerializeField] private int index;

    public void NextScreen()
    {
        index++;
        SelectPlayModeHandling.Instance.SetSelectedFolder(levelSelectScreen.GetChild(index));
    }

    public void PreviousScreen()
    {
        index--;
        SelectPlayModeHandling.Instance.SetSelectedFolder(levelSelectScreen.GetChild(index));
    }
}
