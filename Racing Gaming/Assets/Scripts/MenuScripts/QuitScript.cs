using System.Collections;
using UnityEngine;

public class QuitScript : MonoBehaviour, IPressButton
{
    [SerializeField] private Animator blackScreenAnimator;
    public void Press(Transform player)
    {
        StartCoroutine(PlayBlackScreenAnimation());
    }

    private IEnumerator PlayBlackScreenAnimation()
    {
        blackScreenAnimator.SetTrigger("BlackScreenIn");
        yield return new WaitForSeconds(2);
        Application.Quit();
    }
}
