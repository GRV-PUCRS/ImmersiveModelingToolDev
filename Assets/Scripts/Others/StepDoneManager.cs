using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepDoneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject view;
    [SerializeField] private Transform handTransform;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private Slider sliderProgress;

    [Header("Configuration")]
    [SerializeField] private float timeToCompleteStep = 1f;

    private float currentTime = 0f;
    private bool poseDetected = false;

    private void Start()
    {
        transform.SetParent(handTransform);
        transform.position = transform.position - transform.forward * 0.5f;
    }

    private void Update()
    {
        if (poseDetected)
        {
            float timeDiff = Time.time - currentTime;

            if (timeDiff >= timeToCompleteStep)
            {
                CompleteStep();
                OnThumbsUpUnselected();
            }
            else
            {
                sliderProgress.value = timeDiff / timeToCompleteStep;
            }
        }
    }

    public void CompleteStep()
    {
        taskManager.CompleteTask(true);
    }

    public void OnThumbsUpSelected()
    {
        poseDetected = true;
        currentTime = Time.time;

        view.SetActive(true);
    }

    public void OnThumbsUpUnselected()
    {
        poseDetected = false;

        view.SetActive(false);
    }

    private void OnEnable()
    {
        HandTrackingManager.Instance.OnThumbsUpSelected.AddListener(OnThumbsUpSelected);   
        HandTrackingManager.Instance.OnThumbsUpUnselected.AddListener(OnThumbsUpUnselected);
    }

    private void OnDisable()
    {
        HandTrackingManager.Instance.OnThumbsUpSelected.RemoveListener(OnThumbsUpSelected);
        HandTrackingManager.Instance.OnThumbsUpUnselected.RemoveListener(OnThumbsUpUnselected);
    }
}
