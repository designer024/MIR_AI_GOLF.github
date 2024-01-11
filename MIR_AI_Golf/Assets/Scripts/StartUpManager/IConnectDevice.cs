using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectDevice
{
    void OnConnectDeviceAndGotoScene(int aSceneIndex, string aDeviceAddress);
}
