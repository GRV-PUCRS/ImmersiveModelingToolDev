using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Interactable,
    Flag
}

public class Events
{
    public enum FILE_TRANSFER_EVENT_CODE { REQUEST=0, UPLOAD, INFO, FILE, CONFIRMATION }
    public enum FILE_SEARCH_EVENT_CODE { REQUEST=0, RESPONSE }

    public const byte UPDATE_TAG = 0;
    public const byte UPDATE_CAMERA = 1;
    public const byte UPDATE_OBJECT = 2;
    public const byte UPDATE_ORIGIN = 3;
    public const byte UPDATE_FRAME = 4;
    public const byte CREATE_OBJECT = 5;
    public const byte DELETE_OBJECT = 6;
    public const byte ACTION_OBJECT = 7;
    public const byte HIDE_OBJECT = 8;
    public const byte SHOW_OBJECT = 9;
    public const byte UPDATE_ARCAMERA = 10;
    public const byte RESET_ARCAMERA = 11;
    public const byte PAINEL_EVENT = 12;
    public const byte ANIMATION_EVENT = 13;
    public const byte FILE_TRANSFER_EVENT = 14;
    public const byte FILE_SEARCH_EVENT = 15;
    public const byte OBJECT_TRACKER_EVENT = 16;
}
