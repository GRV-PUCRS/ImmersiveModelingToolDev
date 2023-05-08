using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] private ObjectType objType = ObjectType.Model;
    [SerializeField] private int fps = 20;
    [SerializeField] private bool isActive = true;

    private float accumTime = 0f;
    private object[] message = new object[4];
    private Collider objectCollider;

    public enum ObjectType { Model, Painel, Animation };

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();

        SetActive(isActive);
    }

    void Update()
    {
        /*
        if (transform.hasChanged && isActive)
        {
            accumTime += Time.deltaTime;

            if (accumTime >= 1 / (float)fps)
            {
                accumTime = 0f;

                message[0] = transform.name;
                message[1] = transform.localPosition;
                message[2] = transform.localEulerAngles;
                message[3] = transform.localScale;

                EventManager.TriggerSendMessageRequest(Events.UPDATE_OBJECT, message);
                transform.hasChanged = false;
            }
        
        */
    }

    public void SetActive(bool newValue) 
    { 
        isActive = newValue;
        objectCollider.enabled = isActive;
    }

    public ObjectType Type { get => objType; }
}
