using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static DisplayMultipleObjectsController;
using static TextElementTemplateController;

public class TextManager : MonoBehaviour
{
    [SerializeField] private DisplayMultipleObjectsController filesDisplayController;
    [SerializeField] private DisplayMultipleObjectsController linesDisplayController;
    [SerializeField] private TextMeshProUGUI txtPainelText;
    [SerializeField] private Transform createObjectPivot;
    [SerializeField] private GameObject textPainelPrefab;

    private string filespath;

    private void Start()
    {
        filespath = Path.Combine(Application.persistentDataPath, "bundles");

        PopulateDisplayer(Directory.GetFiles(filespath, "*.txt").Select(path => Path.GetFileName(path)).ToList(), filesDisplayController, OnFileSelected);

        linesDisplayController.gameObject.SetActive(false);
    }

    public void OnFileSelected(string fileName)
    {
        string fileContent = File.ReadAllText(Path.Combine(filespath, fileName));

        PopulateDisplayer(fileContent.Split('\n').ToList(), linesDisplayController, OnLineSelected);

        linesDisplayController.gameObject.SetActive(true);
    }

    public void OnLineSelected(string line)
    {
        txtPainelText.text += $"{line}\n";
    }

    public void CreatePainel()
    {
        GameObject sceneElement = new GameObject("Scene Element");
        GameObject instance = Instantiate(textPainelPrefab, sceneElement.transform);
        PainelController controller =  instance.GetComponent<PainelController>();
        DragUI dragController = instance.GetComponent<DragUI>();

        if (controller == null || dragController == null)
        {
            Destroy(instance);

            Debug.LogWarning("[TextManager] Cannot create Text Painel.", this);

            return;
        }

        instance.transform.position = createObjectPivot.position;
        instance.transform.rotation = createObjectPivot.rotation;
        controller.SetTextPainel(txtPainelText.text, true);

        dragController.SetNewTransform(sceneElement.transform);

        txtPainelText.text = "";

        OculusManager.Instance.TaskManager.AddObjectInTask(sceneElement.transform);
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
    }

    IEnumerator DestroyAfterFrame()
    {
        yield return null;

        Destroy(gameObject);
    }
}
