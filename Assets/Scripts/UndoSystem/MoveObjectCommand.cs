using System.Collections.Generic;
using UnityEngine;

public class MoveObjectCommand : ICommand
{
    private List<GameObject> references;
    private List<TransformInfo> oldTransformInfos;

    public MoveObjectCommand(List<GameObject> references)
    {
        this.references = new List<GameObject>(references);

        oldTransformInfos = new List<TransformInfo>();

        foreach (GameObject reference in references)
        {
            oldTransformInfos.Add(new TransformInfo(reference.transform));
        }
    }

    public void Undo()
    {
        for (int i = 0; i < references.Count; i++)
        {
            references[i].transform.localPosition = oldTransformInfos[i].localPosition;
            references[i].transform.localEulerAngles = oldTransformInfos[i].localEulerAngles;
            references[i].transform.localScale = oldTransformInfos[i].localScale;
        }
    }

    public class TransformInfo
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;

        public TransformInfo(Transform transform)
        {
            localPosition = transform.localPosition;
            localEulerAngles = transform.localEulerAngles;
            localScale = transform.localScale;
        }
    }
}
