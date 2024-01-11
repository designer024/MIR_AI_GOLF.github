using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using EthanLin.AndroidBluetoothLib;
using EthanLin.Config;

namespace EthanLin
{
    public class StartUpManagerV2 : MonoBehaviour, IConnectDevice
    {
        [SerializeField] private BluetoothManager _bluetoothManager;
        [SerializeField] private SoundManager _soundManager;

        [SerializeField] private Button _scanButton;
        [SerializeField] private Text _scanButtonLabel;
        
        [SerializeField] private Text _scanResultText;


        [SerializeField] private GameObject _connectDeviceButtonPrefab;
        [SerializeField] private Transform _connectDeviceButtonParentTransform;
        
        private void Start()
        {
            _scanResultText.text = "";
            DestroyAllDeviceButton();
        }
        
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void StartToScanDevice()
        {
            _scanResultText.text = "Searching...";
            
            _scanButton.interactable = false;
            _scanButtonLabel.color = new Color(1f, 1f, 1f, 0.3f);
            
            _bluetoothManager.ScanBluetoothDevicesV2();
        }

        /// <summary>
        /// Called by Android Native
        /// </summary>
        public void receiveMessageFromAndroidNative(string aValue)
        {
            Debug.Log($"{AllConfigs.DEBUG_TAG}, 共找到了{aValue}個");
            if (!aValue.Equals("0"))
            {
                _scanResultText.text = "Device Found";
                
                DestroyAllDeviceButton();
                
                int totalSize = int.Parse(aValue);
                for (int i = 0; i < totalSize; ++i)
                {
                    AndroidJavaObject device = _bluetoothManager.GetBluetoothHelper.deviceListJavaObject.Call<AndroidJavaObject>("get", i);
                    string deviceName = _bluetoothManager.GetBluetoothHelper.GetDeviceName(device);
                    string deviceAddress = _bluetoothManager.GetBluetoothHelper.GetDeviceAddress(device);
                    // Debug.Log($"EthanLinUnityWiFiP2pDebug, from Unity deviceName: {deviceName}, deviceAddress: {deviceAddress}");

                    GameObject deviceButton = Instantiate(_connectDeviceButtonPrefab, _connectDeviceButtonParentTransform);
                    deviceButton.GetComponent<ConnectDeviceButtonBehaviour>().SetDeviceNameToText(deviceName, deviceAddress);
                }
            }
            else
            {
                _scanResultText.text = $"Please make sure device is connected correctly,{System.Environment.NewLine}and turn on the power";
                _scanButton.interactable = true;
                _scanButtonLabel.color = Color.white;
            }
        }
        
        private void DestroyAllDeviceButton()
        {
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("ConnectDeviceButton");
            if (buttons != null && buttons.Length > 0)
            {
                foreach (var btn in buttons)
                {
                    Destroy(btn);
                }
            }
        }

        /// <summary>
        /// 實做interface
        /// </summary>
        /// <param name="aSceneIndex">scene index</param>
        public void OnConnectDeviceAndGotoScene(int aSceneIndex, string aDeviceAddress)
        {
            Debug.Log($"{AllConfigs.DEBUG_TAG}, scene: {aSceneIndex}, address: {aDeviceAddress}");
            
            _soundManager.PlayTheSound();
                
            ConnectDeiceAddressSingleton.GetInstance().CONNECT_DEVICE_ADDRESS = aDeviceAddress;
            SceneManager.LoadScene(aSceneIndex);
        }
    }
}

