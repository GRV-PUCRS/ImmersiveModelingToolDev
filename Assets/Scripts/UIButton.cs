using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IUIButton
{
    [SerializeField] private Color unselectedColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color selectedColor = new Color(0.849f, 0.849f, 0.849f, 1);
    [SerializeField] private Color submitedColor = new Color(0.7547f, 0.7547f, 0.7547f, 1);
    [SerializeField] private Color releasedColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color interactableOff = new Color(0.5943f, 0.5943f, 0.5943f, 1);
    [SerializeField] private bool interactableInExecMode = true;
    [SerializeField] private bool interactable = true;


    public UnityEvent callbacks;

    private Image backgroundButton;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        backgroundButton = GetComponent<Image>();
        Interactable = interactable;
    }

    private void Start()
    {
        if (interactable)
        {
            if (OculusManager.Instance != null)
                Interactable = interactableInExecMode ? true : OculusManager.Instance.IsEditMode;
        }
    }

    public void OnSelected(ObjectSelector controller)
    {
        if (!interactable) return;
        if (backgroundButton != null) backgroundButton.color = selectedColor;
    }

    public void OnUnselected(ObjectSelector controller)
    {
        if (!interactable) return;
        if (backgroundButton != null) backgroundButton.color = unselectedColor;
    }

    public void OnSubmited(ObjectSelector controller)
    {
        if (!interactable) return;

        if (backgroundButton != null) backgroundButton.color = submitedColor;

        callbacks?.Invoke();

        SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
    }

    public void OnReleased(ObjectSelector controller)
    {
        if (!interactable) return;
        if (backgroundButton != null) backgroundButton.color = releasedColor;
    }

    public void ToggleSelect()
    {
        if (toggle == null) return;

        toggle.isOn = !toggle.isOn;
    }

    public bool Interactable { get => interactable; set {
            interactable = value;
            if (backgroundButton != null) backgroundButton.color = interactable ? unselectedColor : interactableOff;
        } }

    public bool IsOn { 
        get {
            if (toggle == null) return false;
            return toggle.isOn;
        }
        set
        {
            if (toggle != null)
            {
                toggle.isOn = value;
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnExecModeChange += OnEditModeChange;
    }

    private void OnEditModeChange(bool newValue)
    {
        if (!interactableInExecMode)
            Interactable = newValue;
        else
            Interactable = true;
    }

    private void OnDisable()
    {
        EventManager.OnExecModeChange -= OnEditModeChange;
    }
}
