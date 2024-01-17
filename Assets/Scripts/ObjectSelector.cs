using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ObjectSelector : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private bool isLeftController = false;
    [SerializeField] private bool isController = true;
    [SerializeField] private LayerMask target;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform objectHolderParent;
    [SerializeField] private ObjectSelector otherController;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color grabColor;
    [SerializeField] private Color scaleColor;
    [SerializeField] private int lineDistance = 5;
    [SerializeField] private GameObject pointer;
    [SerializeField] private BannerController _bannerController;


    private Transform currentHit;
    private Transform currentObjectHold;
    private bool turnOffDescription = false;
    private Vector3 hitPoint;
    private bool isTouchingObj = false;

    private bool toDeselect = false;

    private bool result;
    private RaycastHit info;
    private Vector3 _originalGrabLocalPosition;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (isLeftController)
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                OnTriggerDown();
            }

            if (Input.GetKeyUp(KeyCode.Y))
            {
                OnTriggerUp();
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.U))
            {
                OnTriggerDown();
            }

            if (Input.GetKeyUp(KeyCode.I))
            {
                OnTriggerUp();
            }
        }

        if (isTouchingObj || !lineRenderer.enabled) return;


        if (isController)
        {
            result = Physics.Raycast(pivot.position, pivot.forward, out info, lineDistance, target);
        }
        else
        {
            result = Physics.Raycast(transform.position, transform.forward, out info, lineDistance, target);
        }

        if (result)
        {
            if (isController)
            {
                lineRenderer.SetPosition(0, pivot.position);
                lineRenderer.SetPosition(1, info.point);
            }
            else
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(info.point, lineRenderer.transform.position));
            }

            hitPoint = info.point;

            _bannerController.ActiveBanner(isLeftController, info.transform.gameObject);

            if (currentHit != info.transform)
            {
                if (currentHit != null)
                {
                    if (currentHit.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                    {
                        OnHandleUnselectedUIElement(currentHit);
                        OnHandleReleaseUIElement(currentHit);
                    }
                    else
                    {
                        SetHighlight(currentHit.gameObject, false);
                    }

                }

                if (info.collider.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                {
                    OnHandleSelectUIElement(info.collider.gameObject);
                    SetLineRendererColor(hitColor);
                    currentHit = info.transform;
                    //PlaySound(SoundManager.Instance.highlightObject);
                }
                else
                {
                    if (OculusManager.Instance.IsEditMode && !IsReferenceObject(info.transform.gameObject))
                    {
                        SetHighlight(info.transform.gameObject, true);
                        SetLineRendererColor(hitColor);
                        currentHit = info.transform;

                        //PlaySound(SoundManager.Instance.highlightObject);
                    }
                }
            }

            HandleStickAction(InputController.Instance.GetStickState());

            pointer.transform.position = hitPoint;
        }
        else
        {
            if (isController)
            {
                lineRenderer.SetPosition(0, pivot.position);
                lineRenderer.SetPosition(1, pivot.position + pivot.forward * lineDistance);
            }
            else
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.forward * lineDistance);
            }

            if (currentHit != null)
            {
                if (currentHit.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                {
                    OnHandleUnselectedUIElement(currentHit);
                    OnHandleReleaseUIElement(currentHit);
                }
                else
                {
                    SetHighlight(currentHit.gameObject, false);
                }

                currentHit = null;


                SetLineRendererColor(normalColor);
                pointer.SetActive(false);
            }


            _bannerController.DeactivateBanner(isLeftController);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isTouchingObj = true;
        lineRenderer.enabled = false;
        currentHit = other.transform;

        Debug.Log("Seletor dentro do objeto " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        isTouchingObj = false;
        currentHit = null;

        Debug.Log("Seletor saiu do objeto " + other.name);
    }

    public void SetRayActive(bool isActive)
    {
        SetLineRenderActive(isActive);
        otherController.SetLineRenderActive(isActive);
    }

    public void SetLineRenderActive(bool isActive) => lineRenderer.enabled = isActive;

    private void HandleStickAction(Vector2 stickState)
    {
        if (stickState.y != 0)
        {
            int signal = stickState.y < 0 ? -1 : 1;

            if (Vector3.Distance(hitPoint, InputController.Instance.RightController.position) > 0.1f || signal > 0)
            {
                objectHolderParent.position = objectHolderParent.position + InputController.Instance.RightController.forward * Time.deltaTime * signal;
            }
        }
    }

    public bool IsReferenceObject(GameObject obj)
    {
        DragUI dragUI = obj.GetComponent<DragUI>();

        if (dragUI == null) return false;

        return dragUI.IsFixed;
    }

    public void SetHighlight(GameObject obj, bool newValue)
    {
        /*
        Outline outline = obj.transform.GetComponent<Outline>();

        if (outline != null)
            outline.IsActive = newValue;*/

        DragUI element = obj.GetComponent<DragUI>();

        if (element == null || element.IsSelected || !element.IsVisible) return;

        if (obj.layer == LayerMask.NameToLayer("Draggable"))
        {
            Outline outline = obj.GetComponent<Outline>();

            if (outline == null) return;

            if (newValue)
                outline.EnableOutline();
            else
                outline.DisableOutline();
        }
        else
        {
            foreach (Outline child in element.TransformToUpdate.GetComponentsInChildren<Outline>())
            {
                if (newValue)
                    child.EnableOutline();
                else
                    child.DisableOutline();
            }
        }
    }

    public void SetLineRendererColor(Color newColor)
    {
        lineRenderer.endColor = newColor;
    }

    private void OnHandleUnselectedUIElement(Transform obj)
    {
        if (obj == null) return;

        IUIButton uiButton = obj.GetComponent<IUIButton>();

        if (uiButton != null)
        {
            uiButton.OnUnselected(this);
        }
    }

    private void OnSubmitedHandler(Transform obj)
    {
        if (obj == null) return;

        IUIButton uiButton = obj.GetComponent<IUIButton>();

        if (uiButton != null)
        {
            uiButton.OnSubmited(this);
        }
    }

    private void OnHandleSelectUIElement(GameObject obj)
    {
        if (obj == null) return;

        IUIButton uiButton = obj.GetComponent<IUIButton>();

        if (uiButton != null)
        {
            uiButton.OnSelected(this);
        }
    }

    private void OnHandleReleaseUIElement(Transform obj)
    {
        if (obj == null) return;

        IUIButton uiButton = obj.GetComponent<IUIButton>();

        if (uiButton != null)
        {
            uiButton.OnReleased(this);
        }
    }

    private void OnTriggerUp()
    {
        if (currentObjectHold == null) return;

        if (currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
        {
            OnHandleReleaseUIElement(currentObjectHold);
        }
        else
        {
            if (!OculusManager.Instance.IsEditMode) return;

            bool isAction = currentObjectHold.GetComponent<IAction>() != null;

            // Acaba Escala
            if (currentObjectHold.GetComponent<DragUI>().IsScalling)
            {
                Debug.Log("Fim da Escala");

                foreach (GameObject sceneElement in OculusManager.Instance.SelectionList)
                {
                    foreach (DragUI child in sceneElement.GetComponentsInChildren<DragUI>())
                    {
                        child.EndScale();
                    }
                }

                otherController.SetPointerActive(false);
                otherController.SetLineRendererColor(hitColor);
                otherController.ObjectHold = null;

                SetLineRendererColor(hitColor);
                SetPointerActive(false);

                EventManager.TriggerScaleEnd(currentObjectHold.GetComponent<DragUI>().TransformToUpdate);
            }
            else // Solta objeto
            {
                Debug.Log("Fim do Drag");

                if (isAction || currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("Draggable")))
                {
                    currentObjectHold.GetComponent<DragUI>().EndDrag();
                }
                else
                {
                    foreach (GameObject sceneElement in OculusManager.Instance.SelectionList)
                    {
                        foreach (DragUI child in sceneElement.GetComponentsInChildren<DragUI>())
                        {
                            child.EndDrag();
                        }
                    }

                    if (toDeselect)
                    {
                        OculusManager.Instance.ClearSelection();
                    }
                }


                SetLineRendererColor(hitColor);
                pointer.SetActive(false);

                SoundManager.Instance.PlaySound(SoundManager.Instance.endGrab);

                EventManager.TriggerDragEnd(currentObjectHold.GetComponent<DragUI>());
            }
        }

        currentObjectHold = null;
    }

    private void OnTriggerDown()
    {
        if (currentHit == null) return;

        currentObjectHold = currentHit;

        if (currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("UI")) && currentObjectHold != otherController.ObjectHold)
        {
            OnSubmitedHandler(currentObjectHold);
        }
        else
        {
            if (!OculusManager.Instance.IsEditMode)
            {
                currentObjectHold = null;
                return;
            }

            // Se o outro objeto for igual a esse, comeca o scale
            if (CheckScaleOperation())
            {
                // End drag action on the other controller
                foreach (DragUI child in otherController.ObjectHolder.GetComponentsInChildren<DragUI>())
                {
                    child.EndDrag();
                }
                EventManager.TriggerDragEnd(otherController.ObjectHold.GetComponent<DragUI>());

                // Start scale action
                foreach (GameObject sceneElement in OculusManager.Instance.SelectionList)
                {
                    foreach (DragUI child in sceneElement.GetComponentsInChildren<DragUI>())
                    {
                        child.BeginScale(Pivot, otherController.Pivot);
                    }
                }

                otherController.SetLineRendererColor(scaleColor);
                SetLineRendererColor(scaleColor);
                SetPointerActive(true);

                EventManager.TriggerScaleBegin(currentObjectHold, Pivot, otherController.Pivot);

                SoundManager.Instance.PlaySound(SoundManager.Instance.beginGrab);
            }
            else if (otherController.ObjectHold == null) // senao, segue a acao de grab
            {
                objectHolderParent.localPosition = Vector3.zero;

                DragUI element = currentObjectHold.gameObject.GetComponent<DragUI>();

                if (element == null) return;

                if (currentObjectHold.GetComponent<IAction>() != null || currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("Draggable")))
                {
                    currentObjectHold.GetComponent<DragUI>().BeginDrag(ObjectHolder);
                }
                else
                {
                    if (!OculusManager.Instance.SelectionList.Contains(element.TransformToUpdate.gameObject))
                    {
                        OculusManager.Instance.ClearSelection();
                        OculusManager.Instance.AddSelectedObject(element.TransformToUpdate.gameObject);

                        toDeselect = true;
                    }
                    else
                    {
                        toDeselect = false;
                    }

                    foreach (GameObject sceneElement in OculusManager.Instance.SelectionList)
                    {
                        Debug.Log($"Scene Element === {sceneElement}");

                        DragUI child = sceneElement.GetComponentInChildren<DragUI>();

                        if (child == null) continue;

                        child.BeginDrag(objectHolderParent);
                    }
                }

                SetLineRendererColor(grabColor);
                SetPointerActive(true);
                pointer.transform.position = hitPoint;

                EventManager.TriggerDragBegin(currentObjectHold.gameObject.GetComponent<DragUI>());


                SoundManager.Instance.PlaySound(SoundManager.Instance.beginGrab);
            }
            else
            {
                currentObjectHold = null;
            }
        }

        Debug.Log("OnRightIndexTriggerDown 2");
    }

    private bool CheckScaleOperation()
    {
        if (otherController.ObjectHold == null) return false;

        return currentObjectHold.IsChildOf(otherController.ObjectHolder) || otherController.ObjectHold == currentObjectHold;
    }

    public void SetPointerActive(bool newValue)
    {
        pointer.SetActive(newValue);
    }

    public Transform ObjectHold { get => currentObjectHold; set => currentObjectHold = value; }
    public Transform ObjectHolder { get => objectHolderParent; }

    public Transform Pivot { get => pivot; }
    public bool IsLeft { get => isLeftController; }

    private void OnEnable()
    {
        if (isLeftController)
        {
            InputController.Instance.OnLeftIndexTriggerUp.AddListener(OnTriggerUp);
            InputController.Instance.OnLeftIndexTriggerDown.AddListener(OnTriggerDown);

        }
        else
        {
            InputController.Instance.OnRightIndexTriggerUp.AddListener(OnTriggerUp);
            InputController.Instance.OnRightIndexTriggerDown.AddListener(OnTriggerDown);
        }
    }
    private void OnDisable()
    {
        if (isLeftController)
        {
            InputController.Instance.OnLeftIndexTriggerUp.RemoveListener(OnTriggerUp);
            InputController.Instance.OnLeftIndexTriggerDown.RemoveListener(OnTriggerDown);

        }
        else
        {
            InputController.Instance.OnRightIndexTriggerUp.RemoveListener(OnTriggerUp);
            InputController.Instance.OnRightIndexTriggerDown.RemoveListener(OnTriggerDown);
        }
    }
}