using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackTileButtonHandler : MonoBehaviour, ISelectHandler
{

    public TrackTile trackTile;

    [SerializeField] private bool isFinishTile;
    private void Start()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(BuildManager.Instance.GetSpawnNewTileInput);
            if (isFinishTile) 
            {
                button.onClick.AddListener(BuildPlayerManagement.Instance.RemovePlayerTurn);
                return;

            } 
            button.onClick.AddListener(BuildPlayerManagement.Instance.GiveTurnToNextPlayer);
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        print("epic");
        BuildManager.Instance.ShowNewTile(trackTile, false);
    }
}
