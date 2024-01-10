using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTEste : MonoBehaviour
{
    private void Start()
    {
        AnimatorClipInfo[] a = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        Debug.Log(a.Length);

        foreach (var g in a)
            Debug.Log(g.clip.name);
    }
}
