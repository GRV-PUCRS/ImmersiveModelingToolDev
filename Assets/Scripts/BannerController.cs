using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BannerController : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private TextMeshProUGUI _txtID; 
    [SerializeField] private TextMeshProUGUI _txtDescription;
    [SerializeField] private ObjectSelector _leftController;
    [SerializeField] private ObjectSelector _rightController;

    private bool _isLeftController;

    private void Awake()
    {
        _view.SetActive(false);
    }

    public void ActiveBanner(bool isleft, GameObject obj, bool isHighlighted)
    {
        if (isHighlighted)
        {
            if (!OculusManager.Instance.IsEditMode || !obj.gameObject.layer.Equals(LayerMask.NameToLayer("StoredObject"))) return;

            if (obj.TryGetComponent<DragUI>(out var dragUI)) ActiveBanner(dragUI);
            if (obj.TryGetComponent<PainelController>(out var painel)) ActiveBanner(painel);

            _isLeftController = isleft;
        }
        else
        {
            DeactivateBanner(isleft);
        }

    }

    public void ActiveBannerWhenHolderAction(bool isleft, GameObject obj, bool isHighlighted)
    {
        if (!(_leftController.WithActionHolder || _rightController.WithActionHolder)) return;

        if (isHighlighted)
        {
            if (!OculusManager.Instance.IsEditMode || !obj.gameObject.layer.Equals(LayerMask.NameToLayer("StoredObject"))) return;

            if (obj.TryGetComponent<DragUI>(out var dragUI)) ActiveBanner(dragUI);
            if (obj.TryGetComponent<PainelController>(out var painel)) ActiveBanner(painel);

            _isLeftController = isleft;
        }
        else
        {
            DeactivateBanner(isleft);
        }
    }

    public void ActiveBanner(DragUI obj)
    {
        ActiveBanner(obj.ID, obj.Description);
    }

    public void ActiveBanner(PainelController painel)
    {
        ActiveBanner(painel.ID, painel.Description);
    }

    public void ActiveBanner(string id, string description)
    {
        _txtID.text = id;
        _txtDescription.text = description;

        _view.SetActive(id.Length != 0);
        _txtDescription.gameObject.SetActive(description.Length != 0);
    }

    public void DeactivateBanner(bool isLeft)
    {
        if (isLeft != _isLeftController) return;

        _view.SetActive(false);
    }

    public void OnHoldeActionChange(bool isLeftController, bool isActionActive)
    {
        if (!isActionActive) _view.SetActive(false);
    }
}