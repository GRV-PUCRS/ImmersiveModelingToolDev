using UnityEngine;
using System.Collections.Generic;

public class DownloadViaPhoton : Singleton<DownloadViaPhoton>
{
    [Header("Photon Config")]
    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject receiverPrefab;


}