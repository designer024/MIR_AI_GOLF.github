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
        [SerializeField] private int _sceneIndex;
        
        [SerializeField] private BluetoothManager _bluetoothManager;
        [SerializeField] private VariationConfig _variationConfig;
        
        [Header("BGM")] [SerializeField] private AudioSource _bgmAudioSource;
        [Header("按鈕音效")] [SerializeField] private AudioSource _buttonClickAudioSource;
        
        [Header("選項頁面")] [SerializeField] private GameObject _optionsPage;
        private bool _isOptionsPageOn;
        
        [Header("對話框頁面")] [SerializeField] private GameObject _dialoguePage;
        
        /// <summary>
        /// PIP RawImage
        /// </summary>
        [Header("PIP RawImage")] [SerializeField] private GameObject _PipRawImage;

        #region Toggles

        [Header("BGM")] [SerializeField] private Toggle _bgmToggle;
        [Header("按鈕音效")] [SerializeField] private Toggle _buttonClickToggle;
        [Header("只有上半身")] [SerializeField] private Toggle _onlyUpperBodyToggle;
        [Header("使用Variation")] [SerializeField] private Toggle _usingVariationToggle;

        #endregion
        
        private void Start()
        {
            InitOptionsPageUi();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (CurrentPageIs.CURRENT_PAGE == CurrentPageIs.MAIN_PAGE)
                {
                    InitOptionsPageUi();
                    // OpenDialoguePageDirectly();
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
            
            _isOptionsPageOn = false;
            _optionsPage.transform.localScale = Vector3.zero;
            _dialoguePage.transform.localScale = Vector3.zero;
            
            _PipRawImage.SetActive(false);
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

                _bgmToggle.isOn = _bgmAudioSource.volume > 0f;
                _buttonClickToggle.isOn = _buttonClickAudioSource.volume > 0f;
                _onlyUpperBodyToggle.isOn = _variationConfig.GetOnlyUpperBody;
                _usingVariationToggle.isOn = _variationConfig.GetIsUsingVariation;
            });
        }
        
        /// <summary>
        /// 關閉選項頁面
        /// </summary>
        public void CloseOptionsPage()
        {
            _optionsPage.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                _isOptionsPageOn = false;
                CurrentPageIs.CURRENT_PAGE = CurrentPageIs.MAIN_PAGE;
            });
        }

        /// <summary>
        /// 開啟對話框頁面
        /// </summary>
        public void OpenDialoguePage()
        {
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
            _dialoguePage.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
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
        /// PIP on / off
        /// </summary>
        public void TurnOnOffPip() => _PipRawImage.SetActive(!_PipRawImage.activeInHierarchy);
        
        public void GotoNormalScene() => SceneManager.LoadScene(1);
        public void GotoAR_Scene() => SceneManager.LoadScene(2);
        
        #region Toggles 功能

        public void SetOnlyUpperBody(bool aOnlyUpperBody)
        {
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

        #endregion
    }
}