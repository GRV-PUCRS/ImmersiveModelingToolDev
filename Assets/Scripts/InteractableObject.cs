using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] private ObjectType objType = ObjectType.Model;
    public enum ObjectType { Model, Painel, Animation };

    public ObjectType Type { get => objType; }
}
