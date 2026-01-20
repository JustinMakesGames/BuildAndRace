using UnityEngine;

public class SelectPlayModeHandling : SwitchPages, IPressButton
{

    public static SelectPlayModeHandling Instance;

    [SerializeField] private Transform levelSelectFolder;

    private Transform _player;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (levelSelectFolder.childCount == 0) return;
        SetSelectedFolder(levelSelectFolder.GetChild(0));
    }

    public override void Press(Transform player)
    {
        _player = player;
        base.Press(player);
    }

    public void SetSelectedFolder(Transform selectionFolder)
    {
        this.selectionFolder = selectionFolder;

        for (int i = 0; i < levelSelectFolder.childCount; i++)
        {
            levelSelectFolder.GetChild(i).gameObject.SetActive(false);
        }

        selectionFolder.gameObject.SetActive(true);
        Press(_player);
        
    }

}
