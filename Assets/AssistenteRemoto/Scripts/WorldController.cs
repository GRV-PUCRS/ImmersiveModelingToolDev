using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Tag
{
    public string name;
    public Transform reference;
}

public class WorldController : MonoBehaviour
{
    public float offset = 0.1f;
    public Transform objectTagReference;
    public Transform camerasConteiner;
    public Transform environmentTransform;
    public Transform trackablesParent;
    public GameObject spherePrefab;

    [SerializeField] private List<Tag> envTags = new List<Tag>();
    [SerializeField] private List<Tag> objTags = new List<Tag>();
    [SerializeField] private Transform userReference;
    [SerializeField] private Transform mainCameraTransform;

    private Dictionary<string, Transform> interactableObjects = new Dictionary<string, Transform>();
    private Vector3 fixedAngle = Vector3.zero;
    private Vector3 fixedPosition = Vector3.zero;
    private Vector3 lastCameraPosition = Vector3.zero;
    private Vector3 lastCameraRotation = Vector3.zero;
    private Vector3 newRotation;
    private Transform interactableObjectsContainer;
    private Transform lastCameraView;

    public float updateObjectDelay = 0.2f;
    private float updateObjectCount = 0.2f;

    // Message
    // message[0] = Name        (string)
    // message[1] = Position    (Vector3)
    // message[2] = EulerAngle  (Vector3)
    object[] message = new object[4];

    private void Awake()
    {
        interactableObjectsContainer = transform.Find("InteractableObjectsContainer").GetComponent<Transform>();

        camerasConteiner.parent = environmentTransform;
    }

    private void Update()
    {

    }

    // Position the User in the world instance
    public void SetUserPosition(Vector3 _position, Vector3 _eulerAngle)
    {
        
        //Debug.Log($"{_position} {fixedPosition} {lastCameraPosition} {_position - fixedPosition}");
        mainCameraTransform.localPosition = _position - fixedPosition;
        newRotation = _eulerAngle;// - fixedAngle;
        newRotation.z = 0;

        mainCameraTransform.localRotation = Quaternion.Euler(newRotation);
        lastCameraPosition = _position;
        lastCameraRotation = _eulerAngle;
        
    }

    // Set User in front of Env tag founded
    public void SetUserReference(string _name, Vector3 _tagPosition)
    {
        // Save last camera position values
        fixedPosition = lastCameraPosition;
        fixedAngle = new Vector3(0, lastCameraRotation.y, 0);
        offset = Vector3.Distance(lastCameraPosition, _tagPosition);
        Debug.Log(lastCameraPosition + " " + _tagPosition + " " + offset);
        // Find the env tag
        Tag tagReference = envTags.Find(t => t.name == _name);

        if (tagReference.Equals(default(Tag))) return;

        //transform.rotation = Quaternion.identity;
        //transform.RotateAround(tagReference.reference.position, Vector3.up, -lastCameraRotation.y);
        userReference.position = tagReference.reference.position;
        //userReference.rotation = Quaternion.Euler(tagReference.reference.rotation.eulerAngles + new Vector3(0, 180, 0));
        userReference.rotation = Quaternion.Euler(new Vector3(0, -fixedAngle.y, 0));
        userReference.position -= userReference.forward * offset;

        Debug.Log("User Reference posicionado");
        SetClientOrigin();
    }

    public void SetWorldPosition(Vector3 newPosition, Vector3 newEulerAngleRotation)
    {
        environmentTransform.position = newPosition;
        environmentTransform.rotation = Quaternion.Euler(newEulerAngleRotation);
    }
    public void SetUserCameraPosition(Vector3 newPosition, Vector3 newEulerAngleRotation)
    {
        mainCameraTransform.position = newPosition;
        mainCameraTransform.rotation = Quaternion.Euler(newEulerAngleRotation);
    }

    public void SetObjectPosition(string _name, Vector3 _tagPosition, Vector3 _tagEulerAngle)
    {
        Tag objectTag = objTags.Find(t => t.name == _name);

        if (objectTag.Equals(default(Tag))) return;

        //objectTag.reference.position = mainCameraTransform.position + mainCameraTransform.forward * Vector3.Distance(lastCameraPosition, _tagPosition);
        objectTag.reference.localPosition = _tagPosition;
    }

    private void SetClientOrigin()
    {
        // Testing
        //message[1] = transform.position - mainCameraTransform.position;
        message[1] = environmentTransform.position - userReference.position;
        //message[2] = userReference.rotation.eulerAngles;
        message[2] = offset;
        
        EventManager.TriggerSendMessageRequest(Events.UPDATE_ORIGIN, message);
    }

    public void CreateInteractableObject()
    {
        string objectName = interactableObjects.Count.ToString();

        if (!interactableObjects.ContainsKey(objectName))
        {
            GameObject instance = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);
            instance.transform.parent = trackablesParent;

            interactableObjects.Add(objectName, instance.transform.transform);
            interactableObjects[objectName].name = objectName;
            interactableObjects[objectName].position = lastCameraView.position + lastCameraView.forward * .5f;


            message[0] = objectName;
            message[1] = interactableObjects[objectName].localPosition;
            message[2] = interactableObjects[objectName].localRotation.eulerAngles;

            EventManager.TriggerSendMessageRequest(Events.CREATE_OBJECT, message);
        }
    }

    private void OnEnable()
    {
        EventManager.OnCameraViewChange += OnCameraViewChange;
    }

    private void OnDisable()
    {
        EventManager.OnCameraViewChange -= OnCameraViewChange;
    }

    private void OnCameraViewChange(Camera cameraView)
    {
        lastCameraView = cameraView.transform;
    }
}
