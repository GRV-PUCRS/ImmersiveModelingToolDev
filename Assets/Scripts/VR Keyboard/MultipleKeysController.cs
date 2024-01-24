using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultipleKeysController : MonoBehaviour
{
    [SerializeField] private float _timeToActiveMultipleKeys = 0.4f;
    [SerializeField] private TextMeshProUGUI _txtKeyValue;
    [SerializeField] private VirtualKeyboardController _virtualKeyboardController;

    [SerializeField] private GameObject _multipleKeysView;
    [SerializeField] private List<string> _keys;
    [SerializeField] private List<string> _upperKeys;
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private float _xOffset = 120;

    private Coroutine _onButtonPressedCoroutine;
    private bool _isMultipleKeyAction = false;
    private string _auxKeyValue;
    private bool _isShiftOn = false;

    private void Awake()
    {
        _multipleKeysView.SetActive(false);
        IsShiftOn = false;

        InitAuxKeys();
    }

    private void InitAuxKeys()
    {
        Utils.ClearChilds(_multipleKeysView.transform);

        float currentXPosition = (_xOffset * _keys.Count) / -2 + _xOffset / 2;

        var currentKeys = IsShiftOn ? _upperKeys : _keys;

        foreach (string key in currentKeys)
        {
            GameObject keyInstance = Instantiate(_keyPrefab, _multipleKeysView.transform);
            keyInstance.transform.localPosition = new Vector3(currentXPosition, 0, 0);

            var textComponent = keyInstance.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = key;

            var button = keyInstance.GetComponent<UIButton>();
            button._OnHighlightedButton.AddListener(() => OnKeyHighlighted(textComponent));
            button._OnHighlightedReleasedButton.AddListener(() => OnKeyHighlighted(_txtKeyValue));

            currentXPosition += _xOffset;
        }
    }

    public void OnButtonPressed()
    {
        if (_onButtonPressedCoroutine != null) StopCoroutine(_onButtonPressedCoroutine);

        _onButtonPressedCoroutine = StartCoroutine(OnStartButtonPress());
    }

    private IEnumerator OnStartButtonPress()
    {
        _isMultipleKeyAction = false;
        _auxKeyValue = _txtKeyValue.text;

        yield return new WaitForSecondsRealtime(_timeToActiveMultipleKeys);

        _isMultipleKeyAction = true;
        _multipleKeysView.SetActive(true);
    }

    public void OnButtonReleased()
    {
        if (_isMultipleKeyAction)
        {
            _virtualKeyboardController.OnKeyPressed(_auxKeyValue);
        }
        else
        {
            _virtualKeyboardController.OnKeyPressed(_txtKeyValue.text);
        }

        if (_onButtonPressedCoroutine != null) StopCoroutine(_onButtonPressedCoroutine);
        _multipleKeysView.SetActive(false);
    }

    public void OnKeyHighlighted(TextMeshProUGUI txtKey)
    {
        _auxKeyValue = txtKey.text;
    }

    public bool IsShiftOn { get => _isShiftOn; set { _isShiftOn = value; InitAuxKeys(); } }
}
