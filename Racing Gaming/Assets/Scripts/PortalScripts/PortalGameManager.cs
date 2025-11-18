using UnityEngine;

public class PortalGameManager : MonoBehaviour
{
    public Camera cameraB;

    public Material cameraMaterialB;

    private void Start()
    {
        if (cameraB.targetTexture != null)
        {
            cameraB.targetTexture.Release();
        }

        cameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMaterialB.mainTexture = cameraB.targetTexture;
    }
}
