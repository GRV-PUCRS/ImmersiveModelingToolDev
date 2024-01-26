using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionnaireUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private QuestionnaireManager questionnaireManager;

    [Header("Gameobject References")]
    [SerializeField] private RectTransform questionParent;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private GameObject btnPrevious;
    [SerializeField] private GameObject btnNext;

    [Header("Prefabs")]
    [SerializeField] private GameObject radioQuestionPrefab;
    [SerializeField] private GameObject ListQuestionPrefab;

    private IQuestionDisplay currentblock;


    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Next();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Previous();
        }
    }

    public void UpdateUI()
    {
        Utils.ClearChilds(questionParent);
        QuestionBlock questionBlock = questionnaireManager.GetCurrentQuestionBlock();

        if (questionBlock == null) return;

        GameObject instance = null;

        switch (questionBlock.type)
        {
            case QuestionBlock.QuestionType.Radio: instance = Instantiate(radioQuestionPrefab, questionParent); break;
            case QuestionBlock.QuestionType.List: instance = Instantiate(ListQuestionPrefab, questionParent); break;
        }

        currentblock = instance.GetComponent<IQuestionDisplay>();
        currentblock.SetQuestion(questionBlock);

        txtDescription.text = questionBlock.description;
        btnPrevious.SetActive(questionnaireManager.CanGoBack());
        btnNext.SetActive(questionnaireManager.CanGoNext());
    }

    public void Next()
    {
        if (currentblock.CanGoNext())
        {
            SaveAnswers();
            questionnaireManager.NextQuestion();

            UpdateUI();
        }
        else
        {
            currentblock.ShowFeedback();
        }
    }

    public void Previous()
    {
        SaveAnswers();
        questionnaireManager.PreviousQuestion();

        UpdateUI();
    }

    public void SaveAnswers()
    {
        questionnaireManager.SaveAnswers(currentblock.GetAnswers());
    }
}
