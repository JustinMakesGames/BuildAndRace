using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private List<ItemScriptableObject> itemScriptableObjects = new List<ItemScriptableObject>();
    [SerializeField] private List<Sprite> imageSprites = new List<Sprite>();

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
    }

    public ItemScriptableObject ReturnItem()
    {
        int random = Random.Range(0, itemScriptableObjects.Count);

        return itemScriptableObjects[random];
    }

    public List<Sprite> ReturnImageSprites()
    {
        return imageSprites;
    }
}
