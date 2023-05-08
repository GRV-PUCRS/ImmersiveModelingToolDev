using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    [SerializeField] private UnityEvent callbacks;

    private bool interactable;

    private void Awake()
    {
        Interactable = true;
    }

    public void OnSubmited()
    {
        if (!interactable) return;

        callbacks?.Invoke();

        Debug.Log("Toggle Submitted");
    }

    public bool Interactable { get => interactable; set => interactable = value; }
}
