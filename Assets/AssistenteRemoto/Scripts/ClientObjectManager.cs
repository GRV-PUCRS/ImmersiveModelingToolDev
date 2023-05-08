using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientObjectManager : MonoBehaviour
{
    public Transform origin;
    public Transform environment;
    public Transform mainCamera;
    public Transform trackablesParent;

    [SerializeField] private GameObject interactableObjectPrefab;
    [SerializeField] private GameObject flagPrefab;
    private Dictionary<string, Transform> interactableObjects = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> flags = new Dictionary<string, Transform>();

    private void Awake()
    {
        arrows = new List<GameObject>();
    }

    public void HandleUpdateObject(object[] message)
    {
        string objectName = (string)message[0];

        if (interactableObjects.ContainsKey(objectName))
        {
            interactableObjects[objectName].transform.localPosition = Vector3.Lerp(interactableObjects[objectName].transform.localPosition, (Vector3)message[1], 1);
        }
    }

    public void HandleCreateNewObject(object[] message)
    {
        string objectName;

        switch ((ObjectType)message[0])
        {
            case ObjectType.Flag:
                objectName = (string)message[1];

                if (!flags.ContainsKey(objectName))
                {
                    Transform flag = Instantiate(flagPrefab, trackablesParent).GetComponent<Transform>();

                    flag.name = objectName;
                    flag.localPosition = (Vector3)message[2];
                    flag.localRotation = (Quaternion)message[3];

                    flags.Add(objectName, flag);
                }

                break;
            case ObjectType.Interactable:
                string prefabName = (string)message[1];
                objectName = (string)message[2];

                if (!interactableObjects.ContainsKey(objectName))
                {
                    interactableObjects.Add(objectName, Instantiate((GameObject)Resources.Load($"Prefabs/{prefabName}"), trackablesParent).GetComponent<Transform>());
                    interactableObjects[objectName].transform.localPosition = (Vector3)message[3];

                    Logger.Log($"Objeto {objectName} criado com sucesso!");
                }
                else
                {
                    Logger.Log($"Objeto {objectName} ja existe!");
                }

                break;
        }
    }

    public void HandleDeleteObject(object[] message)
    {
        string objectName = (string)message[1];

        switch ((ObjectType)message[0])
        {
            case ObjectType.Flag:
                if (flags.ContainsKey(objectName))
                {
                    Destroy(flags[objectName].gameObject);
                    flags.Remove(objectName);
                }

                break;

            case ObjectType.Interactable:

                if (interactableObjects.ContainsKey(objectName))
                {
                    Destroy(interactableObjects[objectName]);
                    interactableObjects.Remove(objectName);

                    Logger.Log($"Objeto {objectName} removido com sucesso!");

                }
                else
                {
                    Logger.Log($"Objeto {objectName} nao encontrado!");
                }

                break;
        }
        
    }

    public void HandleUpdateOrigin(object[] message)
    {
        Logger.Log("===> " + origin.position + " " + (Vector3)message[1]);

        origin.position = mainCamera.position;
        origin.rotation = Quaternion.Euler(new Vector3(0, mainCamera.rotation.eulerAngles.y, 0));
        origin.position = origin.position + origin.forward * (float)message[2];
        environment.localPosition = (Vector3)message[1];

        //origin.rotation = Quaternion.Euler(new Vector3(0, -mainCamera.rotation.eulerAngles.y, 0));
        //origin.position = (Vector3)message[1];
        //origin.rotation = Quaternion.identity;
        //origin.rotation = Quaternion.Euler(mainCamera.rotation.eulerAngles - (Vector3)message[2]);
        //Logger.Log(origin.position + " " + (Vector3)message[1] + "  " + origin.rotation.eulerAngles + " " + (Vector3)message[2]);
    }

    public GameObject arrowPrefab;

    private List<GameObject> arrows;

    public void HandleReceiveArrowAction(object[] args)
    {
        string arrowId = (string)args[0];
        Vector3 arrowPosition = (Vector3)args[1];
        bool enableArrow = (bool)args[2];

        GameObject arrow = arrows.Find(a => a.name == arrowId);

        if (arrow == null && enableArrow)
        {
            arrow = Instantiate(arrowPrefab, trackablesParent);
            arrow.name = arrowId;
            arrow.transform.localPosition = arrowPosition;

            arrows.Add(arrow);
        }
        else if (arrow != null && !enableArrow)
        {
            Destroy(arrow.gameObject);
            arrows.Remove(arrow);
        }
    }
}
