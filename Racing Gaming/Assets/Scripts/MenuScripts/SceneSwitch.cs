using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour, IPressButton
{
    [SerializeField] private string nextScene;

    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Animator>();
    }
    public virtual void Press(Transform player)
    {
        StartCoroutine(HandleSceneSwitch());    
    }

    public void PressButtonInGame()
    {
        StartCoroutine(HandleSceneSwitch());
    }

    private IEnumerator HandleSceneSwitch()
    {
        if (nextScene == "MainMenu" && MenuManager.Instance != null)
        {
            Destroy(MenuManager.Instance.gameObject);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("BlackScreenIn");
            yield return new WaitForSeconds(2);
        }

        print("Loading new Scene");
        SceneManager.LoadScene(nextScene);
    }
}
