using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExternalDataListener
{
    public void ReceivedData(string data);
}
