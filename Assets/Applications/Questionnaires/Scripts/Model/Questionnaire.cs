using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Questionnaire
{
    public string description;
    public List<QuestionBlock> questionBlocks = new List<QuestionBlock>();

    public void ResetAnswers()
    {
        questionBlocks.ForEach(block => block.Reset());
    }
}
