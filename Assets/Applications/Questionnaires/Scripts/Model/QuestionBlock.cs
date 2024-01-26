using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestionBlock
{
    public enum QuestionType { Radio, List }

    public string description;
    public QuestionType type;
    public List<Question> questions = new List<Question>();

    public void Reset() => questions.ForEach(question => question.Reset());
}
