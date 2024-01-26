using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Question
{

    public string description;
    public List<string> alternatives = new List<string>();
    public List<string> answers = new List<string>();
    public bool multipleAnswers;
    public bool mandatory;

    public void Reset()
    {
        answers = new List<string>();
    }
}
