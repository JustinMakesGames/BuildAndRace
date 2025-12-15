using System.Collections;
using UnityEngine;

public class ItemBoxHandling : MonoBehaviour
{
    [SerializeField] private Renderer itemRenderer;
    [SerializeField] private float interval;
    private bool _hasCollided;

    public void CollideWithBox()
    {
        if (_hasCollided) return;
        StartCoroutine(DisableThenEnableRenderer());
    }

    private IEnumerator DisableThenEnableRenderer()
    {
        _hasCollided = true;
        itemRenderer.enabled = false;
        yield return new WaitForSeconds(interval);
        itemRenderer.enabled = true;
        _hasCollided = false;
    }

    public bool HasCollided()
    {
        return _hasCollided;
    }
}
