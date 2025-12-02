using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour, IPressButton
{
    [SerializeField] private string nextScene;
    public void Press(Transform player)
    {
        SceneManager.LoadScene(nextScene);
    }
}
