using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewEncoderCustom : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;

    public RenderTexture GetRenderTexture()
    {
        return renderTexture;
    }
}
