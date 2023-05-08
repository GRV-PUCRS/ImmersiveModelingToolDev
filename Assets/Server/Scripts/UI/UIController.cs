using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    private TextMeshProUGUI roomText;
    private TextMeshProUGUI playerCountText;
    private RawImage worldViewImage;
    private static Text log;

    [Header("Render Textures")]
    [SerializeField] private RenderTexture clientViewTexture;
    [SerializeField] private RenderTexture freeViewTexture;
    [SerializeField] private RenderTexture fullEnvTexture;

    [Header("Cameras")]
    [SerializeField] private Camera clientViewCamera;
    [SerializeField] private Camera freeViewCamera;
    [SerializeField] private Camera fullEnvCamera;

    [Header("UI Components")]
    [SerializeField] private RectTransform streamView;
    [SerializeField] private RectTransform menuView;
    [SerializeField] private float aspectRatio;

    private void Awake()
    {
        Instance = this;

        roomText = transform.Find("Room").GetComponent<TextMeshProUGUI>();
        playerCountText = transform.Find("PlayerCount").GetComponent<TextMeshProUGUI>();
        worldViewImage = transform.Find("WorldView").GetComponent<RawImage>();

        //log = transform.Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        SetFreeViewCamera();
    }

    public static void Log(string message)
    {
        //log.text += '\n' + message;
    }

    public void SetRoom(string roomName)
    {
        roomText.text = $"Room: {roomName}";
    }

    public void SetPlayerCount(int playerCount)
    {
        playerCountText.text = $"Player Count: {playerCount}";
    }

    public void SetFreeViewCamera()
    {
        worldViewImage.texture = freeViewTexture;
        EventManager.TriggerCameraViewChange(freeViewCamera);
        EventManager.TriggerObjectSelected(freeViewCamera.transform);
    }

    public void SetClientViewCamera()
    {
        worldViewImage.texture = clientViewTexture;
        EventManager.TriggerCameraViewChange(clientViewCamera);
    }

    public void SetFullEnvViewCamera()
    {
        worldViewImage.texture = fullEnvTexture;
        EventManager.TriggerCameraViewChange(fullEnvCamera);
        EventManager.TriggerObjectSelected(fullEnvCamera.transform);
    }

    private void OnEnable()
    {
        EventManager.OnScreenSizeChange += OnScreenSizeChange;
    }



    private void OnScreenSizeChange(int newWidth, int newHeight)
    {
        bool isFreeView = worldViewImage.texture == freeViewTexture;

        freeViewTexture = UpdateTexture(freeViewTexture, newWidth, newHeight);
        freeViewCamera.targetTexture = freeViewTexture;

        clientViewTexture = UpdateTexture(clientViewTexture, newWidth, newHeight);
        clientViewCamera.targetTexture = clientViewTexture;

        fullEnvTexture = UpdateTexture(fullEnvTexture, newWidth, newHeight);
        fullEnvCamera.targetTexture = fullEnvTexture;

        if (worldViewImage.texture == freeViewTexture)
            worldViewImage.texture = freeViewTexture;
        else if (worldViewImage.texture == clientViewTexture)
            worldViewImage.texture = clientViewTexture;
        else
            worldViewImage.texture = fullEnvTexture;

        float streamGlobalView = Screen.width - menuView.rect.width;
        float streamViewHeight = newHeight / 2;
        streamView.sizeDelta = new Vector2(aspectRatio * streamViewHeight, streamViewHeight);
        streamView.transform.position = new Vector3(menuView.rect.width + (streamGlobalView - streamView.sizeDelta.x) / 2, newHeight, 0);


        Debug.Log($"{worldViewImage.texture.width} - {worldViewImage.texture.height}");
    }

    private RenderTexture UpdateTexture(RenderTexture renderTexture, int newWidth, int newHeight)
    {
        RenderTexture newRenderTexture = new RenderTexture(renderTexture);
        newRenderTexture.width = newWidth;
        newRenderTexture.height = newHeight;
        newRenderTexture.Create();

        return newRenderTexture;
    }
}
