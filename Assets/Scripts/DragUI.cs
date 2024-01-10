using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DragUI : MonoBehaviour, IDraggable
{
    [SerializeField] private string _id = "";
    [SerializeField] private string _description = "";
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool useParent = false;
    [SerializeField] private bool scallable = true;
    [SerializeField] protected float maxScaleTransformation = 1f;

    protected bool isDragging = false;
    protected bool isInScaleMode = false;
    protected Transform pivot1, pivot2;
    protected float oldDist;

    private Transform transformToUpdate;
    private Transform oldParent;

    private ColorController colorController;

    // Object Properties
    private bool isReferenceObject = false;
    private bool isFixed = false;
    private bool isPersistent = false;
    private bool isSelected = false;
    private bool isOcclusion = false;

    // Occlusion variables
    private bool isVisible = true;
    private Renderer[] renderers;
    private Material[] oldMaterials;
    private Material occlusionMaterial;

    public event Action OnDestroyedObject;

    protected virtual void Awake()
    {
        transformToUpdate = useParent ? transform.parent : transform;
        oldParent = transformToUpdate == null ? null : transformToUpdate.parent;

        colorController = new ColorController(transformToUpdate.gameObject);

        if (_id.Length == 0) _id = name;
    }

    private void Update()
    {
        if (isInScaleMode)
        {
            HandleScale();
        }
    }

    protected virtual void HandleScale()
    {
        float newDist = Vector3.Distance(pivot1.position, pivot2.position);
        float newScale = Mathf.Min(Mathf.Abs(newDist - oldDist) / oldDist, maxScaleTransformation);

        if (newScale > 0.1f)
        {
            newScale = (newDist > oldDist) ? newScale : -newScale;

            Vector3 oldScale = new Vector3();

            oldScale.x = OculusManager.Instance.ScaleX ? transform.localScale.x * newScale : 0;
            oldScale.y = OculusManager.Instance.ScaleY ? transform.localScale.y * newScale : 0;
            oldScale.z = OculusManager.Instance.ScaleZ ? transform.localScale.z * newScale : 0;

            transform.localScale = transform.localScale + oldScale;
            oldDist = newDist;

            SoundManager.Instance.PlaySound(newScale < 0 ? SoundManager.Instance.decreaseScale : SoundManager.Instance.increaseScale);
        }
    }

    public void BeginDrag(Transform controllerPivot)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject || IsDragging || IsFixed) return;

        oldParent = transformToUpdate.parent;
        transformToUpdate.SetParent(controllerPivot);

        isDragging = true;
    }

    public void EndDrag()
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject || !isDragging || IsFixed) return;

        transformToUpdate.SetParent(oldParent);

        isDragging = false;
    }

    public void BeginScale(Transform newPivot1, Transform newPivot2, bool recursive = true)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject || IsFixed) return;

        isInScaleMode = scallable;

        pivot1 = newPivot1;
        pivot2 = newPivot2;

        oldDist = Vector3.Distance(pivot1.position, pivot2.position);

        if (recursive)
        {
            foreach (Transform child in TransformToUpdate)
            {
                if (child == transform) continue;

                child.GetComponent<DragUI>().BeginScale(newPivot1, newPivot2, false);
            }
        }
    }

    public void EndScale(bool recursive = true)
    {
        isInScaleMode = false;

        if (recursive)
        {
            foreach (Transform child in TransformToUpdate)
            {
                if (child == transform) continue;

                child.GetComponent<DragUI>().EndScale(false);
            }
        }
    }

    public void SetNewTransform(Transform newTransform)
    {
        transformToUpdate = newTransform;
    }

    public void SetObjectVisibility(bool isVisible)
    {
        if (renderers == null)
            InitOcclusionValues();

        this.isVisible = isVisible;

        int i = 0;
        foreach (Renderer renderer in renderers)
        {
            renderer.material = isVisible ? oldMaterials[i++] : occlusionMaterial;
        }
    }

    public void InitOcclusionValues()
    {
        renderers = GetComponentsInChildren<Renderer>();
        oldMaterials = new Material[renderers.Length];
        occlusionMaterial = Resources.Load<Material>("Materials/OcclusionMat");

        for (int i = 0; i < renderers.Length; i++)
        {
            oldMaterials[i] = renderers[i].material;
        }
    }

    public ColorController ColorController { get => colorController; }
    public bool IsReferenceObject { get => isReferenceObject; set => isReferenceObject = value; }
    public bool IsFixed { get => isFixed; set => isFixed = value; }
    public bool IsPersistent { get => isPersistent; set => isPersistent = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public bool IsDragging { get => isDragging; }
    public bool IsScalling { get => isInScaleMode; }
    public bool IsOcclusion
    {
        get => isOcclusion; set
        {
            isOcclusion = value;
            SetObjectVisibility(isOcclusion ? OculusManager.Instance.IsOcclusionObjVisible : true);
        }
    }
    public bool IsVisible { get => isVisible; }
    public Transform TransformToUpdate { get => transformToUpdate; }

    public string ID { get => _id; }
    public string Description { get => _description; }

    private void OnDestroy()
    {
        OnDestroyedObject?.Invoke();
    }
}
