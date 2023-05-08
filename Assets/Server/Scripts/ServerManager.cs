using UnityEngine;

public class ServerManager : Singleton<ServerManager>
{
    // Instance of the Environment
    [SerializeField] private WorldController worldController;


    // Handle position and rotate of device camera
    public void HandleUpdateCamera(object[] data)
    {
        worldController.SetUserCameraPosition((Vector3)data[2], (Vector3)data[3]);
    }

    // Handle tag tracked from Client
    public void HandleUpdateTag(object[] data)
    {
        worldController.SetWorldPosition((Vector3)data[2], (Vector3)data[3]);
    }
}
