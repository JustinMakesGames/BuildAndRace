using UnityEngine;

public class LevelHandleMovement : MonoBehaviour
{
    [SerializeField] private Transform levelSelectScreen;

    [SerializeField] private int index;

    public void NextScreen()
    {
        int previousIndex = index;
        index++;
        levelSelectScreen.GetChild(previousIndex).gameObject.SetActive(false);
        levelSelectScreen.GetChild(index).gameObject.SetActive(true);
    }

    public void PreviousScreen()
    {
        int previousIndex = index;
        index--;
        levelSelectScreen.GetChild(previousIndex).gameObject.SetActive(false);
        levelSelectScreen.GetChild(index).gameObject.SetActive(true);
    }
}
