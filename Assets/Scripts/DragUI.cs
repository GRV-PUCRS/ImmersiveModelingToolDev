using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragUI : MonoBehaviour, IDraggable
{
    [SerializeField] private bool useParent = false;
    [SerializeField] private bool scallable = true;
    [SerializeField] private float maxScaleTransformation = 1f;

    private AudioClip increaseSizeSoundEffect;
    private AudioClip decreaseSizeSoundEffect;

    private bool isInScaleMode = false;
    private Transform pivot1, pivot2;
    private float oldDist;

    private Transform transformToUpdate;
    private Transform oldParent;

    private bool isReferenceObject = false;
    private bool isFixed = false;

    private void Awake()
    {
        transformToUpdate = useParent ? transform.parent : transform;
        oldParent = transformToUpdate == null ? null : transformToUpdate.parent;

        increaseSizeSoundEffect = SoundManager.Instance.increaseScale;
        decreaseSizeSoundEffect = SoundManager.Instance.decreaseScale;
    }

    private void Update()
    {
        if (isInScaleMode)
        {
            float newDist = Vector3.Distance(pivot1.position, pivot2.position);
            float newScale = Mathf.Min(Mathf.Abs(newDist - oldDist) / oldDist, maxScaleTransformation);

            if (newScale > 0.1f)
            {
                newScale = (newDist > oldDist) ? newScale : -newScale;

                Vector3 oldScale = new Vector3();

                oldScale.x = OculusManager.Instance.ScaleX ? transformToUpdate.localScale.x * newScale : 0;
                oldScale.y = OculusManager.Instance.ScaleY ? transformToUpdate.localScale.y * newScale : 0;
                oldScale.z = OculusManager.Instance.ScaleZ ? transformToUpdate.localScale.z * newScale : 0;

                transformToUpdate.localScale = transformToUpdate.localScale + oldScale;
                oldDist = newDist;

                SoundManager.Instance.PlaySound(newScale < 0 ? decreaseSizeSoundEffect : increaseSizeSoundEffect);
            }
        }
    }

    public void BeginDrag(Transform controllerPivot)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject) return;

        Logger.Log("BGN drag  " + transformToUpdate.name);
        oldParent = transformToUpdate.parent;
        transformToUpdate.SetParent(controllerPivot);

        EventManager.TriggerDragBegin(transformToUpdate);
    }

    public void EndDrag()
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject) return;

        Logger.Log("END drag");
        transformToUpdate.SetParent(oldParent);

        EventManager.TriggerDragEnd(transformToUpdate);
    }

    public void BeginScale(Transform newPivot1, Transform newPivot2)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject) return;

        isInScaleMode = scallable;

        pivot1 = newPivot1;
        pivot2 = newPivot2;

        oldDist = Vector3.Distance(pivot1.position, pivot2.position);
    }

    public void EndScale()
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject) return;

        isInScaleMode = false;
    }

    public void SetNewTransform(Transform newTransform)
    {
        transformToUpdate = newTransform;
    }

    public bool IsReferenceObject { get => isReferenceObject; set => isReferenceObject = value; }
    public bool IsFixed { get => isFixed; set => isFixed = value; }
    public Transform TransformToUpdate { get => transformToUpdate; }
}
