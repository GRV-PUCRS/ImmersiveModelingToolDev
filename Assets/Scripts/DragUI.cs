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
    private bool _isReferenceObject = false;
    private bool _isFixed = false;
    private bool _isPersistent = false;
    private bool _isSelected = false;
    private bool _isHighlighted = false;
    private bool _isOcclusion = false;
    private bool _isInteractionDisabled = false;

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

    public void BeginDrag(ObjectSelector controller)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject || IsDragging || IsFixed || IsInteractionDisabled) return;

        oldParent = transformToUpdate.parent;
        transformToUpdate.SetParent(controller.Pivot);

        isDragging = true;
    }

    public void EndDrag()
    {
        if (!isDragging) return;

        transformToUpdate.SetParent(oldParent);

        isDragging = false;
    }

    public void BeginScale(Transform newPivot1, Transform newPivot2, bool recursive = true)
    {
        if (!OculusManager.Instance.IsEditMode || IsReferenceObject || IsFixed || !IsInteractableObject || IsInteractionDisabled) return;

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

    public void SetSelected(bool isActive)
    {
        _isSelected = isActive;

        SetOutline(isActive, Color.red);
    }

    public void SetHighlighted(bool isActive)
    {
        _isHighlighted = isActive;

        if (_isSelected)
        {
            SetOutline(true, Color.red);
        }
        else
        {
            SetOutline(isActive, Color.white);
        }
    }

    public void SetHighlightedByAction(bool isActive)
    {
        _isHighlighted = isActive;

        if (_isSelected && !isActive)
        {
            SetOutline(true, Color.red);
        }
        else
        {
            SetOutline(isActive, Color.yellow);
        }

    }

    private void SetOutline(bool active, Color color)
    {
        var outline = GetComponent<Outline>();

        if (outline == null) return;

        outline.OutlineColor = color;
        
        if (active)
        {
            outline.EnableOutline();
        }
        else
        {
            outline.DisableOutline();
        }
    }

    public bool IsInteractableObject { get => gameObject.layer == LayerMask.NameToLayer("InteractableObject"); }
    public ColorController ColorController { get => colorController; }
    public bool IsReferenceObject { get => _isReferenceObject; set => _isReferenceObject = value; }
    public bool IsFixed { get => _isFixed; set => _isFixed = value; }
    public bool IsPersistent { get => _isPersistent; set => _isPersistent = value; }
    public bool IsSelected { get => _isSelected; }
    public bool IsHighlighted { get => _isHighlighted; }
    public bool IsDragging { get => isDragging; }
    public bool IsScalling { get => isInScaleMode; }
    public bool IsInteractionDisabled { get => _isInteractionDisabled; set => _isInteractionDisabled = value; }
    public bool IsOcclusion
    {
        get => _isOcclusion; set
        {
            _isOcclusion = value;
            SetObjectVisibility(_isOcclusion ? OculusManager.Instance.IsOcclusionObjVisible : true);
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
