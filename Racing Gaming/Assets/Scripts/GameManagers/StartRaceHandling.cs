using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartRaceHandling : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private TMP_Text countdownText;

    [Header("Countdown Variables")]
    [SerializeField] private bool isRacing;
    [SerializeField] private int interval;

    [SerializeField] private List<Transform> cars = new List<Transform>();

    private int _countdownNumber = 4;

    private void Start()
    {
        if (!isRacing) return;

        StartCoroutine(StartCountdown());
    }

    //Handles the countdown
    private IEnumerator StartCountdown()
    {

        StartGettingCarsReady();
        yield return new WaitForSeconds(2);
        countdownText.gameObject.SetActive(true);

        while (_countdownNumber > 1)
        {
            _countdownNumber--;
            countdownText.text = _countdownNumber.ToString();
            yield return new WaitForSeconds(interval);
        }

        countdownText.text = "GO!";
        MakeCarsReady();
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);
    }

    private void StartGettingCarsReady()
    {
        foreach (Transform car in cars)
        {
            car.GetComponent<ArcadeCarController>().SetVerhicleOff();
            car.GetComponent<StartBoostBehaviour>().ActivateBoostSlider();
            
        }
    }

    private void MakeCarsReady()
    {
        foreach (Transform car in cars)
        {
            car.GetComponent<ArcadeCarController>().SetVerhicleOn();
            car.GetComponent<StartBoostBehaviour>().GiveBoost();
        }
    }
}
