using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorController : MonoBehaviour, IInteractor
{
    [SerializeField] private GameObject interactor;
    [SerializeField] private bool isPersistent;

    public void OnInstantiated()
    {
        GameObject sceneElement = new GameObject("Scene Element");
        GameObject instance = Instantiate(interactor, sceneElement.transform);
        instance.transform.position = transform.position;

        Vector3 direction = InputController.Instance.HMD.position - transform.position;
        instance.transform.LookAt(InputController.Instance.HMD);

        if (isPersistent)
        {
            OculusManager.Instance.TaskManager.AddPersistentObject(sceneElement.transform);
        }
        else
        {
            OculusManager.Instance.TaskManager.AddObjectInTask(sceneElement.transform);
        }

        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return null;

        Destroy(gameObject);
    }
}
