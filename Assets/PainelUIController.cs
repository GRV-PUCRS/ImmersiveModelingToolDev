using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PainelUIController : Singleton<PainelUIController>
{
    [Header("Painel References")]
    [SerializeField] private GameObject view;

    [Header("Video Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetButton;

    public void SetPainel(PainelController painel)
    {
        view.SetActive(true);

        // Remove todos os eventos dos botoes
        playButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();

        // Adiciona eventos para cada botao

        playButton.onClick.AddListener(painel.PlayVideo);
        playButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, true));
        playButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, false));


        pauseButton.onClick.AddListener(painel.PauseVideo);
        pauseButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, true));
        pauseButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, false));

        resetButton.onClick.AddListener(painel.ResetVideo);
        resetButton.onClick.AddListener(() => SetElementActive(playButton.gameObject, true));
        resetButton.onClick.AddListener(() => SetElementActive(pauseButton.gameObject, false));
    }

    private void SetElementActive(GameObject element, bool value) { element.SetActive(value); }

    public void HideUI() { view.SetActive(false); }
}
