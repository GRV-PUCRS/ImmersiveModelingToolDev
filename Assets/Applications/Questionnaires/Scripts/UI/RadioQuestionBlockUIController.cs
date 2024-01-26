using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RadioQuestionBlockUIController : MonoBehaviour, IQuestionDisplay
{
    [SerializeField] private RectTransform radioQuestionParent;
    [SerializeField] private GameObject radioQuestionPrefab;

    private QuestionBlock questionBlock;
    List<RadioQuestionController> controllers;

    public bool CanGoNext()
    {
        foreach (RadioQuestionController controller in controllers)
        {
            if (!controller.CanGoNext()) return false;
        }

        return true;
    }

    public List<List<string>> GetAnswers()
    {
        List<List<string>> answers = new List<List<string>>();

        foreach (RadioQuestionController controller in controllers)
            answers.Add(controller.GetAnswers());

        return answers;
    }

    public void SetQuestion(QuestionBlock questionBlock)
    {
        this.questionBlock = questionBlock;
        UpdateUI();
    }

    public void ShowFeedback()
    {
        foreach (RadioQuestionController controller in controllers) controller.ShowVisualFeedback();
    }

    public void UpdateUI()
    {
        Utils.ClearChilds(radioQuestionParent);

        controllers = new List<RadioQuestionController>();

        foreach (Question question in questionBlock.questions)
        {
            RadioQuestionController controller = Instantiate(radioQuestionPrefab, radioQuestionParent).GetComponent<RadioQuestionController>();
            controller.SetQuestion(question);

            controllers.Add(controller);
        }
    }
}
