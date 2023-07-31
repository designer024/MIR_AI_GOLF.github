using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.AssignDataHelper;
using EthanLin.CalibrateHelper;

namespace EthanLin.Variation
{
    public class VariationHelper
    {
        private int _partId;
        
        private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        private VariationConfig _variationConfig;
        
        private CalibrateHelper_V2 _calibrateHelperV2;
        private StickHelper _stickHelper;
        
        /// <summary>
        /// 投影 for Y-Z 平面
        /// </summary>
        private Vector3 _referenceProjectedYZ;
        /// <summary>
        /// 投影 for X-Y 平面
        /// </summary>
        private Vector3 _referenceProjectedXY;
        
        /// <summary>
        /// 投影 for 1st fix 平面
        /// </summary>
        private Vector3 _reference_1stProjected;
        /// <summary>
        /// 第一次修完後的指向
        /// </summary>
        private Vector3 _firstStepFixedVector;

        private Vector3 _bodyVector;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="aPartId">sensor 部位ID, 參考 AllPartNameIndex / AllPartNameEnum</param>
        /// <param name="aDataStringToQuaternionHelper">DataStringToQuaternionHelper</param>
        /// <param name="aVariationConfig">VariationConfig</param>
        public VariationHelper(int aPartId, DataStringToQuaternionHelper aDataStringToQuaternionHelper, VariationConfig aVariationConfig)
        {
            _partId = aPartId;
            
            _referenceProjectedYZ = Vector3.zero;
            _referenceProjectedXY = Vector3.zero;
            
            _reference_1stProjected = Vector3.zero;

            _firstStepFixedVector = Vector3.zero;
            
            _bodyVector = Vector3.zero;

            _dataStringToQuaternionHelper = aDataStringToQuaternionHelper;
            _variationConfig = aVariationConfig;
        }
        
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="aPartId">sensor 部位ID, 參考 AllPartNameIndex / AllPartNameEnum</param>
        /// <param name="aCalibrateHelperV2">CalibrateHelper_V2</param>
        /// <param name="aVariationConfig">VariationConfig</param>
        public VariationHelper(int aPartId, CalibrateHelper_V2 aCalibrateHelperV2, VariationConfig aVariationConfig)
        {
            _partId = aPartId;
            
            _referenceProjectedYZ = Vector3.zero;
            _referenceProjectedXY = Vector3.zero;
            
            _reference_1stProjected = Vector3.zero;

            _firstStepFixedVector = Vector3.zero;
            
            _bodyVector = Vector3.zero;

            _calibrateHelperV2 = aCalibrateHelperV2;
            _variationConfig = aVariationConfig;
        }
        
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="aPartId">sensor 部位ID, 參考 AllPartNameIndex / AllPartNameEnum</param>
        /// <param name="aStickHelper">StickHelper</param>
        /// <param name="aVariationConfig">VariationConfig</param>
        public VariationHelper(int aPartId, StickHelper aStickHelper, VariationConfig aVariationConfig)
        {
            _partId = aPartId;
            
            _referenceProjectedYZ = Vector3.zero;
            _referenceProjectedXY = Vector3.zero;
            
            _reference_1stProjected = Vector3.zero;

            _firstStepFixedVector = Vector3.zero;
            
            _bodyVector = Vector3.zero;

            _stickHelper = aStickHelper;
            _variationConfig = aVariationConfig;
        }

        /// <summary>
        /// 最後輸出的Rotation結果, base on DataStringToQuaternionHelper
        /// </summary>
        public Quaternion GetFinalQuaternion_DataStringToQuaternionHelper(Vector3 aBaseVectorYZ_Right, Vector3 aBaseVectorXY_Forward, Vector3 aBaseVectorXZ_Up)
        {
            _referenceProjectedYZ = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, aBaseVectorYZ_Right);
            _referenceProjectedXY = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, aBaseVectorXY_Forward);
            // _referenceProjectedXZ = Vector3.ProjectOnPlane(_serialPortDataHandler.GetBodyQuaternionDictionary[_partId] * Vector3.up, aBaseVectorXZ_Up);
            
            // 靠近X-Z面
            if (Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) < 45.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) < 45.05f || Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) > 135.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) > 135.05f)
            {
                // 靠近Y-Z面 & X-Z面
                if (Vector3.Angle(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedYZ) < Vector3.Angle(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedXY))
                {
                    #region 先處理 Y-Z 面 A

                    float input_1st = Vector3.Angle(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion

                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
                // 靠近X-Y面 & X-Z面
                else
                {
                    #region 先處理 X-Y 面 B

                    float input_1st = Vector3.Angle(_referenceProjectedXY, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion
                    
                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
                
            }
            // 靠近Y-Z面 & X-Y面
            else
            {
                #region 先處理 Y-Z 面 A

                float input_1st = Vector3.Angle(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up);
                // 小於Threshold
                if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_1st * 1f;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                
                _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXY_Forward);

                #endregion
                    
                #region 再處理 X-Y 面 B

                float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                // 小於Threshold
                if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st* _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_after_1st * 1f;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }

                #endregion
            }
            
            // _finalQuaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(_serialPortDataHandler.GetBodyQuaternionDictionary[_partId] * Vector3.forward, _bodyVector), _bodyVector);
            return Quaternion.LookRotation(Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[_partId] * Vector3.forward, _bodyVector), _bodyVector);;
        }

        /// <summary>
        /// 最後輸出的Rotation結果, base on Calibrate Helper
        /// </summary>
        public Quaternion GetFinalQuaternion_CalibrateHelper(Vector3 aBaseVectorYZ_Right, Vector3 aBaseVectorXY_Forward, Vector3 aBaseVectorXZ_Up)
        {
            _referenceProjectedYZ = Vector3.ProjectOnPlane(_calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, aBaseVectorYZ_Right);
            _referenceProjectedXY = Vector3.ProjectOnPlane(_calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, aBaseVectorXY_Forward);
            
            // 靠近X-Z面
            if (Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) < 45.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) < 45.05f || Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) > 135.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) > 135.05f)
            {
                // 靠近Y-Z面 & X-Z面
                if (Vector3.Angle(_calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedYZ) < Vector3.Angle(_calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedXY))
                {
                    #region 先處理 Y-Z 面 A

                    float input_1st = Vector3.Angle(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion

                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
                // 靠近X-Y面 & X-Z面
                else
                {
                    #region 先處理 X-Y 面 B

                    float input_1st = Vector3.Angle(_referenceProjectedXY, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion
                    
                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
            }
            // 靠近Y-Z面 & X-Y面
            else
            {
                #region 先處理 Y-Z 面 A

                float input_1st = Vector3.Angle(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up);
                // 小於Threshold
                if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_1st * 1f;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                
                _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXY_Forward);

                #endregion
                    
                #region 再處理 X-Y 面 B

                float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                // 小於Threshold
                if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st* _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_after_1st * 1f;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }

                #endregion
            }
            
            return Quaternion.LookRotation(Vector3.ProjectOnPlane(_calibrateHelperV2.GetNameToQuaternionDictionary[_partId] * Vector3.forward, _bodyVector), _bodyVector);
        }
        
        /// <summary>
        /// 最後輸出的Rotation結果, base on Stick Helper
        /// </summary>
        public Quaternion GetFinalQuaternion_StickHelper(Vector3 aBaseVectorYZ_Right, Vector3 aBaseVectorXY_Forward, Vector3 aBaseVectorXZ_Up)
        {
            _referenceProjectedYZ = Vector3.ProjectOnPlane(_stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, aBaseVectorYZ_Right);
            _referenceProjectedXY = Vector3.ProjectOnPlane(_stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, aBaseVectorXY_Forward);
            
            // 靠近X-Z面
            if (Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) < 45.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) < 45.05f || Vector3.Angle(_referenceProjectedYZ, aBaseVectorXY_Forward) > 135.05f || Vector3.Angle(_referenceProjectedXY, aBaseVectorYZ_Right) > 135.05f)
            {
                // 靠近Y-Z面 & X-Z面
                if (Vector3.Angle(_stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedYZ) < Vector3.Angle(_stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, _referenceProjectedXY))
                {
                    #region 先處理 Y-Z 面 A

                    float input_1st = Vector3.Angle(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion

                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
                // 靠近X-Y面 & X-Z面
                else
                {
                    #region 先處理 X-Y 面 B

                    float input_1st = Vector3.Angle(_referenceProjectedXY, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up);
                    // 小於Threshold
                    if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_1st * 1f;
                        _firstStepFixedVector = Vector3.Lerp(_referenceProjectedXY, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                    }
                    
                    _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXZ_Up);

                    #endregion
                    
                    #region 再處理 X-Z 面 C

                    float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                    // 小於Threshold
                    if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 超過Threshold 
                    else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                    {
                        float output = input_after_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }
                    // 目前在RawData
                    else
                    {
                        float output = input_after_1st * 1f;
                        _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                    }

                    #endregion
                }
            }
            // 靠近Y-Z面 & X-Y面
            else
            {
                #region 先處理 Y-Z 面 A

                float input_1st = Vector3.Angle(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up);
                // 小於Threshold
                if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_1st * _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_1st * 1f;
                    _firstStepFixedVector = Vector3.Lerp(_referenceProjectedYZ, _stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.up, (output + 0.1f) / (input_1st + 0.1f));
                }
                
                _reference_1stProjected = Vector3.ProjectOnPlane(_firstStepFixedVector, aBaseVectorXY_Forward);

                #endregion
                    
                #region 再處理 X-Y 面 B

                float input_after_1st = Vector3.Angle(_firstStepFixedVector, _reference_1stProjected);
                // 小於Threshold
                if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st * _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 超過Threshold 
                else if (input_after_1st < _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData + 0.05f && input_after_1st > _variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold + 0.05f)
                {
                    float output = input_after_1st* _variationConfig.GetSlopV2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData) + _variationConfig.GetB_V2(_variationConfig.GetBodyPartVariationConfigList[_partId].bodyThreshold, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyDiscount, _variationConfig.GetBodyPartVariationConfigList[_partId].bodyBackToRawData);
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }
                // 目前在RawData
                else
                {
                    float output = input_after_1st * 1f;
                    _bodyVector = Vector3.Lerp(_reference_1stProjected, _firstStepFixedVector, (output + 0.1f) / (input_after_1st + 0.1f));
                }

                #endregion
            }
            
            return Quaternion.LookRotation(Vector3.ProjectOnPlane(_stickHelper.GetStickQuaternionDictionary[_partId] * Vector3.forward, _bodyVector), _bodyVector);
        }
    }
}