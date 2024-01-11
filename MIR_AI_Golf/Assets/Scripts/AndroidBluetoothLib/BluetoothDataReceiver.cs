using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using EthanLin.AssignDataHelper;

namespace EthanLin.AndroidBluetoothLib
{
    public class BluetoothDataReceiver : MonoBehaviour
    {
        [SerializeField] private BluetoothManager _bluetoothManager;

        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;

        
        private string _dataString;

        private void Start()
        {
            Invoke(nameof(ConnectBluetoothDevice), 1f);
        }
    
        /// <summary>
        /// 連接至藍牙裝置
        /// </summary>
        // private void ConnectBluetoothDevice() => _bluetoothManager.ConnectBluetoothDevice();
        private void ConnectBluetoothDevice() => _bluetoothManager.ConnectBluetoothDevice(ConnectDeiceAddressSingleton.GetInstance().CONNECT_DEVICE_ADDRESS);

        /// <summary>
        /// Called by android library 在AR場景使用
        /// </summary>
        /// <param name="aDataString"></param>
        public void ReceiveDataFromAndroidNative(string aDataString)
        {
            if (!string.IsNullOrEmpty(aDataString))
            {
                _dataString = aDataString;
                _dataStringToQuaternionHelper.ConvertDataStringToQuaternion(_dataString);
            }
        }
    }
}


