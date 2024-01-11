using System.Collections;
using System.Collections.Generic;
using EthanLin;
using UnityEngine;
using UnityEngine.UI;

public class ConnectDeviceButtonBehaviour : MonoBehaviour
{
    private IConnectDevice mConnectDeviceCallback;
    
    [SerializeField] private Text _deviceNameText;
    private string _deviceName;
    private string _deviceAddress;
    
    private void Start()
    {
        if (mConnectDeviceCallback == null)
        {
            mConnectDeviceCallback = GameObject.FindWithTag("StartUpManager").GetComponent<StartUpManagerV2>();
        }
    }
    
    private void Update()
    {
        
    }

    public void SetDeviceNameToText(string aDeviceName, string aDeviceAddress)
    {
        _deviceName = aDeviceName;
        _deviceAddress = aDeviceAddress;
        _deviceNameText.text = _deviceName;
    }

    public void StartToShow(int aSceneIndex)
    {
        if (mConnectDeviceCallback != null)
        {
            mConnectDeviceCallback.OnConnectDeviceAndGotoScene(aSceneIndex, _deviceAddress);
        }
    }
}
