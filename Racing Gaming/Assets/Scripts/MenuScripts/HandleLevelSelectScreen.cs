using System.Collections;
using TMPro;
using UnityEngine;

public class HandleLevelSelectScreen : MonoBehaviour
{
    [SerializeField] private GameObject levelButton;
    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private Transform levelSpawnPlaceFolder;
    [SerializeField] private GameObject levelSpawnPlace;

    [SerializeField] private int levelAmountPerFolder;
    private Transform _currentLevelSpawnPlace;
    private string[] _levels;

    private void Start()
    {
        StartCoroutine(SpawnLevels());
    }

    private IEnumerator SpawnLevels()
    {
        int times = 0;
        levelSelectScreen.SetActive(true);
        LevelData levelData = SaveLevelName.LoadBuild();

        _levels = levelData.levelNames;

        _currentLevelSpawnPlace = Instantiate(levelSpawnPlace, levelSpawnPlaceFolder).transform;
        for (int i = 0; i < _levels.Length; i++)
        {
            
            GameObject newLevel = Instantiate(levelButton, _currentLevelSpawnPlace);

            var text = newLevel.GetComponentInChildren<TMP_Text>();

            text.text = _levels[i];

            times++;

            if (times >= levelAmountPerFolder)
            {
                times = 0;
                _currentLevelSpawnPlace = Instantiate(levelSpawnPlace, levelSpawnPlaceFolder).transform;
            }
        }

        yield return null; //Wait one frame so vertical layout can work right.
        levelSelectScreen.SetActive(false);
        
    }
}
