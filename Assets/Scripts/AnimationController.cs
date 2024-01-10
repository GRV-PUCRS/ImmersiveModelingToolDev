using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animation animation;
    private object[] message = new object[2];
    public enum AnimationOperation { Play, Pause, Reset };

    private void Awake()
    {
        Debug.Log(name);
        animation = GetComponentInChildren<Animation>();
    }

    private void OnEnable()
    {
        EventManager.OnAnimationEvent += OnAnimationEvent;
    }

    private void OnDisable()
    {
        EventManager.OnAnimationEvent -= OnAnimationEvent;
    }

    public void OnAnimationEvent(object[] receivedMessage)
    {
        Debug.Log($"--> {(string)receivedMessage[0]}");
        if ((string)receivedMessage[0] != name) return;

        switch ((AnimationOperation)receivedMessage[1])
        {
            case AnimationOperation.Play:
                PlayAnimation(false);

                break;

            case AnimationOperation.Pause:
                PauseAnimation(false);

                break;

            case AnimationOperation.Reset:
                ResetAnimation(false);

                break;
        }
    }

    public void PlayAnimation(bool updatePlayer)
    {
        animation[animation.clip.name].speed = 1;
        
        if (!animation.isPlaying)
            animation.Play();

        if (updatePlayer)
            SendAnimationOperatio(AnimationOperation.Play);
    }

    public void PauseAnimation(bool updatePlayer)
    {
        animation[animation.clip.name].speed = 0;

        if (updatePlayer)
            SendAnimationOperatio(AnimationOperation.Pause);
    }

    public void ResetAnimation(bool updatePlayer)
    {
        animation.Rewind();
        animation[animation.clip.name].speed = 0;

        if (updatePlayer)
            SendAnimationOperatio(AnimationOperation.Reset);
    }

    private void SendAnimationOperatio(AnimationOperation op)
    {
        message[0] = name;
        message[1] = op;

        EventManager.TriggerSendMessageRequest(Events.ANIMATION_EVENT, message);
    }
}
