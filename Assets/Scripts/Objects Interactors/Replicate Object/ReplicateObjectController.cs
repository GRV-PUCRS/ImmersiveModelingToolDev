using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateObjectController : MonoBehaviour, IShowInterface
{
    [SerializeField] private ReplicateObjectActionUIController _uiController;

    private List<GameObject> _elements;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            _uiController.Complete();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            _uiController.UpdateUI(7, 5);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            _uiController.UpdateUI(20, 13);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Cancel();
        }
    }

    public void ApplyAction(List<GameObject> elements)
    {
        _elements = elements;

        var plan = OculusManager.Instance.TaskManager.GetPlan();

        _uiController.UpdateUI(plan.Tasks.Count, plan.CurrentTaskIndex);
    }

    public void Confirm(List<int> stepsToAddObjects)
    {
        var taskManager = OculusManager.Instance.TaskManager;
        var currentTaskIndex = taskManager.GetPlan().CurrentTaskIndex;

        foreach (int stepIndex in stepsToAddObjects)
        {
            if (currentTaskIndex == stepIndex) continue;

            foreach (var element in _elements)
            {
                taskManager.AddObjectInTask(stepIndex, element.transform);
            }
        }

        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}
