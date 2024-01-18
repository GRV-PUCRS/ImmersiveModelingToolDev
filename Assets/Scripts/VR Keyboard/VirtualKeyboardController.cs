using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboardController : MonoBehaviour, IKeyboard
{
    private const string STR_CARET_OFF = "<color=#FFFFFF>|</color>";
    private const string STR_CARET_ON = "<color=#2B3C44>|</color>";
    private const string TMP_TEXTPREVIEW_COMPONENT_ID = "TXT Text Preview";


    [Header("Keyboards Views")]
    [SerializeField] private KeyboardTypeProvider _defaultKeyboard;
    [SerializeField] private KeyboardTypeProvider _numericKeyboard;
    private KeyboardTypeProvider _currentKeyboardProvider;

    [SerializeField] private float _caretRefreshTime = 0.4f;
    private TextMeshProUGUI _txtTextPreview;

    [SerializeField] private List<TextMeshProUGUI> _keys;
    [SerializeField] private TextMeshProUGUI _shift;
    private bool _isShiftOn = false;

    private int _caretPosition = 0;
    private string _text = "";

    private Action<string> _onConfirmAction;
    private Coroutine _caretAnimationCoroutine;

    private void Start()
    {
        _currentKeyboardProvider = _defaultKeyboard;

        _defaultKeyboard.SetActive(false);
        _numericKeyboard.SetActive(false);
    }

    public IEnumerator UpdateInputfield()
    {
        while (true)
        {
            _txtTextPreview.text = _text.Insert(_caretPosition, STR_CARET_ON);
            yield return new WaitForSecondsRealtime(_caretRefreshTime);
            _txtTextPreview.text = _text.Insert(_caretPosition, STR_CARET_OFF);
            yield return new WaitForSecondsRealtime(_caretRefreshTime);
        }
    }

    private void UpdateText()
    {
        _txtTextPreview.text = _text.Insert(_caretPosition, STR_CARET_ON);
    }

    public void OnKeyPressed(string key)
    {
        _text = _text.Insert(_caretPosition, key);
        MoveCaret(false);
    }

    public void Confirm()
    {
        _onConfirmAction?.Invoke(_text);
        _currentKeyboardProvider.SetActive(false);
    }

    public void Cancel()
    {
        _currentKeyboardProvider.SetActive(false);
    }

    public void MoveCaret(bool toLeft)
    {
        if (toLeft)
        {
            _caretPosition = Mathf.Max(_caretPosition - 1, 0);
        }
        else
        {
            _caretPosition = Mathf.Min(_caretPosition + 1, _text.Length);
        }

        UpdateText();
    }

    public void EnterKey()
    {
        _text = _text.Insert(_caretPosition, "\n");
        _caretPosition += 1;
        UpdateText(); 
        UpdateCaretAnimtion();
    }

    public void BackspaceKey()
    {
        if (_caretPosition == 0) return;

        if (_caretPosition != 1 && _text.Substring(_caretPosition - 2, 2).Equals("\n"))
        {
            _text = _text.Remove(_caretPosition - 2, 2);
            _caretPosition -= 2;
            UpdateText();
        }
        else
        {
            _text = _text.Remove(_caretPosition - 1);
            MoveCaret(true);
        }

        Debug.Log($"    {_caretPosition} {_text}");
    }

    public void OnKeyPressed(TextMeshProUGUI txtKey) => OnKeyPressed(txtKey.text);

    public void GetInput(InputField inputField, Action<string> onConfirm, TouchScreenKeyboardType keyboardType)
    {
        switch (keyboardType)
        {
            case TouchScreenKeyboardType.DecimalPad:
            case TouchScreenKeyboardType.NumbersAndPunctuation:
            case TouchScreenKeyboardType.NumberPad: _currentKeyboardProvider = _numericKeyboard; break;
            default: _currentKeyboardProvider = _defaultKeyboard; break;
        }

        _txtTextPreview = _currentKeyboardProvider._txtTextPreview;

        _text = inputField.text;
        _caretPosition = _text.Length;
        _onConfirmAction = onConfirm;

        _currentKeyboardProvider.SetActive(true);
        UpdateCaretAnimtion();
    }

    public void Shift()
    {
        _isShiftOn = !_isShiftOn;

        foreach (var key in _keys)
        {
            if (_isShiftOn) key.text = key.text.ToUpper();
            else key.text = key.text.ToLower();
        }

        if (!_isShiftOn) _shift.text = _shift.text.ToUpper();
        else _shift.text = _shift.text.ToLower();
    }

    public void SelectInputField()
    {
        Confirm();
        StopCoroutine(_caretAnimationCoroutine);
    }

    public void CancelSelection()
    {
        Cancel();
        StopCoroutine(_caretAnimationCoroutine);
    }

    public void UpdateCaretAnimtion()
    {
        if (_caretAnimationCoroutine != null) StopCoroutine(_caretAnimationCoroutine);

        _caretAnimationCoroutine = StartCoroutine(UpdateInputfield());
    }
}
