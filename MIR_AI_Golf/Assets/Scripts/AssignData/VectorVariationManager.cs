using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.AssignDataHelper;
using EthanLin.CalibrateHelper;
using EthanLin.Config;

namespace EthanLin.Variation
{
    public class VectorVariationManager : MonoBehaviour
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        
        [SerializeField] private VariationConfig _variationConfig;
        public VariationConfig GetVariationConfig => _variationConfig;
        
        [SerializeField] private CalibrateHelper_V2 _calibrateHelperV2;
        [SerializeField] private StickHelper _stickHelper;
        
        /// <summary>
        /// For Name to Quaternion mapping
        /// </summary>
        private Dictionary<int, Quaternion> _variationQuaternionDictionary = new Dictionary<int, Quaternion>();
        /// <summary>
        /// For Name to Quaternion mapping
        /// </summary>
        public Dictionary<int, Quaternion> GetVariationQuaternionDictionary => _variationQuaternionDictionary;
        
        #region 胸修正會用到的物件

        /// <summary>
        /// RawData胸 與 Vector3.Up間的夾角
        /// </summary>
        private float _angleBetweenChestAndUp = 0f;
        /// <summary>
        /// 修正後胸的up vector
        /// </summary>
        private Vector3 _tempChestFixedVectorUp = Vector3.zero;

        #endregion

        #region 臀修正會用到的物件

        /// <summary>
        /// RawData臀 與 Vector3.Up間的夾角
        /// </summary>
        private float _angleBetweenPelvisAndUp = 0f;
        /// <summary>
        /// 修正後臀的up vector
        /// </summary>
        private Vector3 _tempPelvisFixedVectorUp = Vector3.zero;

        #endregion
        
        /// <summary>
        /// index 比照 SerialPortDataHandler, 1: 右前臂, 2: 右上臂, 3: 胸, 4: 左上臂, 5: 左前臂,
        /// 6: 左小腿, 7: 左大腿, 8: 臀, 9: 右大腿, 10: 右小腿
        /// </summary>
        private VariationHelper[] _variationHelpers;
        private Dictionary<int, VariationHelper> _variationHelperDictionary = new Dictionary<int, VariationHelper>();
        
        private void Start()
        {
            if (_dataStringToQuaternionHelper == null)
            {
                _dataStringToQuaternionHelper = GameObject.FindWithTag("DataStringToQuaternionHelper").GetComponent<DataStringToQuaternionHelper>();
            }

            if (_variationConfig == null )
            {
                _variationConfig = GameObject.FindWithTag("VectorVariationManager").GetComponent<VariationConfig>();
            }
            
            InitQuaternionDictionary();

            InitAllTableHelperDataStringToQuaternionHelper();
        }

        private void Update()
        {
            if (_dataStringToQuaternionHelper.GetBluetoothDataReceived)
            {
                // 使用Variation
                if (_variationConfig.IsUsingVariation)
                {
                    // 沒有使用任何模式之校正
                    if (_stickHelper.GetIsUsingNormalMode == false && _calibrateHelperV2.GetIsUsingAdvancedMode == false)
                    {
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, isUsing 沒有使用任何模式之校正");
                        
                        #region 上半身

                        #region 胸新的
                        
                        _angleBetweenChestAndUp = Vector3.Angle(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, Vector3.up);

                        if (_angleBetweenChestAndUp <= _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _angleBetweenChestAndUp * _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else if (_angleBetweenChestAndUp < _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData + 0.05f && _angleBetweenChestAndUp > _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData) * _angleBetweenChestAndUp + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData);
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else
                        {
                            float output = _angleBetweenChestAndUp * 1f;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }

                        Vector3 fixedChestForward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] = Quaternion.LookRotation(fixedChestForward, _tempChestFixedVectorUp);

                        #endregion

                        #region 左上臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_UPPER_ARM].GetFinalQuaternion_DataStringToQuaternionHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                        
                        #region 右上臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_UPPER_ARM].GetFinalQuaternion_DataStringToQuaternionHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                        
                        #region 左前臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_FOREARM].GetFinalQuaternion_DataStringToQuaternionHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                        
                        #region 右前臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_FOREARM].GetFinalQuaternion_DataStringToQuaternionHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion

                        #endregion

                        #region 下半身

                        #region 臀新的

                        _angleBetweenPelvisAndUp = Vector3.Angle(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, Vector3.up);

                        if (_angleBetweenPelvisAndUp <= _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold + 0.05f)
                        {
                            float output = _angleBetweenPelvisAndUp * _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold;
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }
                        else if (_angleBetweenPelvisAndUp < _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData + 0.05f && _angleBetweenPelvisAndUp > _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData + 0.05f)
                        {
                            float output = _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData) * _angleBetweenPelvisAndUp + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData);
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }
                        else
                        {
                            float output = _angleBetweenPelvisAndUp * 1f;
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }

                        Vector3 fixedPelvisForward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.forward, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up);
                        _variationQuaternionDictionary[AllPartNameIndex.PELVIS] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : Quaternion.LookRotation(fixedPelvisForward, _tempPelvisFixedVectorUp);

                        #endregion
                        
                        #region 左大腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.LEFT_THIGH].GetFinalQuaternion_DataStringToQuaternionHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 右大腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.RIGHT_THIGH].GetFinalQuaternion_DataStringToQuaternionHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 左小腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_CALF] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.LEFT_CALF].GetFinalQuaternion_DataStringToQuaternionHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 右小腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.RIGHT_CALF].GetFinalQuaternion_DataStringToQuaternionHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion

                        #endregion
                    }
                    // 有使用進階模式之校正
                    else if (_stickHelper.GetIsUsingNormalMode == false && _calibrateHelperV2.GetIsUsingAdvancedMode)
                    {
                        // 記得要先變更各個閥值、打折數、BackToRawData值
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, isUsing 有使用進階模式之校正");
                
                        #region 上半身

                        #region 胸新的
                
                        _angleBetweenChestAndUp = Vector3.Angle(_calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, Vector3.up);

                        if (_angleBetweenChestAndUp <= _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _angleBetweenChestAndUp * _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else if (_angleBetweenChestAndUp < _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData + 0.05f && _angleBetweenChestAndUp > _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData) * _angleBetweenChestAndUp + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData);
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else
                        {
                            float output = _angleBetweenChestAndUp * 1f;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }

                        Vector3 fixedChestForward = Vector3.ProjectOnPlane(_calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] = Quaternion.LookRotation(fixedChestForward, _tempChestFixedVectorUp);

                        #endregion
                
                        #region 左上臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_UPPER_ARM].GetFinalQuaternion_CalibrateHelper(
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 右上臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_UPPER_ARM].GetFinalQuaternion_CalibrateHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 左前臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_FOREARM].GetFinalQuaternion_CalibrateHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 右前臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_FOREARM].GetFinalQuaternion_CalibrateHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion  

                        #endregion

                        #region 下半身

                        #region 臀新的        
                
                        _angleBetweenPelvisAndUp = Vector3.Angle(_calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, Vector3.up);

                        if (_angleBetweenPelvisAndUp <= _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold + 0.05f)
                        {
                            float output = _angleBetweenPelvisAndUp * _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold;
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }
                        else if (_angleBetweenPelvisAndUp < _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData + 0.05f && _angleBetweenPelvisAndUp > _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData + 0.05f)
                        {
                            float output = _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData) * _angleBetweenPelvisAndUp + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.PELVIS].bodyBackToRawData);
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }
                        else
                        {
                            float output = _angleBetweenPelvisAndUp * 1f;
                            _tempPelvisFixedVectorUp = Vector3.Lerp(Vector3.up, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, (output + 0.1f) / (_angleBetweenPelvisAndUp + 0.1f));
                        }

                        Vector3 fixedPelvisForward = Vector3.ProjectOnPlane(_calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.forward, _calibrateHelperV2.GetNameToQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up);
                        
                        _variationQuaternionDictionary[AllPartNameIndex.PELVIS] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : Quaternion.LookRotation(fixedPelvisForward, _tempPelvisFixedVectorUp);

                        #endregion
                
                        #region 左大腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.LEFT_THIGH].GetFinalQuaternion_CalibrateHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 右大腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.RIGHT_THIGH].GetFinalQuaternion_CalibrateHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 左小腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_CALF] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.LEFT_CALF].GetFinalQuaternion_CalibrateHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion
                        
                        #region 右小腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] = _variationConfig.GetOnlyUpperBody ? Quaternion.identity : _variationHelperDictionary[AllPartNameIndex.RIGHT_CALF].GetFinalQuaternion_CalibrateHelper(Vector3.right, Vector3.forward, Vector3.up);

                        #endregion

                        #endregion
                    }
                    // 有使用普通模式之校正
                    else if (_stickHelper.GetIsUsingNormalMode && _calibrateHelperV2.GetIsUsingAdvancedMode == false)
                    {
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, isUsing 有使用普通模式之校正");
                        
                        #region 上半身

                        #region 胸新的
                
                        _angleBetweenChestAndUp = Vector3.Angle(_stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, Vector3.up);

                        if (_angleBetweenChestAndUp <= _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _angleBetweenChestAndUp * _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else if (_angleBetweenChestAndUp < _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData + 0.05f && _angleBetweenChestAndUp > _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold + 0.05f)
                        {
                            float output = _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData) * _angleBetweenChestAndUp + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[AllPartNameIndex.CHEST].bodyBackToRawData);
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }
                        else
                        {
                            float output = _angleBetweenChestAndUp * 1f;
                            _tempChestFixedVectorUp = Vector3.Lerp(Vector3.up, _stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, (output + 0.1f) / (_angleBetweenChestAndUp + 0.1f));
                        }

                        Vector3 fixedChestForward = Vector3.ProjectOnPlane(_stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, _stickHelper.GetStickQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] = Quaternion.LookRotation(fixedChestForward, _tempChestFixedVectorUp);

                        #endregion
                
                        #region 左上臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_UPPER_ARM].GetFinalQuaternion_StickHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 右上臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_UPPER_ARM].GetFinalQuaternion_StickHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 左前臂

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.LEFT_FOREARM].GetFinalQuaternion_StickHelper(
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                                _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion
                
                        #region 右前臂

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] =
                            _variationHelperDictionary[AllPartNameIndex.RIGHT_FOREARM].GetFinalQuaternion_StickHelper(
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.right,
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward,
                        _variationQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up);

                        #endregion

                        #endregion
                        
                        #region 下半身 暫時都用 Quaternion.identity

                        #region 臀新的        
                
                        _variationQuaternionDictionary[AllPartNameIndex.PELVIS] = Quaternion.identity;

                        #endregion
                
                        #region 左大腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] = Quaternion.identity;

                        #endregion
                
                        #region 右大腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] = Quaternion.identity;

                        #endregion
                
                        #region 左小腿

                        _variationQuaternionDictionary[AllPartNameIndex.LEFT_CALF] = Quaternion.identity;

                        #endregion
                
                        #region 右小腿

                        _variationQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] = Quaternion.identity;

                        #endregion

                        #endregion
                    }
                }
                // 不用Variation
                else
                {
                    // 沒有使用任何模式之校正
                    if (_stickHelper.GetIsUsingNormalMode == false && _calibrateHelperV2.GetIsUsingAdvancedMode == false)
                    {
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, NoUsing 只用RawData");
                        
                        for (int i = 1; i < 11; ++i)
                        {
                            _variationQuaternionDictionary[i] = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i];
                        }
                    }
                    // 有使用進階模式之校正
                    else if (_stickHelper.GetIsUsingNormalMode == false && _calibrateHelperV2.GetIsUsingAdvancedMode)
                    {
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, NoUsing 只用進階校正");
                        
                        for (int i = 1; i < 11; ++i)
                        {
                            _variationQuaternionDictionary[i] = _calibrateHelperV2.GetNameToQuaternionDictionary[i];
                        }
                    }
                    // 有使用普通模式之校正
                    else if (_stickHelper.GetIsUsingNormalMode && _calibrateHelperV2.GetIsUsingAdvancedMode == false)
                    {
                        Debug.Log($"{AllConfigs.DEBUG_TAG}, NoUsing 只用普通校正");
                        
                        for (int i = 1; i < 11; ++i)
                        {
                            _variationQuaternionDictionary[i] = _stickHelper.GetStickQuaternionDictionary[i];
                        }
                    }
                }
            }
        }
        
        private void InitQuaternionDictionary()
        {
            _variationQuaternionDictionary.Clear();

            for (int i = 1; i < 11; ++i)
            {
                _variationQuaternionDictionary.Add(i, Quaternion.identity);
            }
        }
        
        /// <summary>
        /// Init all table helper, base on RawData
        /// </summary>
        public void InitAllTableHelperDataStringToQuaternionHelper()
        {
            _variationHelperDictionary.Clear();
            
            // 因為是用 1 ～ 10
            _variationHelpers = new VariationHelper[11];
            
            for (int i = 1; i < _variationHelpers.Length; ++i)
            {
                _variationHelpers[i] = new VariationHelper(i, _dataStringToQuaternionHelper, _variationConfig);
                _variationHelperDictionary.Add(i, _variationHelpers[i]);
            }
        }
        
        /// <summary>
        /// Init all table helper, base on Calibrate Helper
        /// </summary>
        public void InitAllTableHelperCalibrateHelper()
        {
            _variationHelperDictionary.Clear();
            
            // 因為是用 1 ～ 10
            _variationHelpers = new VariationHelper[11];

            for (int i = 1; i < _variationHelpers.Length; ++i)
            {
                _variationHelpers[i] = new VariationHelper(i, _calibrateHelperV2, _variationConfig);
                _variationHelperDictionary.Add(i, _variationHelpers[i]);
            }
        }
        
        /// <summary>
        /// Init all table helper, base on StickHelper
        /// </summary>
        public void InitAllTableHelperStickHelper()
        {
            _variationHelperDictionary.Clear();
            
            // 因為是用 1 ～ 10
            _variationHelpers = new VariationHelper[11];

            for (int i = 1; i < _variationHelpers.Length; ++i)
            {
                _variationHelpers[i] = new VariationHelper(i, _stickHelper, _variationConfig);
                _variationHelperDictionary.Add(i, _variationHelpers[i]);
            }
        }
    }
}