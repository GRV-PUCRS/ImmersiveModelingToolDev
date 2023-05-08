using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendARImage : MonoBehaviour
{
    public Camera camera;
    public RenderTexture rt;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, rt);
        Graphics.Blit(source, destination);
    }
}
