using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Logger : Singleton<Logger>
{
    [SerializeField] private TextMeshProUGUI output;
    [SerializeField] private bool toHide = false;
    [SerializeField] private float timeToHide = 3f;
    private int count = 0;
    public static void Log(object message)
    {
        Debug.Log(message);

        if (Instance.output == null) return;

        Instance.count++;
        if (Instance.count >= 50)
        {
            Instance.output.text = "";
            Instance.count = 0;
        }

        Instance.output.text += message + "\n";
        Instance.output.gameObject.SetActive(true);
        Instance.StopAllCoroutines();


        if (Instance.toHide)
        {
            Instance.StartCoroutine(Instance.HideLogTextAfterTime(Instance.timeToHide));
        }
    }

    private IEnumerator HideLogTextAfterTime(float time)
    {

        yield return new WaitForSeconds(time);

        output.gameObject.SetActive(false);
    }
}
