using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PainelController))]
public class TextReceiver : MonoBehaviour, IExternalDataListener
{
    [SerializeField] private string externalSourceID = "textPainel";
    [SerializeField] private TextMeshProUGUI txtID = null;

    private PainelController painelController;
    public static int idCounter = 0;

    private void Awake()
    {
        painelController = GetComponent<PainelController>();

        externalSourceID = $"{externalSourceID}_{idCounter++}";
        if (txtID != null) txtID.text = $"ID {externalSourceID}";
    }

    public string GetID()
    {
        return externalSourceID;
    }

    public void ReceivedData(string data)
    {
        painelController.EditText(data, false);
    }

    public void SetID(string newID)
    {
        Unsubscribe();

        externalSourceID = newID;
        if (txtID != null) txtID.text = $"ID {newID}";

        Subscribe();
    }

    public void ChangeID()
    {
        KeyboardManager.Instance.GetInput(SetID, null, externalSourceID);
    }

    public void Subscribe()
    {
        if (ExternalDataManager.Instance == null) return;

        ExternalDataManager.Instance.AddListener(this);
    }

    public void Unsubscribe()
    {
        if (ExternalDataManager.Instance == null) return;

        ExternalDataManager.Instance.RmvListener(this);
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
