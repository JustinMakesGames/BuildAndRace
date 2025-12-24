using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseMenuHandling : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private MultiplayerEventSystem eventSystem;
    [SerializeField] private GameObject firstButton;

    [SerializeField] private List<ArcadeCarController> cars = new List<ArcadeCarController>();

    private void Start()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            cars.Add(transform.parent.GetChild(i).GetComponent<ArcadeCarController>());
        }
    }

    public void PauseInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Pause();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(firstButton);

        foreach (var car in cars)
        {
            car.Pause();
        }
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

        StartCoroutine(UnpauseCars());
        
    }

    private IEnumerator UnpauseCars()
    {
        yield return new WaitForFixedUpdate();

        foreach (var car in cars)
        {
            car.Unpause();
        }
    }

    
}
