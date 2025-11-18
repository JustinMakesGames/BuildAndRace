using UnityEngine;
using UnityEngine.EventSystems;

public class TrackTileButtonHandler : MonoBehaviour, ISelectHandler
{

    public TrackTile trackTile;

    public void OnSelect(BaseEventData eventData)
    {
        print("epic");
        BuildManager.Instance.ShowNewTile(trackTile, false);
    }
}
