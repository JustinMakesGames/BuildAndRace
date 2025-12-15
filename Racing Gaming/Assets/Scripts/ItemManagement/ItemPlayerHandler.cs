using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemPlayerHandler : MonoBehaviour
{

    [Header("UI Handling")]
    [SerializeField] private Image itemImage;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [Header("Roulette Handling")]
    [SerializeField] private float rouletteInterval;
    private bool _isRouletting;
    private int _rouletteIndex;
    private bool _hasItem;

    [Header("Item Handling")]
    [SerializeField] private ItemScriptableObject itemScriptableObject;

    private GameObject _itemObject;
    private IItemUse _itemUse;
    private bool _canUseItem;

    [Header("CPU Handling")]
    [SerializeField] private float minTimeInput;
    [SerializeField] private float maxTimeInput;
    private PlayerState _playerState;

    public void ItemUseInput(InputAction.CallbackContext context)
    {
        if (context.started && _canUseItem)
        {
            HandleUsingItem();
        }
    }

    private void HandleUsingItem()
    {
        UseItem();
        _itemUse.UseItem(transform);
        Destroy(_itemObject);
    }

    private void UseItem()
    {
        _canUseItem = false;
        _hasItem = false;
        itemImage.sprite = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBox") && !_hasItem)
        {

            if (other.TryGetComponent(out ItemBoxHandling itemBoxScript))
            {
                if (itemBoxScript.HasCollided()) return;
            }

            StartCoroutine(ItemHandling());
        }

        if (other.CompareTag("ItemBox"))
        {
            if (other.TryGetComponent(out ItemBoxHandling itemBoxScript))
            {
                itemBoxScript.CollideWithBox();
            }
        }
    }

    private IEnumerator ItemHandling()
    {
        SetVariables();
        Shuffle(sprites);
        StartCoroutine(SetTime());
        yield return StartCoroutine(ItemRoulette());
        GetItem();
        HandleCPU();
    }

    private void SetVariables()
    {
        _hasItem = true;
        sprites = new List<Sprite>(ItemManager.Instance.ReturnImageSprites());
    }

    private IEnumerator SetTime()
    {
        _isRouletting = true;
        yield return new WaitForSeconds(rouletteInterval);
        _isRouletting = false;
    }

    private IEnumerator ItemRoulette()
    {
        _rouletteIndex = 0;
        while (_isRouletting)
        {
            itemImage.sprite = sprites[_rouletteIndex];

            _rouletteIndex = (_rouletteIndex + 1) % sprites.Count;

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void GetItem()
    {
        itemScriptableObject = ItemManager.Instance.ReturnItem();
        itemImage.sprite = itemScriptableObject.itemSprite;

        _itemObject = Instantiate(itemScriptableObject.itemObject, Vector3.zero, Quaternion.identity);
        _itemUse = _itemObject.GetComponent<IItemUse>();
        _canUseItem = true;

    }

    private void HandleCPU()
    {
        if (TryGetComponent(out ArcadeCarController carController))
        {
            _playerState = carController.ReturnPlayerState();

            if (_playerState == PlayerState.CPU)
            {
                StartCoroutine(HandleCPUInput());
            }
        }
    }

    private IEnumerator HandleCPUInput()
    {
        float randomInterval = Random.Range(minTimeInput, maxTimeInput);

        yield return new WaitForSeconds(randomInterval);
        HandleUsingItem();
    }
}
