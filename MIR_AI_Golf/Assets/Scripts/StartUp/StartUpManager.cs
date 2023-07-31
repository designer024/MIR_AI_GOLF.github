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
        
        [SerializeField] private Animator _quitDialogueAnimator;

        private void OnEnable() => BluetoothDelegateManager.scanDeviceResultEvent += GetScanResult;

        private void OnDisable() => BluetoothDelegateManager.scanDeviceResultEvent -= GetScanResult;

        private void Start()
        {
            _scanResultText.text = "";
            _startButton.interactable = true;
            _startButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            _startButton.onClick.AddListener(StartToScanDevice);
        }

        private void StartToScanDevice()
        {
            _scanResultText.text = "";
            _startButton.interactable = false;
            _startButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.3f);
            _bluetoothManager.ScanBluetoothDevices();
        }

        /// <summary>
        /// 開啟或關閉退出APP的對話框
        /// </summary>
        public void RequestToQuitApp(bool aPop) => _quitDialogueAnimator.Play(aPop ? "Opening" : "Closing");

        /// <summary>
        /// 快去AR場景
        /// </summary>
        private void GoToARScene() => SceneManager.LoadScene(1);
        public void GoToSceneTo(int aSceneIndex) => SceneManager.LoadScene(aSceneIndex);

        public void QuitThisApp() => Application.Quit();

        private void GetScanResult(bool aResult)
        {
            _scanResultText.text = aResult ? "             Device Found" : "Please make sure the device is connected correctly and turn on the power";
            
            if (aResult)
            {
                _startButton.interactable = false;
                _startButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.3f);
                Invoke(nameof(GoToARScene), 1f);
            }
            else
            {
                _startButton.interactable = true;
                _startButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            }
        }
    }
}


