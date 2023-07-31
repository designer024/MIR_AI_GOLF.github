using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.AssignDataHelper;
using EthanLin.Variation;
using EthanLin.Config;

namespace EthanLin.CalibrateHelper
{
    public class StickHelper : CalibrateConfig
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;

        [SerializeField] private RawDataAssignManager _rawDataAssignManager;
        
        [SerializeField] private VectorVariationManager _vectorVariationManager;

        /// <summary>
        /// 箭頭 for 胸
        /// </summary>
        private GameObject _arrowChest_Up, _arrowChest_Forward;
        
        /// <summary>
        /// 箭頭 for 左手臂指向
        /// </summary>
        private GameObject _arrowLU_Up, _arrowLF_Up;
        
        /// <summary>
        /// 箭頭 for 右手臂指向
        /// </summary>
        private GameObject _arrowRU_Up, _arrowRF_Up;
        
        /// <summary>
        /// 箭頭 for 左手臂LookForward
        /// </summary>
        private GameObject _arrowLU_Forward, _arrowLF_Forward;
        
        /// <summary>
        /// 箭頭 for 右手臂LookForward
        /// </summary>
        private GameObject _arrowRU_Forward, _arrowRF_Forward;

        private Dictionary<int, Quaternion> _stickQuaternionDictionary = new Dictionary<int, Quaternion>();
        public Dictionary<int, Quaternion> GetStickQuaternionDictionary => _stickQuaternionDictionary;
        
        
        /// <summary>
        /// Stick按鈕文字
        /// </summary>
        [Header("Stick按鈕文字")] [SerializeField] private Text _stickButtonText;
        /// <summary>
        /// 是否已Stick
        /// </summary>
        // public bool IsSticked { private set; get; }

        private void Start()
        {
            // IsSticked = false;
            IsUsingNormalMode = false;
            
            InitQuaternionDictionary();
            
            InitAllArrowObjects();
        }

        private void OnApplicationQuit() => DestroyAllArrow();

        private void Update()
        {
            // if (IsSticked)
            if (IsUsingNormalMode)
            {
                if (_arrowChest_Up != null && _arrowChest_Forward != null && _arrowLU_Up != null && _arrowLU_Forward != null && _arrowRU_Up != null && _arrowRU_Forward != null && _arrowLF_Up != null && _arrowLF_Forward != null && _arrowRF_Up != null && _arrowRF_Forward != null)
                {
                    _arrowChest_Up.transform.position = _rawDataAssignManager.GetChestObject.transform.position;
                    _arrowChest_Forward.transform.position = _rawDataAssignManager.GetChestObject.transform.position;
                    
                    _arrowLU_Up.transform.position = _rawDataAssignManager.GetLeftUpperArmObject.transform.position;
                    _arrowLU_Forward.transform.position = _rawDataAssignManager.GetLeftUpperArmObject.transform.position;
                    
                    _arrowRU_Up.transform.position = _rawDataAssignManager.GetRightUpperArmObject.transform.position;
                    _arrowRU_Forward.transform.position = _rawDataAssignManager.GetRightUpperArmObject.transform.position;
                    
                    _arrowLF_Up.transform.position = _rawDataAssignManager.GetLeftForeArmObject.transform.position;
                    _arrowLF_Forward.transform.position = _rawDataAssignManager.GetLeftForeArmObject.transform.position;
                    
                    _arrowRF_Up.transform.position = _rawDataAssignManager.GetRightForeArmObject.transform.position;
                    _arrowRF_Forward.transform.position = _rawDataAssignManager.GetRightForeArmObject.transform.position;
                    
                    
                    Vector3 chest_up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, _arrowChest_Up.transform.up);
                    Vector3 chest_forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, _arrowChest_Forward.transform.up);
                    _stickQuaternionDictionary[AllPartNameIndex.CHEST] = Quaternion.LookRotation(chest_forward, chest_up);

                    Vector3 llluuu_up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.up, _arrowLU_Up.transform.up);
                    Vector3 llluuu_forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.forward, _arrowLU_Forward.transform.up);
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] = Quaternion.LookRotation(llluuu_forward, llluuu_up);
                    
                    Vector3 rrruuu_up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.up, _arrowRU_Up.transform.up);
                    Vector3 rrruuu_forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.forward, _arrowRU_Forward.transform.up);
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] = Quaternion.LookRotation(rrruuu_forward, rrruuu_up);

                    Vector3 lllfff_up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.up, _arrowLF_Up.transform.up);
                    Vector3 lllfff_forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.forward, _arrowLF_Forward.transform.up);
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] = Quaternion.LookRotation(lllfff_forward, lllfff_up);

                    Vector3 rrrfff_up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.up, _arrowRF_Up.transform.up);
                    Vector3 rrrfff_forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.forward, _arrowRF_Forward.transform.up);
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] = Quaternion.LookRotation(rrrfff_forward, rrrfff_up);
                }
            }
        }

        /// <summary>
        /// Stick 以立正姿式
        /// </summary>
        public void StickButtonFunctionAtAttention()
        {
            // if (IsSticked == false)
            if (IsUsingNormalMode == false)
            {
                if (_arrowChest_Up != null && _arrowChest_Forward != null && _arrowLU_Up != null && _arrowLU_Forward != null && _arrowRU_Up != null && _arrowRU_Forward != null && _arrowLF_Up != null && _arrowLF_Forward != null && _arrowRF_Up != null && _arrowRF_Forward != null)
                {
                    _stickQuaternionDictionary[AllPartNameIndex.CHEST] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST];
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM];
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM];
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM];
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM];
                
                    // 1st for chest
                    _arrowChest_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowChest_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward - Vector3.forward), Vector3.forward);
                    
                    // 2nd for left upper arm
                    _arrowLU_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowLU_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.forward - Vector3.forward), Vector3.forward);
                
                    // 3rd for right upper arm
                    _arrowRU_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowRU_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.forward - Vector3.forward), Vector3.forward);
                    
                    // 4th for left forearm
                    _arrowLF_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowLF_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.forward - Vector3.forward),Vector3.forward);
                
                    // 5th for right forearm
                    _arrowRF_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowRF_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.forward - Vector3.forward), Vector3.forward);
                
                    _arrowChest_Up.transform.SetParent(_rawDataAssignManager.GetChestObject.transform);
                    _arrowChest_Forward.transform.SetParent(_rawDataAssignManager.GetChestObject.transform);
            
                    _arrowLU_Up.transform.SetParent(_rawDataAssignManager.GetLeftUpperArmObject.transform);
                    _arrowLU_Forward.transform.SetParent(_rawDataAssignManager.GetLeftUpperArmObject.transform);
            
                    _arrowRU_Up.transform.SetParent(_rawDataAssignManager.GetRightUpperArmObject.transform);
                    _arrowRU_Forward.transform.SetParent(_rawDataAssignManager.GetRightUpperArmObject.transform);
            
                    _arrowLF_Up.transform.SetParent(_rawDataAssignManager.GetLeftForeArmObject.transform);
                    _arrowLF_Forward.transform.SetParent(_rawDataAssignManager.GetLeftForeArmObject.transform);
            
                    _arrowRF_Up.transform.SetParent(_rawDataAssignManager.GetRightForeArmObject.transform);
                    _arrowRF_Forward.transform.SetParent(_rawDataAssignManager.GetRightForeArmObject.transform);
                
                    // IsSticked = true;
                    IsUsingNormalMode = true;
                    // TakeScreenShot();
                    // Invoke(nameof(TakeScreenShot), 1f);
                }
            }
            else
            {
                _arrowChest_Up.transform.SetParent(null);
                _arrowChest_Forward.transform.SetParent(null);
            
                _arrowLU_Up.transform.SetParent(null);
                _arrowLU_Forward.transform.SetParent(null);
            
                _arrowRU_Up.transform.SetParent(null);
                _arrowRU_Forward.transform.SetParent(null);
            
                _arrowLF_Up.transform.SetParent(null);
                _arrowLF_Forward.transform.SetParent(null);
            
                _arrowRF_Up.transform.SetParent(null);
                _arrowRF_Forward.transform.SetParent(null);
                
                // IsSticked = false;
                IsUsingNormalMode = false;
            }
            
            // _stickButtonText.color = IsSticked ? Color.yellow : Color.white;
            _stickButtonText.color = IsUsingNormalMode ? Color.yellow : Color.white;
        }
        /// <summary>
        /// Stick 以Zombie姿式
        /// </summary>
        public void StickButtonFunctionBasedOnZombie()
        {
            // if (IsSticked == false)
            if (IsUsingNormalMode == false)
            {
                if (_arrowChest_Up != null && _arrowChest_Forward != null && _arrowLU_Up != null && _arrowLU_Forward != null && _arrowRU_Up != null && _arrowRU_Forward != null && _arrowLF_Up != null && _arrowLF_Forward != null && _arrowRF_Up != null && _arrowRF_Forward != null)
                {
                    _stickQuaternionDictionary[AllPartNameIndex.CHEST] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST];
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM];
                    _stickQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM];
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM];
                    _stickQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM];
                    
                    // 1st for chest
                    _arrowChest_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up - Vector3.up), Vector3.up);
                    _arrowChest_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward - Vector3.forward), Vector3.forward);
                    
                    // 2nd for left upper arm
                    _arrowLU_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.up - Vector3.back), Vector3.back);
                    _arrowLU_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.forward - Vector3.up), Vector3.up);
                
                    // 3rd for right upper arm
                    _arrowRU_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.up - Vector3.back), Vector3.back);
                    _arrowRU_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.forward - Vector3.up), Vector3.up);
                    
                    // 4th for left forearm
                    _arrowLF_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.up - Vector3.back), Vector3.back);
                    _arrowLF_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.forward - Vector3.up),Vector3.up);
                
                    // 5th for right forearm
                    _arrowRF_Up.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.up - Vector3.back), Vector3.back);
                    _arrowRF_Forward.transform.up = Vector3.ProjectOnPlane((_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.forward - Vector3.up), Vector3.up);
                    
                    _arrowChest_Up.transform.SetParent(_rawDataAssignManager.GetChestObject.transform);
                    _arrowChest_Forward.transform.SetParent(_rawDataAssignManager.GetChestObject.transform);
            
                    _arrowLU_Up.transform.SetParent(_rawDataAssignManager.GetLeftUpperArmObject.transform);
                    _arrowLU_Forward.transform.SetParent(_rawDataAssignManager.GetLeftUpperArmObject.transform);
            
                    _arrowRU_Up.transform.SetParent(_rawDataAssignManager.GetRightUpperArmObject.transform);
                    _arrowRU_Forward.transform.SetParent(_rawDataAssignManager.GetRightUpperArmObject.transform);
            
                    _arrowLF_Up.transform.SetParent(_rawDataAssignManager.GetLeftForeArmObject.transform);
                    _arrowLF_Forward.transform.SetParent(_rawDataAssignManager.GetLeftForeArmObject.transform);
            
                    _arrowRF_Up.transform.SetParent(_rawDataAssignManager.GetRightForeArmObject.transform);
                    _arrowRF_Forward.transform.SetParent(_rawDataAssignManager.GetRightForeArmObject.transform);
                }

                // IsSticked = true;
                IsUsingNormalMode = true;
                
                _vectorVariationManager.InitAllTableHelperStickHelper();
                // TakeScreenShot();
                // Invoke(nameof(TakeScreenShot), 1f);
            }
            else
            {
                _arrowChest_Up.transform.SetParent(null);
                _arrowChest_Forward.transform.SetParent(null);
            
                _arrowLU_Up.transform.SetParent(null);
                _arrowLU_Forward.transform.SetParent(null);
            
                _arrowRU_Up.transform.SetParent(null);
                _arrowRU_Forward.transform.SetParent(null);
            
                _arrowLF_Up.transform.SetParent(null);
                _arrowLF_Forward.transform.SetParent(null);
            
                _arrowRF_Up.transform.SetParent(null);
                _arrowRF_Forward.transform.SetParent(null);
                
                // IsSticked = false;
                IsUsingNormalMode = false;
                
                _vectorVariationManager.InitAllTableHelperDataStringToQuaternionHelper();
            }

            // _stickButtonText.color = IsSticked ? Color.yellow : Color.white;
            _stickButtonText.color = IsUsingNormalMode ? Color.yellow : Color.white;
        }
        
        private void InitAllArrowObjects()
        {
            _arrowChest_Up = new GameObject("Arrow_Chest_Up");
            _arrowChest_Forward = new GameObject("Arrow_Chest_Look");
            _arrowLU_Up = new GameObject("Arrow_LU_Up");
            _arrowLU_Forward = new GameObject("Arrow_LU_Look");
            _arrowRU_Up = new GameObject("Arrow_RU_Up");
            _arrowRU_Forward = new GameObject("Arrow_RU_Look");
            _arrowLF_Up = new GameObject("Arrow_LF_Up");
            _arrowLF_Forward = new GameObject("Arrow_LF_Look");
            _arrowRF_Up = new GameObject("Arrow_RF_Up");
            _arrowRF_Forward = new GameObject("Arrow_RF_Look");
        }

        private void DestroyAllArrow()
        {
            // IsSticked = false;
            IsUsingNormalMode = false;

            // arrow Chest
            if (_arrowChest_Up != null)
            {
                Destroy(_arrowChest_Up);
                _arrowChest_Up = null;
            }
            if (_arrowChest_Forward != null)
            {
                Destroy(_arrowChest_Forward);
                _arrowChest_Forward = null;
            }
            // arrow LU
            if (_arrowLU_Up != null)
            {
                Destroy(_arrowLU_Up);
                _arrowLU_Up = null;
            }
            if (_arrowLU_Forward != null)
            {
                Destroy(_arrowLU_Forward);
                _arrowLU_Forward = null;
            }
            // arrow LF
            if (_arrowLF_Up != null)
            {
                Destroy(_arrowLF_Up);
                _arrowLF_Up = null;
            }
            if (_arrowLF_Forward != null)
            {
                Destroy(_arrowLF_Forward);
                _arrowLF_Forward = null;
            }
            // arrow RU
            if (_arrowRU_Up != null)
            {
                Destroy(_arrowRU_Up);
                _arrowRU_Up = null;
            }
            if (_arrowRU_Forward != null)
            {
                Destroy(_arrowRU_Forward);
                _arrowRU_Forward = null;
            }
            // arrow RF
            if (_arrowRF_Up != null)
            {
                Destroy(_arrowRF_Up);
                _arrowRF_Up = null;
            }
        }
        
        private void InitQuaternionDictionary()
        {
            _stickQuaternionDictionary.Clear();

            for (int i = 1; i < 11; ++i)
            {
                _stickQuaternionDictionary.Add(i, Quaternion.identity);
            }
        }
        
        private void TakeScreenShot() => ScreenCapture.CaptureScreenshot($"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png");
    }
}

