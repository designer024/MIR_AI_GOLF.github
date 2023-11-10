using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using EthanLin.AndroidBluetoothLib;

namespace EthanLin
{
    public class StartUpManager : MonoBehaviour
    {
        [SerializeField] private BluetoothManager _bluetoothManager;
        
        [SerializeField] private  Text _scanResultText;
        
        [SerializeField] private Button _startButton;
        [SerializeField] private int _defaultSceneIndex;

        private void OnEnable() => BluetoothDelegateManager.scanDeviceResultEvent += GetScanResult;

        private void OnDisable() => BluetoothDelegateManager.scanDeviceResultEvent -= GetScanResult;

        private void Start()
        {
            // _scanResultText.text = $"{Screen.width}x{Screen.height}";
            _scanResultText.text = "";
            _startButton.interactable = true;
            _startButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            _startButton.onClick.AddListener(StartToScanDevice);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void StartToScanDevice()
        {
            _scanResultText.color = Color.white;
            _scanResultText.text = "             Searching...";
            _startButton.interactable = false;
            _startButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.3f);
            _bluetoothManager.ScanBluetoothDevices();
        }

        /// <summary>
        /// 快去AR場景
        /// </summary>
        private void GoToDemoScene() => SceneManager.LoadScene(_defaultSceneIndex);
        public void GoToSceneTo(int aSceneIndex) => SceneManager.LoadScene(aSceneIndex);

        private void GetScanResult(bool aResult)
        {
            _scanResultText.color = aResult ? Color.white : Color.yellow;
            _scanResultText.text = aResult ? "             Device Found" : "Please make sure the device is connected correctly and turn on the power";
            
            if (aResult)
            {
                _startButton.interactable = false;
                _startButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.3f);
                Invoke(nameof(GoToDemoScene), 1f);
            }
            else
            {
                _startButton.interactable = true;
                _startButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            }
        }
    }
}


