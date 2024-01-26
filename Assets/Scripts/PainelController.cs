using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System;
using System.Linq;

public class PainelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject textView;
    [SerializeField] private GameObject imageView;
    [SerializeField] private GameObject videoView;
    [SerializeField] private GameObject _selectFileView;

    [Header("Componentes")]
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private RawImage imgField;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage imgVideoField;
    [SerializeField] private string _id;
    [SerializeField] private string _description;

    [Header("Edit Mode")]
    [SerializeField] private GameObject textEditMode;
    [SerializeField] private GameObject videoEditMode;

    [Header("VideoPlayer components")]
    [SerializeField] private GameObject _btnPlay;
    [SerializeField] private GameObject _btnPause;
    [SerializeField] private GameObject _btnReset;
    [SerializeField] private GameObject _videoPlayerBackground;

    [Header("Prefabs")]
    [SerializeField] private FileSelector _fileSelectorPrafab;

    public enum PainelType { None, Text, Video, Image }

    [SerializeField] private PainelType _type = PainelType.None;

    private void Start()
    {
        EventManager_OnExecModeChange(OculusManager.Instance.IsEditMode);


        _videoPlayerBackground.SetActive(_type == PainelType.Video);
    }

    public void LoadFile(string fileName)
    {
        string extension = fileName.Split(".").Last();

        if (!ExtensionAllowed.IsAllowedExtension(extension)) return;

        string path = Path.Combine(Application.persistentDataPath, "bundles");

        switch (ExtensionAllowed.GetExtensionValue(extension))
        {
            case ExtensionAllowed.ExtensionValue.Image: SetImagePainel(path, fileName); break;
            case ExtensionAllowed.ExtensionValue.Video: SetVideoPainel(path, fileName); break;
            case ExtensionAllowed.ExtensionValue.CustomText: SetTextPainel(fileName); break;
            default: break;
        }
    }

    public void SetTextPainel(string content)
    {
        SetTextPainel(content.Replace(".ptx", ""), true);
    }

    public void SetTextPainel(string textContent, bool changeObjectName)
    {
        if (changeObjectName)
            transform.name = $"{textContent}.ptx";

        textField.text = textContent;

        textEditMode.SetActive(true);
        videoEditMode.SetActive(false);

        textView.SetActive(true);
        imageView.SetActive(false);
        videoView.SetActive(false);

        if (TryGetComponent(out WorldCanvasInteractor worldCanvasInteractor))
        {
            worldCanvasInteractor.SetRectTransformReference(textView.GetComponent<RectTransform>());
        }

        _type = PainelType.Text;
        _selectFileView.SetActive(false);
    }

    public void SetImagePainel(string path, string name)
    {
        transform.name = name;

        byte[] fileData = File.ReadAllBytes(Path.Combine(path, name));
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        imgField.texture = texture;

        textEditMode.SetActive(false);
        videoEditMode.SetActive(false);

        textView.SetActive(false);
        imageView.SetActive(true);
        videoView.SetActive(false);

        float aspectRatioHeight = texture.height / (float) texture.width;
        imgField.rectTransform.anchorMin = new Vector2(0, 0.5f - aspectRatioHeight/2f);
        imgField.rectTransform.anchorMax = new Vector2(1, 0.5f + aspectRatioHeight/2f);

        /*
        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }
        */
        if (TryGetComponent(out WorldCanvasInteractor worldCanvasInteractor))
        {
            worldCanvasInteractor.SetRectTransformReference(imgField.rectTransform);
        }

        _type = PainelType.Image;
        _selectFileView.SetActive(false);
    }

    public void SetVideoPainel(string path, string name)
    {
        transform.name = name;

        videoPlayer.url = Path.Combine(path, name);

        textEditMode.SetActive(false);
        videoEditMode.SetActive(true);
        
        textView.SetActive(false);
        imageView.SetActive(false);
        videoView.SetActive(true);

        videoPlayer.Play();
        videoPlayer.Stop();
        
        var renderTexture = new RenderTexture(1920, 1080, 16);
        videoPlayer.targetTexture = renderTexture;
        imgVideoField.texture = renderTexture;

        if (TryGetComponent(out WorldCanvasInteractor worldCanvasInteractor))
        {
            worldCanvasInteractor.SetRectTransformReference(videoView.GetComponent<RectTransform>());
        }

        _type = PainelType.Video;
        _selectFileView.SetActive(false);
        _videoPlayerBackground.SetActive(true);
    }

    public void EditText(string newString, bool saveText)
    {
        textField.text = newString;

        if (saveText)
            transform.name = newString + ".ptx";
    }

    public void EditText(string newString) => EditText(newString, true);

    public void GetNewTextFromUser()
    {
        KeyboardManager.Instance.GetInput(EditText, null, textField.text);
    }

    public void PlayVideo()
    {
        videoPlayer.Play();

        _btnPlay.SetActive(false);
        _btnPause.SetActive(true);
        _videoPlayerBackground.SetActive(false);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();

        _btnPlay.SetActive(true);
        _btnPause.SetActive(false);
    }

    public void ReplayVideo()
    {
        videoPlayer.Stop();
        PlayVideo();
    }

    public void OpenTextFileScreen() => OpenLoadFileScreen(PainelType.Text);
    public void OpenImageFileScreen() => OpenLoadFileScreen(PainelType.Image);
    public void OpenVideoFileScreen() => OpenLoadFileScreen(PainelType.Video);

    private void OpenLoadFileScreen(PainelType type)
    {
        var fileSelector = Instantiate(_fileSelectorPrafab);
        fileSelector.SetPainelController(this, type);


        var hmdToObjectDirection = InputController.Instance.HMD.position - transform.position;
        fileSelector.transform.position = transform.position + hmdToObjectDirection * 0.2f;
        fileSelector.transform.rotation = Quaternion.LookRotation(hmdToObjectDirection);
    }

    private void EventManager_OnExecModeChange(bool isEditMode)
    {
        switch (_type)
        {
            case PainelType.Text: textEditMode.SetActive(isEditMode); break;
            case PainelType.Image: break;
            case PainelType.Video: break;
            default: break;
        }
    }

    public string ID { get => _id; }
    public string Description { get => _description; }

    private void OnEnable()
    {
        EventManager.OnExecModeChange += EventManager_OnExecModeChange;
    }

    private void OnDisable()
    {
        EventManager.OnExecModeChange -= EventManager_OnExecModeChange;
    }
}
