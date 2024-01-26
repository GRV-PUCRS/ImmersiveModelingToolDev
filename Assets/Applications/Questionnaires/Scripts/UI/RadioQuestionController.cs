using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ToggleGroup))]
public class RadioQuestionController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtDecription;
    [SerializeField] private RectTransform alternativesParent;
    [SerializeField] private GameObject alternativePrefab;
    [SerializeField] private Image imgBackground;

    private Question question;
    private ToggleGroup toggleGroup;
    private List<Toggle> toggles;

    private Coroutine coroutine;

    private void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        toggleGroup.allowSwitchOff = true;
    }

    public bool CanGoNext()
    {
        if (!question.mandatory) return true;

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn) return true;
        }

        return false;
    }

    public List<string> GetAnswers()
    {
        List<string> answers = new List<string>();

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn) answers.Add(toggle.name);
        }

        return answers;
    }

    public void SetQuestion(Question question)
    {
        this.question = question;
        UpdateUI();
    }

    public void UpdateUI()
    {
        Utils.ClearChilds(alternativesParent);

        txtDecription.text = question.description;
        toggles = new List<Toggle>();

        Debug.Log("Respostas ");
        foreach (string a in question.answers) Debug.Log(a);

        foreach (string alternative in question.alternatives)
        {
            GameObject instance = Instantiate(alternativePrefab, alternativesParent);

            Toggle toggle = instance.GetComponentInChildren<Toggle>();
            if (!question.multipleAnswers)
                toggle.group = toggleGroup;

            toggle.name = alternative;

            toggles.Add(toggle);

            instance.GetComponentInChildren<TextMeshProUGUI>().text = alternative;
        }

        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = question.answers.Contains(toggles[i].name);
        }
    }

    public void ShowVisualFeedback()
    {
        if (CanGoNext()) return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(VisualFeedback(2f));
    }

    private IEnumerator VisualFeedback(float feedbackDuration)
    {
        float colorValue = 0.5f * (Time.deltaTime / feedbackDuration);
        float accumColor = 0.5f;
        Color color = new Color(1f, accumColor, accumColor);

        while (accumColor < 1f)
        {
            accumColor += colorValue;

            color.g = accumColor;
            color.b = accumColor;

            imgBackground.color = color;

            yield return null;
        }
    }
}
