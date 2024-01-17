using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public string _message = "temp";
    public float _caretRefreshTime = 0.4f;

    private const string STR_CARET_OFF = "<color=#FFFFFF>|</color>";
    private const string STR_CARET_ON = "<color=#000000>|</color>";

    private int _caretPosition = 0;

    public TextMeshProUGUI _inputField;


    private void Start()
    {
        StartCoroutine(UpdateInputfield());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _caretPosition = Mathf.Min(_caretPosition + 1, _message.Length);
            _inputField.text = _message.Insert(_caretPosition, STR_CARET_ON);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _caretPosition = Mathf.Max(_caretPosition - 1, 0);
            _inputField.text = _message.Insert(_caretPosition, STR_CARET_ON);
        }
    }

    public IEnumerator UpdateInputfield()
    {
        while (true)
        {
            _inputField.text = _message.Insert(_caretPosition, STR_CARET_ON);
            yield return new WaitForSecondsRealtime(_caretRefreshTime);
            _inputField.text = _message.Insert(_caretPosition, STR_CARET_OFF);
            yield return new WaitForSecondsRealtime(_caretRefreshTime);
        }
    }
}
