using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestViewManager : MonoBehaviour
{
    [SerializeField] private Transform viewParent;
    [SerializeField] private GameObject requestViewPrefab;

    public void Reset()
    {
        Utils.ClearChilds(viewParent);
    }

    public GameObject InstantiateRequestView()
    {
        return Instantiate(requestViewPrefab, viewParent);
    }
}
