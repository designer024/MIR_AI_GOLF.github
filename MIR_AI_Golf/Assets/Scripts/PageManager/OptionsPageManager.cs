using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using EthanLin.Config;
using EthanLin.AndroidBluetoothLib;
using EthanLin.Variation;
using EthanLin.AssignDataHelper;

namespace EthanLin
{
    public class OptionsPageManager : MonoBehaviour
    {
        [Tooltip("1: Normal,\n2: AR")]
        [Header("場景Index")] [SerializeField] private int _sceneIndex;
        
        [SerializeField] private BluetoothManager _bluetoothManager;
        [SerializeField] private VariationConfig _variationConfig;
        [Header("目前只用在Normal場景")] [SerializeField] private BallBallConfigHelperV2 _ballBallConfigHelperV2;
        [SerializeField] private ChestAndPelvisFinalAdjustHelper _chestAndPelvisFinalAdjustHelper;
        [SerializeField] private AlwaysFaceRole _alwaysFaceRole;
        
        [Header("目前只用在Normal場景")] [SerializeField] private RoleSelectHelper _roleSelectHelper;
        
        /// <summary>
        /// BGM
        /// </summary>
        [Header("BGM")] [SerializeField] private AudioSource _bgmAudioSource;
        /// <summary>
        /// 按鈕音效
        /// </summary>
        [Header("按鈕音效")] [SerializeField] private AudioSource _buttonClickAudioSource;
        
        /// <summary>
        /// 選項頁面
        /// </summary>
        [Header("選項頁面")] [SerializeField] private GameObject _optionsPage;
        private bool _isOptionsPageOn;

        /// <summary>
        /// 隱藏的按鈕
        /// </summary>
        [SerializeField] private Button _hiddenButton;
        /// <summary>
        /// 選項頁面之分頁
        /// </summary>
        [Header("選項頁面之分頁")] [SerializeField] private GameObject[] _optionsSubPages;
        
        /// <summary>
        /// 對話框頁面
        /// </summary>
        [Header("對話框頁面")] [SerializeField] private GameObject _dialoguePage;
        
        /// <summary>
        /// PIP RawImage
        /// </summary>
        [Header("PIP RawImage AR場景用的")] [SerializeField] private GameObject _PipRawImage, _PipRawImage_Rawdata, _dragMixedImage;
        /// <summary>
        /// PIP RawImage, 0: 沒有, 1: 只有彩色,2: 彩色和黑白
        /// </summary>
        private int _pipState = 0;
        /// <summary>
        /// PIP 按鈕文字 
        /// </summary>
        [Header("PIP 按鈕文字 AR場景用的")] [SerializeField] private Text _pipButtonLabel;
        
        /// <summary>
        /// Role Mixed
        /// </summary>
        [Header("Role Mixed Normal場景用的")] [SerializeField] private GameObject _roleMixedObject;

        [Header("胸與腰最後調整")] [SerializeField] private GameObject _chestAndPelvisFinalAdjustPanel;

        [Header("是否顯示黑白郎君")] [SerializeField] private Button _isRawDataRoleShowButton;

        #region Toggles

        [Header("BGM")] [SerializeField] private Toggle _bgmToggle;
        [Header("按鈕音效")] [SerializeField] private Toggle _buttonClickToggle;
        [Header("只有上半身")] [SerializeField] private Toggle _onlyUpperBodyToggle;
        [Header("使用Variation")] [SerializeField] private Toggle _usingVariationToggle;
        [Header("旋轉角色用的")] [SerializeField] private Toggle _rotateRoleToggle;
        
        [Header("使用球球")] [SerializeField] private Toggle _usingBallBallToggle;
        [Header("胸與腰最後調整")] [SerializeField] private Toggle _chestAndPelvisFinalAdjustToggle;

        #endregion
        
        private void Start()
        {
            _pipState = 0;
            
            if (_sceneIndex == 1)
            {
                _roleMixedObject.transform.position = new Vector3(100f, 0f, 0f);
            }
            
            InitOptionsPageUi();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (CurrentPageIs.CURRENT_PAGE == CurrentPageIs.MAIN_PAGE)
                {
                    // InitOptionsPageUi();
                    OpenDialoguePageDirectly();
                }
                else if (CurrentPageIs.CURRENT_PAGE == CurrentPageIs.OPTIONS_PAGE)
                {
                    CloseOptionsPage();
                }
                else if (CurrentPageIs.CURRENT_PAGE == CurrentPageIs.DIALOG_PAGE)
                {
                    CloseDialoguePage();
                }
                else
                {
                    InitOptionsPageUi();
                }
            }
        }

        /// <summary>
        /// Init options page ui
        /// </summary>
        private void InitOptionsPageUi()
        {
            CurrentPageIs.CURRENT_PAGE = CurrentPageIs.MAIN_PAGE;

            _chestAndPelvisFinalAdjustToggle.isOn = false;
            _chestAndPelvisFinalAdjustPanel.SetActive(_chestAndPelvisFinalAdjustToggle.isOn);
            
            _isOptionsPageOn = false;
            _optionsPage.transform.localScale = Vector3.zero;
            _dialoguePage.transform.localScale = Vector3.zero;

            _hiddenButton.interactable = false;
            _optionsSubPages[0].transform.localScale = Vector3.one;
            _optionsSubPages[1].transform.localScale = Vector3.zero;

            if (_sceneIndex == 2)
            {
                _PipRawImage.SetActive(false);
                _PipRawImage_Rawdata.SetActive(false);
                _dragMixedImage.SetActive(false);
                _pipButtonLabel.color = Color.white;
            }
        }
        
        /// <summary>
        /// 在主畫面直接開啟對話框頁面
        /// </summary>
        private void OpenDialoguePageDirectly()
        {
            _dialoguePage.transform.localScale = Vector3.one;
            _optionsPage.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                _isOptionsPageOn = true;
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.DIALOG_PAGE;
            });
        }

        /// <summary>
        /// 開啟選項頁面
        /// </summary>
        public void OpenOptionsPage()
        {
            _optionsPage.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                _isOptionsPageOn = true;
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.OPTIONS_PAGE;

                _hiddenButton.interactable = true;
                
                _bgmToggle.isOn = _bgmAudioSource.volume > 0f;
                _buttonClickToggle.isOn = _buttonClickAudioSource.volume > 0f;
                _onlyUpperBodyToggle.isOn = _variationConfig.GetOnlyUpperBody;
                _usingVariationToggle.isOn = _variationConfig.GetIsUsingVariation;
                if (_sceneIndex == 1)
                {
                    _usingBallBallToggle.isOn = _ballBallConfigHelperV2.GetIsUsingBallBall;
                }
                else if (_sceneIndex == 2)
                {
                    _pipButtonLabel.color = _PipRawImage.activeInHierarchy ? Color.yellow : Color.white;
                }
            });
        }
        
        /// <summary>
        /// 關閉選項頁面
        /// </summary>
        public void CloseOptionsPage()
        {
            _hiddenButton.interactable = false;
            _optionsPage.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                _isOptionsPageOn = false;
                _optionsSubPages[0].transform.localScale = Vector3.one;
                _optionsSubPages[1].transform.localScale = Vector3.zero;
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.MAIN_PAGE;
            });
        }

        /// <summary>
        /// 開啟對話框頁面
        /// </summary>
        public void OpenDialoguePage()
        {
            _hiddenButton.interactable = false;
            _dialoguePage.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.DIALOG_PAGE;
            });
        }
        
        /// <summary>
        /// 關閉對話框頁面
        /// </summary>
        public void CloseDialoguePage()
        {
            _optionsSubPages[0].transform.localScale = Vector3.one;
            _optionsSubPages[1].transform.localScale = Vector3.zero;
            _dialoguePage.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                _hiddenButton.interactable = true;
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.OPTIONS_PAGE;
            });
        }

        /// <summary>
        /// 真的回去StartUp場景
        /// </summary>
        public void ConfirmReturnToStartUpScene()
        {
            _bluetoothManager.DisconnectAllBluetoothDevice();
            SceneManager.LoadScene(0);
        }


        /// <summary>
        /// AR場景使用 PIP on / off
        /// </summary>
        public void TurnOnOffPip()
        {
            _pipState++;
            if (_pipState == 3)
            {
                _pipState = 0;
            }
            
            _PipRawImage.SetActive(_pipState > 0);
            _PipRawImage_Rawdata.SetActive(_pipState == 2);
            _dragMixedImage.SetActive(_PipRawImage.activeInHierarchy);
            _pipButtonLabel.color = _pipState switch
            {
                0 => Color.white,
                1 => Color.yellow,
                2 => Color.green,
                _ => Color.white
            };

            // _PipRawImage.SetActive(!_PipRawImage.activeInHierarchy);
            // _PipRawImage_Rawdata.SetActive(_PipRawImage.activeInHierarchy);
            // _dragMixedImage.SetActive(_PipRawImage.activeInHierarchy);
            // _pipButtonLabel.color = _PipRawImage.activeInHierarchy ? Color.yellow : Color.white;
        }
        
        public void GotoNormalScene() => SceneManager.LoadScene(1);
        public void GotoAR_Scene() => SceneManager.LoadScene(2);

        public void TurnSubPage()
        {
            _optionsSubPages[0].transform.localScale = _optionsSubPages[0].transform.localScale == Vector3.one ? Vector3.zero : Vector3.one;
            _optionsSubPages[1].transform.localScale = _optionsSubPages[0].transform.localScale == Vector3.one ? Vector3.zero : Vector3.one;
        }

        /// <summary>
        /// 目前只用在Normal場景, 開啟或關閉RawData
        /// </summary>
        public void SetIsRawDataRoleShow()
        {
            if (_sceneIndex == 1)
            {
                bool curValue = _variationConfig.GetIsRawDataRoleShow;
                _variationConfig.SetIsRawDataRoleShow(!curValue);
                _isRawDataRoleShowButton.transform.GetChild(0).GetComponent<RawImage>().color = _variationConfig.GetIsRawDataRoleShow ? Color.white : new Color(1f, 1f, 1f, 0.3f);
            }
        }
        
        #region Toggles 功能

        public void SetOnlyUpperBody(bool aOnlyUpperBody)
        {
            _bluetoothManager.SwitchUpperOrFullBody(aOnlyUpperBody);
            _variationConfig.SetOnlyUpperBody(aOnlyUpperBody);

            if (_sceneIndex == 1)
            {
                AssignOnlyUpperBody assignOnlyUpperBody = GameObject.FindWithTag("Role").GetComponent<AssignOnlyUpperBody>();
                if (assignOnlyUpperBody != null)
                {
                    assignOnlyUpperBody.SetBodyIsFullOrOnlyUpper(aOnlyUpperBody);
                }
            }
            else if (_sceneIndex == 2)
            {
                AssignOnlyUpperBody assignOnlyUpperBody = GameObject.FindWithTag("RoleMixed").transform.GetChild(AllConfigs.CURRENT_SELECTED_ROLE_INDEX).GetComponent<AssignOnlyUpperBody>();
                if (assignOnlyUpperBody != null)
                {
                    assignOnlyUpperBody.SetBodyIsFullOrOnlyUpper(aOnlyUpperBody);
                }
            }
        }
        public void SetIsUsingVariation(bool aIsUsingVariation) => _variationConfig.SetIsUsingVariation(aIsUsingVariation);

        public void SetBgmOnOff(bool aIsBgmOn) => _bgmAudioSource.volume = aIsBgmOn ? 0.2f : 0f;
        
        public void SetButtonSoundOnOff(bool aIsOn) => _buttonClickAudioSource.volume = aIsOn ? 1f : 0f;

        public void SetIsUsingBallBall(bool aIsUsingBallBall)
        {
            _ballBallConfigHelperV2.SetIsUsingBallBall(aIsUsingBallBall);

            AssignDataToRoleHelper assignDataToRoleHelper = GameObject.FindWithTag("Role").GetComponent<AssignDataToRoleHelper>();

            if (assignDataToRoleHelper != null)
            {
                if (aIsUsingBallBall == false)
                {
                    assignDataToRoleHelper.ToldMeToBack();
                    _roleMixedObject.transform.position = new Vector3(100f, 0f, 0f);
                }
                else
                {
                    assignDataToRoleHelper.ToldMeToGoFarAway();
                    _roleMixedObject.transform.position = _roleSelectHelper.GetColorfulRoleStandingPoint.transform.position;
                }
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG}, AssignDataToRoleHelper is null!");
            }
        }

        public void SetChestAndPelvisFinalAdjustPanelOnOff(bool aIsOn)
        {
            _chestAndPelvisFinalAdjustPanel.SetActive(aIsOn);
            if (aIsOn)
            {
                _chestAndPelvisFinalAdjustHelper.GetValues();
            }
            CloseOptionsPage();
        }

        /// <summary>
        /// 開啟或關閉 rotate role slider
        /// </summary>
        public void SetRotateRoleSliderActive(bool aActive) => _alwaysFaceRole.SetRotateRoleSliderActive(aActive);

        #endregion
    }
}