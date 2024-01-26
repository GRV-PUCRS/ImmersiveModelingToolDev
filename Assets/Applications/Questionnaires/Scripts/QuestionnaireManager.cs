using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireManager : MonoBehaviour
{
    [SerializeField] private Questionnaire questionnaire;

    private int currentIndex;

    private void Awake()
    {
        StartQuestionnaire();
    }

    public void StartQuestionnaire()
    {
        currentIndex = 0;
        questionnaire.ResetAnswers();
    }

    public QuestionBlock GetCurrentQuestionBlock()
    {
        if (questionnaire.questionBlocks.Count == 0) return null;

        return questionnaire.questionBlocks[currentIndex];
    }

    public void NextQuestion()
    {
        if (!CanGoNext()) return;

        currentIndex++;
    }

    public void PreviousQuestion()
    {
        if (!CanGoBack()) return;

        currentIndex--;
    }

    public void SaveAnswers(List<List<string>> answers)
    {
        QuestionBlock block = GetCurrentQuestionBlock();

        if (answers.Count != block.questions.Count) return;

        for (int i = 0; i < answers.Count; i++)
        {
            block.questions[i].answers = answers[i];
        }
    }

    public bool CanGoNext() { return currentIndex < questionnaire.questionBlocks.Count - 1; }
    public bool CanGoBack() { return currentIndex > 0; }
}
