using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRecorder : MonoBehaviour
{
    public SkinnedMeshRenderer origin;
    public SkinnedMeshRenderer destiny;

    private void Update()
    {
        destiny.sharedMesh = origin.sharedMesh;
    }
}
