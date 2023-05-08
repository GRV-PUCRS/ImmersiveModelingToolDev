using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer), typeof(AudioSource))]
public class ObjectSelector : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private bool isLeftController = false;
    [SerializeField] private LayerMask target;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Transform pivot;
    [SerializeField] private ObjectSelector otherController;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color grabColor;
    [SerializeField] private Color scaleColor;
    [SerializeField] private int lineDistance = 5;
    [SerializeField] private GameObject pointer;

    private AudioSource audioSource;

    private Transform currentHit;
    private Transform currentObjectHold;
    private bool turnOffDescription = false;
    private Vector3 hitPoint;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
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

            /*
            if (Input.GetKeyUp(KeyCode.F))
            {
                Teste(new Vector2(0, 1));
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                Teste(new Vector2(0, -11));
            }
            */
        }

        if (Physics.Raycast(pivot.position, pivot.forward, out RaycastHit info, lineDistance, target))
        {
            lineRenderer.enabled = true;

            lineRenderer.SetPosition(0, pivot.position);
            lineRenderer.SetPosition(1, info.point);
            hitPoint = info.point;

            if (OculusManager.Instance.IsEditMode && info.transform.gameObject.layer.Equals(LayerMask.NameToLayer("StoredObject")) && description != null)
            {
                description.transform.parent.parent.gameObject.SetActive(true);
                description.text = info.transform.name;
                turnOffDescription = true;
            }

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
            
            lineRenderer.SetPosition(0, pivot.position);
            lineRenderer.SetPosition(1, pivot.position + pivot.forward * lineDistance);
            
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

            if (description != null && turnOffDescription)
            {
                description.transform.parent.parent.gameObject.SetActive(false);
                turnOffDescription = false;
            }
        }
    }

    private void HandleStickAction(Vector2 stickState)
    {
        if (stickState.y != 0)
        {

            int signal = stickState.y < 0 ? -1 : 1;

            if (Vector3.Distance(hitPoint, InputController.Instance.RightController.position) > 0.1f || signal > 0)
            {
                currentObjectHold.position = currentObjectHold.position + InputController.Instance.RightController.forward * Time.deltaTime * signal;
            }
        }
    }

    public void PlaySound(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.Play();

        Debug.Log(newClip.name);
    }

    public bool IsReferenceObject(GameObject obj)
    {
        DragUI dragUI = obj.GetComponent<DragUI>();

        if (dragUI == null) return false;

        return dragUI.IsReferenceObject;
    }

    public void SetHighlight(GameObject obj, bool newValue)
    {
        OcclusionObject occlusion = obj.GetComponent<OcclusionObject>();

        if (occlusion != null)
        {
            if (!occlusion.Visibility)
                return;
        }

        Outline outline = obj.transform.GetComponent<Outline>();

        if (outline != null)
            outline.IsActive = newValue;
    }

    public void SetLineRendererColor(Color newColor)
    {
        lineRenderer.endColor = newColor;
    }

    private void OnHandleUnselectedUIElement(Transform obj)
    {
        if (obj == null) return;

        UIButton uiButton = obj.GetComponent<UIButton>();

        if (uiButton != null)
        {
            uiButton.OnUnselected();
        }
    }

    private void OnSubmitedHandler(Transform obj)
    {
        if (obj == null) return;

        UIButton uiButton = obj.GetComponent<UIButton>();

        if (uiButton != null)
        {
            uiButton.OnSubmited();
        }
    }

    private void OnHandleSelectUIElement(GameObject obj)
    {
        if (obj == null) return;

        UIButton uiButton = obj.GetComponent<UIButton>();

        if (uiButton != null)
        {
            uiButton.OnSelected();
        }
    }

    private void OnHandleReleaseUIElement(Transform obj)
    {
        if (obj == null) return;

        UIButton uiButton = obj.GetComponent<UIButton>();

        if (uiButton != null)
        {
            uiButton.OnReleased();
        }
    }

    private void OnTriggerUp()
    {
        if (currentObjectHold == null) return;

        if (currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
        {
            OnHandleReleaseUIElement(currentObjectHold);

        }
        else if (currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("Key")))
        {
            currentObjectHold.gameObject.GetComponent<ButtonVR>().OnButtonReleased();
        }
        else
        {
            if (!OculusManager.Instance.IsEditMode) return;

            if (otherController.ObjectHold == currentObjectHold)
            {
                currentObjectHold.GetComponent<DragUI>().EndScale();
                otherController.ObjectHold = null;
                otherController.SetPointerActive(false);
                otherController.SetLineRendererColor(hitColor);

                SetLineRendererColor(hitColor);
                SetPointerActive(false);
            }
            else
            {
                currentObjectHold.GetComponent<DragUI>().EndDrag();
                SetLineRendererColor(hitColor);
                pointer.SetActive(false);

                PlaySound(SoundManager.Instance.endGrab);
            }
        }

        /*
        if (Equals(currentObjectHold.gameObject.layer, LayerMask.NameToLayer("InteractableObject")))
        {
            InteractableObject instance = currentObjectHold.GetComponent<InteractableObject>();

            if (instance.Type == InteractableObject.ObjectType.Animation)
            {
                AnimationController controller = instance.GetComponent<AnimationController>();

                controller.PlayAnimation(false);
            }
        }
        */
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
        else if (currentObjectHold.gameObject.layer.Equals(LayerMask.NameToLayer("Key"))/* && currentObjectHold != otherController.ObjectHold*/)
        {
            currentObjectHold.gameObject.GetComponent<ButtonVR>().OnButtonPressed();
        }
        else
        {
            if (!OculusManager.Instance.IsEditMode)
            {
                currentObjectHold = null;
                return;
            }

            // Se o outro objeto for igual a esse, comeca o scale
            if (otherController.ObjectHold != null && otherController.ObjectHold == currentObjectHold)
            {
                currentObjectHold.GetComponent<DragUI>().EndDrag();

                currentObjectHold.GetComponent<DragUI>().BeginScale(Pivot, otherController.Pivot);
                otherController.SetLineRendererColor(scaleColor);
                SetLineRendererColor(scaleColor);
                SetPointerActive(true);

                PlaySound(SoundManager.Instance.beginGrab);
            }
            else // senao, segue a acao de grab
            {
                currentObjectHold.gameObject.GetComponent<DragUI>().BeginDrag(pivot);
                SetLineRendererColor(grabColor);
                SetPointerActive(true);
                pointer.transform.position = hitPoint;

                PlaySound(SoundManager.Instance.beginGrab);
            }
        }
        /*
        if (Equals(currentObjectHold.gameObject.layer, LayerMask.NameToLayer("InteractableObject")))
        {
            InteractableObject instance = currentObjectHold.GetComponent<InteractableObject>();

            if (instance.Type == InteractableObject.ObjectType.Animation)
            {
                AnimationController controller = instance.GetComponent<AnimationController>();

                controller.ResetAnimation(false);
            }
        }
        */
        Debug.Log("OnRightIndexTriggerDown 2");
    }

    public void SetPointerActive(bool newValue)
    {
        pointer.SetActive(newValue);
    }

    public Transform ObjectHold { get => currentObjectHold; set => currentObjectHold = value; }

    public Transform Pivot { get => pivot; }

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
