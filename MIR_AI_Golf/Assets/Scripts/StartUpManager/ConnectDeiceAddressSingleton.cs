using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectDeiceAddressSingleton
{
    private static ConnectDeiceAddressSingleton _instance;

    public static ConnectDeiceAddressSingleton GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ConnectDeiceAddressSingleton();
        }

        return _instance;
    }

    public string CONNECT_DEVICE_ADDRESS = "";
}
