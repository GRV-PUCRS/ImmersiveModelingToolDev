using System.Collections.Generic;

public interface IQuestionDisplay
{
    void SetQuestion(QuestionBlock question);
    List<List<string>> GetAnswers();
    bool CanGoNext();
    void ShowFeedback();
}
