using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTEste : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            Teste();
        }
    }

    public void Teste()
    {
        var renderer = GetComponentInChildren<MeshRenderer>();

        Debug.Log("====>");
        Debug.Log(renderer.bounds.size);
        Debug.Log(renderer.bounds.max - renderer.bounds.min);
        Debug.Log(renderer.bounds.min + renderer.bounds.center);
        Debug.Log(renderer.bounds);
    }
}
