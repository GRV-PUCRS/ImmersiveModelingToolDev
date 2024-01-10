using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DisplayMultipleObjectsController;
using static TextElementTemplateController;

public class TextElementTemplateController : MonoBehaviour, ITemplateController<TextElement>
{
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private UIButton btnSelectElement;

    public void SetValue(ITemplateElement<TextElement> value)
    {
        txtDescription.text = value.GetValue().Text;
        btnSelectElement.callbacks.AddListener(() => value.GetValue().OnSelected?.Invoke(txtDescription.text));
    }

    public class TextElement : ITemplateElement<TextElement>
    {
        public string Text { get; }
        public Action<string> OnSelected { get; }
        public TextElement(string text, Action<string> onSelected)
        {
            Text = text;
            OnSelected = onSelected;
        }

        public TextElement GetValue()
        {
            return this;
        }
    }
}
