using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RequestViewController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    public void UpdateRequestStatus(int percent)
    {
        text.text = $"{percent}%";
    }
}
