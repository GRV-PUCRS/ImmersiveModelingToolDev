using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static DisplayMultipleObjectsController;
using static PainelController;
using static TextElementTemplateController;

public class FileSelector : MonoBehaviour
{
    [Header("Files Elements")]
    [SerializeField] private DisplayMultipleObjectsController filesDisplayController;

    [Header("Text Files Elements")]
    [SerializeField] private GameObject _textFieldView;
    [SerializeField] private GameObject _btnCreateTextPainel;
    [SerializeField] private DisplayMultipleObjectsController linesDisplayController;
    [SerializeField] private TextMeshProUGUI txtPainelText;

    [Header("Image Files Elements")]
    [SerializeField] private GameObject _imageFieldView;
    [SerializeField] private GameObject _btnCreateImagePainel;
    [SerializeField] private TextMeshProUGUI _txtImageName;
    [SerializeField] private RawImage _rawimgImagePreview;

    [Header("Video Files Elements")]
    [SerializeField] private GameObject _videoFieldView;
    [SerializeField] private GameObject _btnCreateVideoPainel;
    [SerializeField] private TextMeshProUGUI _txtVideoName;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _rawimgVideoPreview;

    private string filespath;
    private PainelController _painelController;

    public void OnLineSelected(string line)
    {
        txtPainelText.text += $"{line}\n";
        _btnCreateTextPainel.SetActive(true);
    }

    public void CreateTextPainel()
    {
        _painelController.SetTextPainel(txtPainelText.text, true);
        CloseScreen();
    }

    public void CreateImagePainel()
    {
        _painelController.SetImagePainel(filespath, _txtImageName.text);
        CloseScreen();
    }

    public void CreateVideoPainel()
    {
        _painelController.SetVideoPainel(filespath, _txtVideoName.text);
        CloseScreen();
    }

    private void PopulateDisplayer(List<string> values, DisplayMultipleObjectsController displayController, Action<string> callback)
    {
        List<ITemplateElement<TextElement>> elements = new List<ITemplateElement<TextElement>>();

        foreach (var value in values) elements.Add(new TextElement(value, callback));

        displayController.Setup(elements);
    }

    public void CloseScreen()
    {
        StartCoroutine(DestroyAfterFrame());
    }

    public void ClearContent()
    {
        txtPainelText.text = "";
        _btnCreateTextPainel.SetActive(false);
    }

    IEnumerator DestroyAfterFrame()
    {
        yield return null;

        EventManager.OnExitSession -= CloseScreen;
        EventManager.OnStageChange -= CloseScreen;

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventManager.OnExitSession += CloseScreen;
        EventManager.OnStageChange += CloseScreen;
    }

    private void InitTextScreen()
    {
        filespath = Path.Combine(Application.persistentDataPath, "bundles");

        PopulateDisplayer(Directory.GetFiles(filespath, "*.txt").Select(path => Path.GetFileName(path)).ToList(), filesDisplayController, OnTextFileSelected);

        linesDisplayController.gameObject.SetActive(false);

        _btnCreateTextPainel.SetActive(false);
    }

    private void OnTextFileSelected(string fileName)
    {
        string fileContent = File.ReadAllText(Path.Combine(filespath, fileName));

        PopulateDisplayer(fileContent.Split('\n').ToList(), linesDisplayController, OnLineSelected);

        linesDisplayController.gameObject.SetActive(true);
        _textFieldView.SetActive(true);
    }

    private void InitImageScreen()
    {
        string[] allowedExtensions = new string[] { "png", "jpg" };

        filespath = Path.Combine(Application.persistentDataPath, "bundles");

        PopulateDisplayer(Directory.GetFiles(filespath, "*.*").Where(f => allowedExtensions.Contains(f.Split(".").Last())).Select(path => Path.GetFileName(path)).ToList(), filesDisplayController, OnImageFileSelected);

        _btnCreateImagePainel.SetActive(false);
    }

    private void OnImageFileSelected(string fileName)
    {
        _txtImageName.text = fileName;

        byte[] fileData = File.ReadAllBytes(Path.Combine(filespath, fileName));
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        _rawimgImagePreview.texture  = texture;

        _btnCreateImagePainel.SetActive(true);
        _imageFieldView.SetActive(true);
    }

    private void InitVideoScreen()
    {
        string[] allowedExtensions = new string[] { "avi", "mp4", "mov" };

        filespath = Path.Combine(Application.persistentDataPath, "bundles");

        PopulateDisplayer(Directory.GetFiles(filespath, "*.*").Where(f => allowedExtensions.Contains(f.Split(".").Last())).Select(path => Path.GetFileName(path)).ToList(), filesDisplayController, OnVideoFileSelected);
        _btnCreateVideoPainel.SetActive(false);


        var renderTexture = new RenderTexture(1920, 1080, 16);
        _videoPlayer.targetTexture = renderTexture;
        _rawimgVideoPreview.texture = renderTexture;
    }

    private void OnVideoFileSelected(string fileName)
    {
        _txtVideoName.text = fileName;

        _videoPlayer.url = Path.Combine(filespath, fileName);

        _videoPlayer.Play();
        _btnCreateVideoPainel.SetActive(true);
        _videoFieldView.SetActive(true);
    }

    public void SetPainelController(PainelController painelController, PainelType painelType)
    {
        _painelController = painelController;

        switch (painelType)
        {
            case PainelType.Text: InitTextScreen(); break;
            case PainelType.Image: InitImageScreen(); break;
            case PainelType.Video: InitVideoScreen(); break;
        }

        _textFieldView.SetActive(false);
        _imageFieldView.SetActive(false);
        _videoFieldView.SetActive(false);
    }
}
