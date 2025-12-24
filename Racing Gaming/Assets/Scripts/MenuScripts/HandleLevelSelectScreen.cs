using System.Collections;
using TMPro;
using UnityEngine;

public class HandleLevelSelectScreen : MonoBehaviour
{
    [SerializeField] private GameObject levelButton;
    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private Transform levelSpawnPlace;
    private string[] _levels;

    private void Start()
    {
        StartCoroutine(SpawnLevels());
    }

    private IEnumerator SpawnLevels()
    {
        levelSelectScreen.SetActive(true);
        LevelData levelData = SaveLevelName.LoadBuild();

        _levels = levelData.levelNames;

        for (int i = 0; i < _levels.Length; i++)
        {
            GameObject newLevel = Instantiate(levelButton, levelSpawnPlace);

            var text = newLevel.GetComponentInChildren<TMP_Text>();

            text.text = _levels[i];
        }

        yield return null; //Wait one frame so vertical layout can work right.
        levelSelectScreen.SetActive(false);
        
    }
}
