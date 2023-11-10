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
        
        [SerializeField] private Button _startArButton;
        [SerializeField] private Button _startNormalButton;
        private Button _currentTriggerButton, _theOtherButton;

        private void OnEnable() => BluetoothDelegateManager.scanDeviceResultEvent += GetScanResult;

        private void OnDisable() => BluetoothDelegateManager.scanDeviceResultEvent -= GetScanResult;
        
        private enum DemoMode
        {
            /// <summary>
            /// 一般的
            /// </summary>
            NORMAL_MODE = 1,
            /// <summary>
            /// AR
            /// </summary>
            AR_MODE = 2
        }
        
        private DemoMode _demoModeEnum;

        private void Start()
        {
            _demoModeEnum = DemoMode.NORMAL_MODE;
            
            InitButtonAndUi();
        }
        
        /// <summary>
        /// Init buttons
        /// </summary>
        private void InitButtonAndUi()
        {
            _scanResultText.text = "";
            _currentTriggerButton = null;
            _theOtherButton = null;
        
            _startArButton.interactable = true;
            _startArButton.GetComponent<ScanButtonUi>().SetLabelAndIcon(Color.white);
            _startArButton.onClick.AddListener(() => { StartToScanDevice(_startArButton,_startNormalButton,  DemoMode.AR_MODE); });
        
            _startNormalButton.interactable = true;
            _startNormalButton.GetComponent<ScanButtonUi>().SetLabelAndIcon(Color.white);
            _startNormalButton.onClick.AddListener(() => { StartToScanDevice(_startNormalButton, _startArButton, DemoMode.NORMAL_MODE); });
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void StartToScanDevice(Button aCurrentSelectedButton, Button aTheOtherButton, DemoMode aDemoMode)
        {
            _scanResultText.text = "Searching...";

            _demoModeEnum = aDemoMode;
            aCurrentSelectedButton.interactable = false;
            aTheOtherButton.interactable = false;
            aCurrentSelectedButton.GetComponent<ScanButtonUi>().SetLabelAndIcon(new Color(0f, 1f, 1f, 1f));
            _currentTriggerButton = aCurrentSelectedButton;
            _theOtherButton = aTheOtherButton;
        
            _bluetoothManager.ScanBluetoothDevices();
        }

        /// <summary>
        /// 快去AR場景
        /// </summary>
        private void GoToDemoScene() => SceneManager.LoadScene((int)_demoModeEnum);
        public void GoToSceneTo(int aSceneIndex) => SceneManager.LoadScene(aSceneIndex);

        private void GetScanResult(bool aResult)
        {
            _scanResultText.text = aResult ? "Device Found" : $"Please make sure device is connected correctly,{Environment.NewLine}and turn on the power";
        
            if (aResult)
            {
                _currentTriggerButton.GetComponent<ScanButtonUi>().SetIcon(new Color(0f, 1f, 1f, 1f));
                _currentTriggerButton.interactable = false;
                _theOtherButton.interactable = false;
                _currentTriggerButton = null;
                _theOtherButton = null;
                Invoke(nameof(GoToDemoScene), 1f);
            }
            else
            {
                _currentTriggerButton.GetComponent<ScanButtonUi>().SetLabelAndIcon(Color.white);
                _currentTriggerButton.interactable = true;
                _theOtherButton.interactable = true;
                _currentTriggerButton = null;
                _theOtherButton = null;
            }
        }
    }
}


