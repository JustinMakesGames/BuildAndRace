using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct CameraViewports
{
    public Vector2 viewportPosition;
    public Vector2 viewportScale;
}
public class PlayerCarManagement : MonoBehaviour
{
    public static PlayerCarManagement Instance;

    [SerializeField] private Transform carFolder;
    [SerializeField] private List<Transform> playableCars = new List<Transform>();

    //Camera Handling
    [SerializeField] private List<CameraViewports> twoPlayerCameraViewports = new List<CameraViewports>();
    [SerializeField] private List<CameraViewports> morePlayerCameraViewports = new List<CameraViewports>();
    [SerializeField] private List<Camera> cameras = new List<Camera>();

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
    }

    public void SetPlayers(List<InputDevice> players, List<CarStats> carStats)
    {
        print("This function is being called.");
        for (int i = 0; i < players.Count; i++)
        {
            var car = carFolder.GetChild(i);
            car.GetComponent<ArcadeCarController>().SetPlayer(PlayerState.Player);
            car.GetComponent<PlayerInput>().enabled = true;
            car.GetComponent<PlayerInput>().SwitchCurrentControlScheme(players[i]);
            playableCars.Add(carFolder.GetChild(i));
            car.GetComponent<ArcadeCarController>().SetVariables(carStats[i]);
        }

        HandleCameras();
    }

    private void HandleCameras()
    {
        switch (playableCars.Count)
        {
            case 1:
                break;
            case 2:
                ChangeCameraViewports(twoPlayerCameraViewports);
                break;

            case > 2:
                ChangeCameraViewports(morePlayerCameraViewports);
                break;
                
        }
    }

    private void ChangeCameraViewports(List<CameraViewports> cameraViewports)
    {
        for (int i = 0; i < playableCars.Count; i++)
        {
            cameras[i].gameObject.SetActive(true);
            cameras[i].rect = new Rect(cameraViewports[i].viewportPosition.x, cameraViewports[i].viewportPosition.y,
                cameraViewports[i].viewportScale.x, cameraViewports[i].viewportScale.y);
        }
    }
}
