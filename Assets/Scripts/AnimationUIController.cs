using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationUIController : Singleton<AnimationUIController>
{
    [Header("UI References")]
    [SerializeField] private GameObject view;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button stopButton;

    public void SetAnimation(AnimationController animController)
    {
        view.SetActive(true);

        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(true);

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => animController.PlayAnimation(true));
        playButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, false));
        playButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, true));

        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(() => animController.PauseAnimation(true));
        pauseButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, false));
        pauseButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, true));

        stopButton.onClick.RemoveAllListeners();
        stopButton.onClick.AddListener(() => animController.ResetAnimation(true));
        stopButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, true));
        stopButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, false));


        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    private void SetElementActive(GameObject element, bool value) { element.SetActive(value); }

    public void HideUI() { view.SetActive(false); }
}
