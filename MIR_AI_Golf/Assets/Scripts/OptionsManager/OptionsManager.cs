using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using EthanLin.AndroidBluetoothLib;
using EthanLin.CalibrateHelper;
using EthanLin.Variation;
using EthanLin.Config;

namespace EthanLin
{
    public class OptionsManager : MonoBehaviour
    {
        // [SerializeField] private Text _debugText;
        
        [SerializeField] private BluetoothManager _bluetoothManager;
        
        [SerializeField] private VectorVariationManager _vectorVariationManager;
        
        [SerializeField] private StickHelper _stickHelper;
        
        [SerializeField] private Animator _optionsPageAnimator, _quitDialogueAnimator;
        
        [SerializeField] private Dropdown _dropdown;
        
        [Header("BGM")] [SerializeField] private AudioSource _bgmAudioSource;
        
        [Header("STICK按鈕文字")] [SerializeField] private Text _stickButtonText;
        
        /// <summary>
        /// RecordMotion按鈕
        /// </summary>
        [Header("RecordMotion按鈕")] [SerializeField] private Button _recordMotionButton;
        /// <summary>
        /// RecordMotion按鈕是否display
        /// </summary>
        private bool _isRecordPostureButtonOn = false;
        
        /// <summary>
        /// PIP RawImage
        /// </summary>
        [SerializeField] private GameObject _PipRawImage;
        
        /// <summary>
        /// is options page opened ?
        /// </summary>
        private bool _isOptionsOn = false;

        [SerializeField] private Button _turnPageButton;
        /// <summary>
        /// 選項分頁
        /// </summary>
        [Header("選項分頁")][SerializeField] private GameObject[] _optionsPages;
        /// <summary>
        /// 當前頁要
        /// </summary>
        private int _currentPage = 0;

        #region 有關Variation Config設定
        
        /// <summary>
        /// 當前選擇的部位
        /// </summary>
        private int _currentSelectBodyPart = 0;
        [Header("Use Variation")][SerializeField] private Toggle _useVariationToggle;
        [Header("Only Upper Body")][SerializeField] private Toggle _onlyUpperBodyToggle;
        [Header("Threshold")][SerializeField] private Slider _thresholdSlider;
        [Header("Discount")][SerializeField] private Slider _discountSlider;
        [Header("Back To RawData")][SerializeField] private Slider _backToRawDataSlider;
        [Header("Threshold Value")][SerializeField] private Text _thresholdValueText;
        [Header("Discount Value")][SerializeField] private Text _discountValueText;
        [Header("Back To RawData Value")][SerializeField] private Text _backToRawDataValueText;

        #endregion

        #region 有關胸、腰最後微調的

        /// <summary>
        /// 胸 Pitch
        /// </summary>
        [Header("胸 Pitch")][SerializeField] private Slider _chestPitchSlider;
        /// <summary>
        /// 胸 Pitch value
        /// </summary>
        [Header("胸 Pitch value")][SerializeField] private Text _chestPitchValueText;
        /// <summary>
        /// 胸 Roll
        /// </summary>
        [Header("胸 Roll")][SerializeField] private Slider _chestRollSlider;
        /// <summary>
        /// 胸 Roll value
        /// </summary>
        [Header("胸 Roll value")][SerializeField] private Text _chestRollValueText;

        #endregion
        
        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            _isOptionsOn = false;
            _turnPageButton.interactable = _isOptionsOn;
            
            _PipRawImage.SetActive(false);

            InitDropDownMenu();
            GetVariationConfigInfo();

            SetChestPelvisSliderToSetPitchRollYaw();
            
            _currentPage = 0;
        }

        /// <summary>
        /// 開啟或關閉Options頁面
        /// </summary>
        public void OpenCloseOptions()
        {
            _isOptionsOn = !_isOptionsOn;
            _turnPageButton.interactable = _isOptionsOn;
            
            _optionsPageAnimator.Play(_isOptionsOn ? "Opening" : "Closing");
            
            if (_isOptionsOn == false)
            {
                foreach (var page in _optionsPages)
                {
                    page.transform.localScale = Vector3.zero;
                }
                _currentPage = 0;
                _optionsPages[_currentPage].transform.localScale = Vector3.one;
            }

            // Stick按鈕的字變成黃色
            // _stickButtonText.color = _stickHelper.IsSticked ? Color.yellow : Color.white;
            _stickButtonText.color = _stickHelper.GetIsUsingNormalMode ? Color.yellow : Color.white;
        }

        /// <summary>
        /// Init dropdown menu
        /// </summary>
        private void InitDropDownMenu()
        {
            List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
            optionDataList.Add(new Dropdown.OptionData("CHEST")); // 3
            optionDataList.Add(new Dropdown.OptionData("LEFT UPPER ARM")); // 4
            optionDataList.Add(new Dropdown.OptionData("RIGHT UPPER ARM")); // 2
            optionDataList.Add(new Dropdown.OptionData("LEFT FOREARM")); // 5
            optionDataList.Add(new Dropdown.OptionData("RIGHT FOREARM")); // 1
            optionDataList.Add(new Dropdown.OptionData("PELVIS")); // 8
            optionDataList.Add(new Dropdown.OptionData("LEFT THIGH")); // 7
            optionDataList.Add(new Dropdown.OptionData("RIGHT THIGH")); // 9
            optionDataList.Add(new Dropdown.OptionData("LEFT CALF")); // 6
            optionDataList.Add(new Dropdown.OptionData("RIGHT CALF")); // 10

            _dropdown.options = optionDataList;
            _dropdown.value = 0;
            _dropdown.onValueChanged.AddListener((int aIndex) => { SelectPartToSetVariation(_dropdown.value); });
        }

        private void SelectPartToSetVariation(int aIndex)
        {
            switch (aIndex)
            {
                case 0:
                    // 胸
                    _currentSelectBodyPart = AllPartNameIndex.CHEST;
                    break;
                
                case 1:
                    // 左上臂
                    _currentSelectBodyPart = AllPartNameIndex.LEFT_UPPER_ARM;
                    break;
                
                case 2:
                    // 右上臂
                    _currentSelectBodyPart = AllPartNameIndex.RIGHT_UPPER_ARM;
                    break;
                
                case 3:
                    // 左前臂
                    _currentSelectBodyPart = AllPartNameIndex.LEFT_FOREARM;
                    break;
                
                case 4:
                    // 右前臂
                    _currentSelectBodyPart = AllPartNameIndex.RIGHT_FOREARM;
                    break;
                
                case 5:
                    // 臀
                    _currentSelectBodyPart = AllPartNameIndex.PELVIS;
                    break;
                
                case 6:
                    // 左大腿
                    _currentSelectBodyPart = AllPartNameIndex.LEFT_THIGH;
                    break;
                
                case 7:
                    // 右大腿
                    _currentSelectBodyPart = AllPartNameIndex.RIGHT_THIGH;
                    break;
                
                case 8:
                    // 左小腿
                    _currentSelectBodyPart = AllPartNameIndex.LEFT_CALF;
                    break;
                
                case 9:
                    // 右小腿
                    _currentSelectBodyPart = AllPartNameIndex.RIGHT_CALF;
                    break;
            }

            SetAllSliderWithCurrentSelectedBodyPart();
        }
        
        private void SetAllSliderWithCurrentSelectedBodyPart()
        {
            _thresholdSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyThreshold;
            _thresholdValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyThreshold}";
            
            _discountSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyDiscount;
            _discountValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyDiscount}";
            
            _backToRawDataSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyBackToRawData;
            _backToRawDataValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[_currentSelectBodyPart].bodyBackToRawData}";
        }

        
        private void GetVariationConfigInfo()
        {
            #region 有關Variation Config設定

            _currentSelectBodyPart = AllPartNameIndex.CHEST;
            
            _useVariationToggle.isOn = _vectorVariationManager.GetVariationConfig.IsUsingVariation;
            _onlyUpperBodyToggle.isOn = _vectorVariationManager.GetVariationConfig.GetOnlyUpperBody;

            _thresholdSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold;
            _thresholdValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold}";
            
            _discountSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount;
            _discountValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount}";
            
            _backToRawDataSlider.value = _vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData;
            _backToRawDataValueText.text = $"{_vectorVariationManager.GetVariationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData}";

            #endregion

            #region 有關胸、腰最後微調的

            _chestPitchSlider.value = _vectorVariationManager.GetVariationConfig.chestPitchAdjustValue;
            _chestPitchValueText.text = $"{_vectorVariationManager.GetVariationConfig.chestPitchAdjustValue}";

            _chestRollSlider.value = _vectorVariationManager.GetVariationConfig.chestRollAdjustValue;
            _chestRollValueText.text = $"{_vectorVariationManager.GetVariationConfig.chestRollAdjustValue}";

            #endregion
        }

        private void SetChestPelvisSliderToSetPitchRollYaw()
        {
            _chestPitchSlider.onValueChanged.AddListener((float aValue) =>
            {
                _vectorVariationManager.GetVariationConfig.chestPitchAdjustValue = aValue;
                // _chestPitchValueText.text = aValue.ToString("0.00");
                _chestPitchValueText.text = string.Format("{0:f2}°", aValue);
            });
            _chestRollSlider.onValueChanged.AddListener((float aValue) =>
            {
                _vectorVariationManager.GetVariationConfig.chestRollAdjustValue = aValue;
                // _chestRollValueText.text = aValue.ToString("0.00");
                _chestRollValueText.text = string.Format("{0:f2}°", aValue);
            });
        }

        public void TurnPage()
        {
            foreach (var page in _optionsPages)
            {
                page.transform.localScale = Vector3.zero;
            }
            
            _currentPage++;
            if (_currentPage == _optionsPages.Length)
            {
                _currentPage = 0;
            }
            
            _optionsPages[_currentPage].transform.localScale = Vector3.one;
        }

        #region Toggle 功能

        /// <summary>
        /// turn Record Posture Button on/off
        /// </summary>
        public void SetRecordPostureButtonToggle(bool aIsOn)
        {
            _isRecordPostureButtonOn = aIsOn;
            _recordMotionButton.transform.gameObject.SetActive(_isRecordPostureButtonOn);
        }

        /// <summary>
        /// 設定是否只用上半身
        /// </summary>
        /// <param name="aIsOnly">true: 只有上半身</param>
        public void SetOnlyUpper(bool aIsOnly) => _vectorVariationManager.GetVariationConfig.SetOnlyUpperBody(aIsOnly);

        /// <summary>
        /// 設定是否使用Variation
        /// </summary>
        /// <param name="aUsing">true: 使用Variation</param>
        public void SetUseVariation(bool aUsing) => _vectorVariationManager.GetVariationConfig.SetIsUsingVariation(aUsing);

        public void SetBackgroundMusicOnOff(bool aEnable) => _bgmAudioSource.enabled = aEnable;

        #endregion
        
        
        
        /// <summary>
        /// PIP on / off
        /// </summary>
        public void TurnOnOffPip() => _PipRawImage.SetActive(!_PipRawImage.activeInHierarchy);
        
        /// <summary>
        /// return to start up scene
        /// </summary>
        public void ReturnStartUpScene()
        {
            _bluetoothManager.DisconnectAllBluetoothDevice();
            SceneManager.LoadScene(0);
        }
        
        public void RequestQuitApp(bool aPop) => _quitDialogueAnimator.Play(aPop ? "OpeningDialogue" : "ClosingDialogue");

        public void QuitThisApp() => Application.Quit();
    }
}