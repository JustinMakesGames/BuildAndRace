using UnityEngine;

public class TabsManagement : MonoBehaviour
{
    [SerializeField] private GameObject gamePage;
    [SerializeField] private GameObject selectedButton;
    public GameObject ReturnGamepage()
    {
        return gamePage;
    }

    public GameObject ReturnSelectedButton()
    {
        return selectedButton;
    }
}
