using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [Header("Lap Count Handling")]
    public static RaceManager Instance;
    [SerializeField] private int maxLapCount;
    [SerializeField] private List<Transform> allCars = new List<Transform>();
    private List<Transform> _finishedCars = new List<Transform>();

    [Header("Finish Handling")]
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private List<TMP_Text> results = new List<TMP_Text>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

   
    public int GetMaxLapCount()
    {
        return maxLapCount;
    }

    //Handles the finished player
    public void FinishPlayer(Transform car)
    {

        if (_finishedCars.Contains(car)) return;

        _finishedCars.Add(car);
        int carIndex = _finishedCars.IndexOf(car);

        results[carIndex].text = car.name;
        car.GetComponent<ArcadeCarController>().SetPlayer(PlayerState.CPU);
        if (allCars.Count == _finishedCars.Count)
        {
            EndRace();
        }
    }

    //Handles the end of the race and shows the results screen doing this.
    private void EndRace()
    {
        StartCoroutine(HandleResultsScreen());
    }

    private IEnumerator HandleResultsScreen()
    {
        yield return new WaitForSeconds(1);
        resultScreen.SetActive(true);
        resultScreen.GetComponent<Animator>().SetTrigger("ResultsInAnimation");
        yield return new WaitForSeconds(5);
        resultScreen.GetComponent<Animator>().SetTrigger("ResultsOutAnimation");
        yield return new WaitForSeconds(4);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }
}
