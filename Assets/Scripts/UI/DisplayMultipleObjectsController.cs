using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMultipleObjectsController : MonoBehaviour
{
    [SerializeField] private int maxElementsPerPage;
    [SerializeField] private RectTransform elementsParent;
    [SerializeField] private TextMeshProUGUI txtElementCounter;
    [SerializeField] private GameObject elementPrefab;
    [SerializeField] private GameObject btnPrev;
    [SerializeField] private GameObject btnNext;

    private int maxPages;
    private int currentPage;
    private List<GameObject> elements;

    public void Setup<T>(List<ITemplateElement<T>> values)
    {
        Utils.ClearChilds(elementsParent);

        elements = new List<GameObject>();

        foreach (var value in values)
        {
            GameObject instance = Instantiate(elementPrefab, elementsParent);
            ITemplateController<T> controller = instance.GetComponentInChildren<ITemplateController<T>>();

            if (controller == null) continue;

            controller.SetValue(value);
            elements.Add(instance);
        }

        maxPages = Mathf.Max((elements.Count - 1) / maxElementsPerPage, 0);
        currentPage = 0;

        UpdateValues();
    }

    private void UpdateValues()
    {
        int beginIndex = currentPage * maxElementsPerPage;
        int endIndex = Mathf.Min(beginIndex + maxElementsPerPage, elements.Count);

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].SetActive(i >= beginIndex && i < endIndex);
        }

        txtElementCounter.text = $"({currentPage + 1}/{maxPages + 1})";
        btnPrev.SetActive(currentPage != 0);
        btnNext.SetActive(currentPage != maxPages);
    }

    public void Next() 
    {
        currentPage = Mathf.Min(currentPage + 1, maxPages);

        UpdateValues();
    }

    public void Prev()
    {
        currentPage = Mathf.Max(currentPage - 1, 0);

        UpdateValues();
    }

    public interface ITemplateElement<T>
    {
        T GetValue();
    }

    public interface ITemplateController<G>
    {
        void SetValue(ITemplateElement<G> value);
    }
}
