using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRenderTexture : MonoBehaviour
{
    public RenderTexture original;
    public RenderTexture copy;
    
    private void OnPostRender()
    {
        Graphics.Blit(original, copy);
    }
}
