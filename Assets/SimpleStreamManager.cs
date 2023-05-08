using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStreamManager : MonoBehaviour
{
    public Transform world;
    public Transform camera;

    public void SetOrigin(object[] message)
    {
        world.position = (Vector3)message[2];
        world.rotation = Quaternion.Euler((Vector3)message[3]);
    }

    public void SetCameraPosition(object[] message)
    {
        camera.position = (Vector3)message[2];
        camera.rotation = Quaternion.Euler((Vector3)message[3]);
    }
}
