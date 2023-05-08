using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.IO;

public class PainelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject view;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private DragUI dragUi;

    [Header("Componentes")]
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private RawImage imgField;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Edit Mode")]
    [SerializeField] private GameObject textEditMode;
    [SerializeField] private GameObject videoEditMode;

    private object[] message = new object[4];
    private ObjectFile painelFile;

    private enum VideoOperation { Play, Pause, Reset };

    private void OnEnable()
    {
        EventManager.OnPainelEvent += OnPainelEvent;

    }

    private void OnDisable()
    {
        EventManager.OnPainelEvent -= OnPainelEvent;
    }


    public void OnPainelEvent(object[] receivedMessage)
    {
        if ((string)receivedMessage[0] != name) return;

        VideoOperation op = (VideoOperation)receivedMessage[1];

        switch (op)
        {
            case VideoOperation.Play:
                videoPlayer.Play();

                break;

            case VideoOperation.Pause:
                videoPlayer.Pause();

                break;

            case VideoOperation.Reset:
                videoPlayer.Stop();
                break;
        }
    }

    public void PlayVideo()
    {
        SendVideoOperation(VideoOperation.Play);
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        SendVideoOperation(VideoOperation.Pause);
        videoPlayer.Pause();
    }

    public void ResetVideo()
    {
        SendVideoOperation(VideoOperation.Reset);
        videoPlayer.Stop();
    }

    private void SendVideoOperation(VideoOperation op)
    {
        message[0] = name;
        message[1] = op;

        EventManager.TriggerSendMessageRequest(Events.PAINEL_EVENT, message);
    }

    public void LoadFile(ObjectFile file)
    {
        painelFile = file;

        switch (file.ExtensionValue)
        {
            case ExtensionAllowed.ExtensionValue.Image:
                imgField.texture = Resources.Load<Texture>($"Prefabs/{file.Name}");

                break;

            case ExtensionAllowed.ExtensionValue.Text:
                TextAsset textAsset = Resources.Load<TextAsset>($"Prefabs/{file.Name}");

                if (textAsset != null)
                {
                    SetTextPainel(textAsset.text);
                }
                else
                {
                    dragUi.TransformToUpdate.name = $"File '{file.FileName}' not found!.ptx";
                    SetTextPainel(dragUi.TransformToUpdate.name.Replace(".ptx", ""));
                }

                break;

            case ExtensionAllowed.ExtensionValue.Video:
                videoPlayer.clip = Resources.Load<VideoClip>($"Prefabs/{file.Name}");
                videoEditMode.SetActive(true);
                videoPlayer.Play();
                videoPlayer.Stop();

                break;

            default:
                break;
        }

        textField.transform.parent.parent.gameObject.SetActive(file.ExtensionValue == ExtensionAllowed.ExtensionValue.Text);
        imgField.gameObject.SetActive(file.ExtensionValue == ExtensionAllowed.ExtensionValue.Image);
        videoPlayer.gameObject.SetActive(file.ExtensionValue == ExtensionAllowed.ExtensionValue.Video);

        //canvasRectTransform.gameObject.SetActive(true);
    }

    public void EditText(string newString)
    {
        textField.text = newString;
        dragUi.TransformToUpdate.name = newString + ".ptx";
    }

    public void SetTextPainel(string textContent)
    {
        textField.text = textContent;
        textEditMode.SetActive(true);
    }

    public void GetNewTextFromUser()
    {
        VRKeyboardManager.Instance.GetUserInputString(EditText, textField.text);
    }

    public void SetPainelSize(Vector3 newCanvasSize)
    {
        canvasRectTransform.sizeDelta = newCanvasSize;
        boxCollider.size = new Vector2(newCanvasSize.x * .15f, newCanvasSize.y);
        boxCollider.center = new Vector2((newCanvasSize.x * (1 - .15f)) / 2, 0);
    }

    public ExtensionAllowed.ExtensionValue PainelFileType { get => painelFile.ExtensionValue; }
}
