
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ClientManager : MonoBehaviour
{
    public Transform mainCamera;
    public GameObject tagPrefab;
    public ClientObjectManager objectManager;

    public float updateTagRotationDelay = 5f;
    public float updateAnchorPositionDelay = 5f;
    private float updateTagRotationTimer = 1f;
    private float updateAnchorPositionTimer = 1f;

    private ARTrackedImageManager arTrackedImageManager;
    private ARAnchorManager arAnchorManager;

    // Mensagens
    object[] message = new object[3] { null, null, null };

    //private Gyroscope gyro;

    private Dictionary<string, ARAnchor> anchorDetected = new Dictionary<string, ARAnchor>();

    private void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
    }

    private void Update()
    {
        message[1] = mainCamera.position;
        message[2] = mainCamera.rotation.eulerAngles;

        /*ClientUIManager.Instance.infoText.text = $"Origin PG {transform.position}\n" +
            $"Origin RG {transform.rotation.eulerAngles}\n" +
            $"Camera PG {mainCamera.position}\n" +
            $"Camera PL {mainCamera.localPosition}\n" +
            $"Camera RG {mainCamera.rotation.eulerAngles}\n" +
            $"Camera RL {mainCamera.localRotation.eulerAngles}";*/

        //FMSocketIOManager.instance.SendToServer(JsonUtility.ToJson(message));
        ClientNetwork.Instance.SendPositionMessageToServer(Events.UPDATE_CAMERA, message);
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs _args)
    {
        foreach (ARTrackedImage image in _args.added)
        {
            AddImage(image);
        }

        foreach (ARTrackedImage image in _args.updated)
        {
            UpdateImage(image);
        }
    }

    private void AddImage(ARTrackedImage _image)
    {
        Logger.Log($"Imagem adicionada: {_image.referenceImage.name}");
        anchorDetected.Add(_image.referenceImage.name, null);
    }

    private void TryCreateAnchor(ARTrackedImage _image)
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
        Pose pose = new Pose(_image.transform.position, _image.transform.rotation);
        ARAnchor anchor = arAnchorManager.AddAnchor(pose);

        if (anchor != null)
        {
            anchorDetected[_image.referenceImage.name] = anchor;
            Logger.Log("Ancora adicionada com sucesso");
        }
        else
        {
            Logger.Log("Erro ao adicionar a ancora");
        }
    }

    private void UpdateImage(ARTrackedImage image)
    {
        //Logger.Log($"{_image.trackingState.ToString()}/{_image.trackingState} - {UnityEngine.XR.ARSubsystems.TrackingState.Tracking} = {_image.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking}");
        if (image.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            if (anchorDetected[image.referenceImage.name] == null)
            {
                TryCreateAnchor(image);
            }

            if (image.referenceImage.name.Contains("ENV"))
            {
                updateTagRotationTimer += Time.deltaTime;
                
                if (updateTagRotationTimer >= updateTagRotationDelay)
                {
                    updateTagRotationTimer = 0f;

                    anchorDetected[image.referenceImage.name].transform.position = image.transform.position;
                    anchorDetected[image.referenceImage.name].transform.rotation = image.transform.rotation;
                    //objectManager.userReference.position = anchorDetected[image.referenceImage.name].transform.position;
                    //Logger.Log($"Anchor {_image.referenceImage.name} : {anchorDetected[_image.referenceImage.name].transform.position} / {objectManager.userReference.position}");
                }

            }else if (image.referenceImage.name.Contains("OBJ"))
            {
                updateAnchorPositionTimer += Time.deltaTime;

                if (updateAnchorPositionTimer >= updateAnchorPositionDelay)
                {
                    updateAnchorPositionTimer = 0f;
                    
                    anchorDetected[image.referenceImage.name].transform.position = image.transform.position;
                    anchorDetected[image.referenceImage.name].transform.rotation = image.transform.rotation;
                    //objectManager.userReference.position = anchorDetected[image.referenceImage.name].transform.position;
                    
                    tagPrefab.SetActive(true);
                    //Logger.Log($"Anchor OBJ {image.referenceImage.name} : {anchorDetected[image.referenceImage.name].transform.position} / {objectManager.userReference.position}");
                }
            }

            message[0] = image.referenceImage.name;
            //message[1] = image.transform.position - objectManager.origin.position;
            message[1] = image.transform.position;
            message[2] = image.transform.rotation.eulerAngles;

            //FMSocketIOManager.instance.SendToServer(JsonUtility.ToJson(message));
            ClientNetwork.Instance.SendPositionMessageToServer(Events.UPDATE_TAG, message);

            /*
            tagPrefab.SetActive(true);
            tagPrefab.transform.position = _image.transform.position;
            tagPrefab.transform.rotation = _image.transform.rotation;
            tagPrefab.transform.localScale = new Vector3(_image.size.x, 0.001f, _image.size.y);
            */
        }
        else
        {
            //tagPrefab.SetActive(false);
        }
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
}
