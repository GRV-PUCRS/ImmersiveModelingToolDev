using System;
using System.Collections.Generic;
using UnityEngine;
using static Plan;

public static class EventManager
{
    public static event Action<bool> OnSocketConnectionChange;
    public static event Action<Transform> OnObjectSelected;
    public static event Action<byte, object[]> OnSendMessageRequest;
    public static event Action<byte, object[]> OnSendMessageToServerRequest;
    public static event Action<Camera> OnCameraViewChange;
    public static event Action<int, int> OnScreenSizeChange;
    public static event Action OnResetSelecteObject;
    public static event Action<object[]> OnPainelEvent;
    public static event Action<object[]> OnAnimationEvent;

    public static event Action<ObjectSelector, DragUI> OnObjectDragBegin;
    public static event Action<ObjectSelector, DragUI> OnObjectDragEnd;

    public static event Action<Transform, Transform, Transform> OnObjectScaleBegin;
    public static event Action<Transform> OnObjectScaleEnd;

    public static event Action<DragUI> OnObjectDuplicated;
    public static event Action<DragUI> OnObjectReplicated;
    public static event Action<DragUI> OnObjectSetPersistent;
    public static event Action<DragUI, Transform> OnJoinedObject;


    public static event Action OnStageChange;
    public static event Action<bool> OnToggleObjectVisibility;
    public static event Action<GameObject> OnObjectDeleted;

    public static event Action<bool> OnVRModeActive;

    public static event Action<RequestViewController> OnRequestViewCreated;

    public static event Action<Plan> OnTaskChanged;

    public static event Action<Transform> OnOriginChange;

    public static event Action OnEnterSession;
    public static event Action OnExitSession;

    public static event Action<bool> OnExecModeChange;

    public static void TriggerSocketConnectionStatus(bool isConnected)
    {
        OnSocketConnectionChange?.Invoke(isConnected);
    }

    public static void TriggerObjectSelected(Transform transform)
    {
        OnObjectSelected?.Invoke(transform);
    }

    public static void TriggerSendMessageRequest(byte code, object[] message)
    {
        OnSendMessageRequest?.Invoke(code, message);
    }

    public static void TriggerSendMessageToServerRequest(byte code, object[] message)
    {
        OnSendMessageToServerRequest?.Invoke(code, message);
    }

    public static void TriggerCameraViewChange(Camera camera)
    {
        OnCameraViewChange?.Invoke(camera);
    }

    public static void TriggerScreenSizeChange(int newWidth, int newHeight)
    {
        OnScreenSizeChange?.Invoke(newWidth, newHeight);
    }

    public static void TriggerResetSelectedObject()
    {
        OnResetSelecteObject?.Invoke();
    }

    public static void TriggerPainelEvent(object[] message)
    {
        OnPainelEvent?.Invoke(message);
    }
    public static void TriggerAnimationEvent(object[] message)
    {
        OnAnimationEvent?.Invoke(message);
    }

    public static void TriggerDragBegin(ObjectSelector controller, DragUI obj) { OnObjectDragBegin?.Invoke(controller, obj); }
    public static void TriggerDragEnd(ObjectSelector controller, DragUI obj) { OnObjectDragEnd?.Invoke(controller, obj); }

    public static void TriggerScaleBegin(Transform origin, Transform pivot1, Transform pivot2) { OnObjectScaleBegin?.Invoke(origin, pivot1, pivot2); }
    public static void TriggerScaleEnd(Transform origin) { OnObjectScaleEnd?.Invoke(origin); }

    public static void TriggerObjectDuplicated(DragUI obj) { OnObjectDuplicated?.Invoke(obj); }
    public static void TriggerObjectReplicated(DragUI obj) { OnObjectReplicated?.Invoke(obj); }
    public static void TriggerObjectSetPersistent(DragUI obj) { OnObjectSetPersistent?.Invoke(obj); }

    public static void TriggerJoinedObject(DragUI obj, Transform oldParent) => OnJoinedObject?.Invoke(obj, oldParent);

    public static void TriggerStageChange() { OnStageChange?.Invoke(); }
    public static void TriggerToggleObjectVisibility(bool newValue) { OnToggleObjectVisibility?.Invoke(newValue); }
    public static void TriggerRequestViewCreated(RequestViewController requestView) { OnRequestViewCreated?.Invoke(requestView); }
    public static void TriggerObjectDeleted(GameObject deletedObject) { OnObjectDeleted?.Invoke(deletedObject); }

    public static void TriggerVRModeActive(bool newValue) { OnVRModeActive?.Invoke(newValue); }

    public static void TriggerTaskChanged(Plan currentPlan) { OnTaskChanged?.Invoke(currentPlan); }

    public static void TriggerOriginChange(Transform origin) { OnOriginChange?.Invoke(origin); }
    public static void TriggerEnterSession() { OnEnterSession?.Invoke(); }
    public static void TriggerExitSession() { OnExitSession?.Invoke(); }
    public static void TriggerExecModeChange(bool isEditMode) => OnExecModeChange?.Invoke(isEditMode);
}