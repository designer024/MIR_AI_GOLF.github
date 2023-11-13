using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EthanLin.AndroidBluetoothLib
{
    public class BluetoothManager : MonoBehaviour
    {
        private BluetoothHelper _bluetoothHelper;
        public BluetoothHelper GetBluetoothHelper => _bluetoothHelper;

        [SerializeField] private BluetoothDelegateManager _bluetoothDelegateManager;

        /// <summary>
        /// 是否有找到裝置
        /// </summary>
        private bool _isFoundDevice = false;

        #region Unity 生命週期

        private void OnEnable()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _bluetoothHelper = new BluetoothHelper();
            }
        }

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                RequestBluetoothPermission();

                InitBluetoothManager();
            
                RequestManageExternalPermission();
            }
        }

        private void OnApplicationQuit()
        {
            _bluetoothHelper.DisconnectAllBluetoothDevice();
        }

        #endregion

        #region private 方法

        /// <summary>
        /// 請求藍牙權限
        /// </summary>
        private void RequestBluetoothPermission() => _bluetoothHelper.RequestBluetoothPermission();

        /// <summary>
        /// 請求管理存取權限
        /// </summary>
        private void RequestManageExternalPermission() => _bluetoothHelper.RequestManageExternalPermission();

        /// <summary>
        /// Init BluetoothManager 
        /// </summary>
        private void InitBluetoothManager() => _bluetoothHelper.InitBluetoothManager();

        private void StopScan()
        {
            CancelInvoke();
            _bluetoothDelegateManager.UpdateScanResult(_isFoundDevice);
            _bluetoothHelper.StopScan();
        }

        #endregion

        #region public 方法

        public void ScanBluetoothDevices()
        {
            _isFoundDevice = false;
            _bluetoothHelper.ScanBluetoothDevices();
            Invoke(nameof(StopScan), 6f);
        }
        
        public void SelectBluetoothToConnect(int aIndex)
        {
            ConnectBluetoothDevice(aIndex);
            Debug.Log($"EthanLinBluetoothDataLib select item {aIndex}");
        }
        
        /// <summary>
        /// 連接裝置 with list index: 0
        /// </summary>
        public void ConnectBluetoothDevice() => _bluetoothHelper.ConnectBluetoothDevice(0);
        
        /// <summary>
        /// 連接裝置 with list index
        /// </summary>
        /// <param name="aIndex">清單的index</param>
        public void ConnectBluetoothDevice(int aIndex) => _bluetoothHelper.ConnectBluetoothDevice(aIndex);
        
        /// <summary>
        /// 斷開所有藍牙裝置
        /// </summary>
        public void DisconnectAllBluetoothDevice() => _bluetoothHelper.DisconnectAllBluetoothDevice();

        /// <summary>
        /// 是否只用上半身
        /// </summary>
        /// <param name="aOnlyUpper"> true: 只用上半身</param>
        public void SwitchUpperOrFullBody(bool aOnlyUpper) => _bluetoothHelper.SwitchUpperOrFullBody(aOnlyUpper);

        /// <summary>
        /// Called by android library 在StartUp場景使用
        /// </summary>
        public void ReceiveMessageFromAndroidNative(string aNativeMessage)
        {
            if (aNativeMessage.Equals("onScanResult"))
            {
                _isFoundDevice = true;
                StopScan();
            }
        }

        #endregion
    }
}


