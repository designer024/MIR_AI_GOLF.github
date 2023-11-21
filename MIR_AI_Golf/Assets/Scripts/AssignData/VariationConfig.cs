using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.Variation
{
    /// <summary>
    /// 設置一些打折的參數
    /// </summary>
    public class VariationConfig : MonoBehaviour
    {
        [Serializable]
        public class SetBodyPartVariationConfig
        {
            /// <summary>
            /// id
            /// </summary>
            [Header("ID")] public int bodyId;
            /// <summary>
            /// 部位名稱
            /// </summary>
            [Header("部位名稱")] [SerializeField] private string _bodyName;
            /// <summary>
            /// 閥值, 0 ~ 20
            /// </summary>
            [Header("閥值")] [Range(0f, 20f)] public float bodyThreshold;
            /// <summary>
            /// 打折數, 0 ~ 1
            /// </summary>
            [Header("打折數")] [Range(0f, 1f)] public float bodyDiscount;
            /// <summary>
            /// 何時回RawData, 25 ~ 45
            /// </summary>
            [Header("何時回RawData")] [Range(25f, 45f)] public float bodyBackToRawData;
        }
        
        /// <summary>
        /// 使用Variation
        /// </summary>
        [Header("使用 Variation")] [SerializeField] private bool _useVariation = true;
        /// <summary>
        /// 使用Variation get
        /// </summary>
        public bool GetIsUsingVariation => _useVariation;
        /// <summary>
        /// 使用Variation set
        /// </summary>
        public void SetIsUsingVariation(bool aIsUsingVariation) => _useVariation = aIsUsingVariation;

        /// <summary>
        /// 只有上半身!?
        /// </summary>
        [Header("只有上半身!?")] [SerializeField] private bool _onlyUpperBody = false;
        /// <summary>
        /// 只有上半身!? get
        /// </summary>
        public bool GetOnlyUpperBody => _onlyUpperBody;
        /// <summary>
        /// 只有上半身!? set
        /// </summary>
        public void SetOnlyUpperBody(bool aOnlyUpperBody) => _onlyUpperBody = aOnlyUpperBody;
        
        /// <summary>
        /// 胸 Pitch, -20 ~ 20
        /// </summary>
        [Header("胸 Pitch, -20 ~ 20")] [Range(-20f, 20f)] public float chestPitchAdjustValue;
        /// <summary>
        /// 胸 Yaw, -20 ~ 20
        /// </summary>
        [Header("胸 Yaw, -20 ~ 20")] [Range(-20f, 20f)] public float chestYawAdjustValue;
        /// <summary>
        /// 胸 Roll, -20 ~ 20
        /// </summary>
        [Header("胸 Roll, -20 ~ 20")] [Range(-20f, 20f)] public float chestRollAdjustValue;
        
        /// <summary>
        /// 臀 Pitch, -20 ~ 20
        /// </summary>
        [Header("臀 Pitch, -20 ~ 20")] [Range(-20f, 20f)] public float pelvisPitchAdjustValue;
        /// <summary>
        /// 臀 Yaw, -20 ~ 20
        /// </summary>
        [Header("臀 Yaw, -20 ~ 20")] [Range(-20f, 20f)] public float pelvisYawAdjustValue;
        /// <summary>
        /// 臀 Roll, -20 ~ 20
        /// </summary>
        [Header("臀 Roll, -20 ~ 20")] [Range(-20f, 20f)] public float pelvisRollAdjustValue;

        /// <summary>
        /// 各部位Variation設置List
        /// </summary>
        [Header("各部位Variation設置")] [SerializeField] private List<SetBodyPartVariationConfig> _bodyPartVariationConfigList;
        public List<SetBodyPartVariationConfig> GetBodyPartVariationConfigList => _bodyPartVariationConfigList;

        #region 給目前第2版用的
        
        public float GetSlopV2(float aThreshold, float aDiscount, float aPointBackToRawData)
        {
            return (aPointBackToRawData - aThreshold * aDiscount) / (aPointBackToRawData - aThreshold);
        }

        public float GetB_V2(float aThreshold, float aDiscount, float aPointBackToRawData)
        {
            return aPointBackToRawData * (1f - ((aPointBackToRawData - aThreshold * aDiscount) / (aPointBackToRawData - aThreshold)));
        }
        
        #endregion

        #region 給第一版或舊版用的

        /// <summary>
        /// 取得閥值內角度的斜率(or 會通 0 90 180 270 360之線段)
        /// </summary>
        /// <param name="aDiscount">打折</param>
        /// <returns></returns>
        public float GetSlope(float aDiscount)
        {
            if (aDiscount > 1f)
            {
                return 1f;
            }
            else if (aDiscount < 0f)
            {
                return 0f;
            }
            else
            {
                return aDiscount;
            }
        }

        /// <summary>
        /// 取得斜率
        /// </summary>
        /// <param name="aThreshold">閥值</param>
        /// <param name="aDiscount">打折 Min: 0, Max: 1</param>
        /// <returns></returns>
        public float GetSlope(float aThreshold, float aDiscount)
        {
            if (aDiscount < 0f)
            {
                aDiscount = 0f;
            }
            else if (aDiscount > 1f)
            {
                aDiscount = 1f;
            }
            return (45f - aThreshold * aDiscount) / (45f - aThreshold);
        }
        
        /// <summary>
        /// 直線方程式 y = mx + b
        /// </summary>
        /// <param name="aSlope">直線之斜率</param>
        /// <param name="aInputAngleValue">手臂或腿與參考指向之夾角</param>
        public float GetEquationOfLine(float aSlope, float aInputAngleValue)
        {
            return aSlope * aInputAngleValue + 45f * (1f - aSlope);
        }
        
        /// <summary>
        /// 直線方程式 y = mx + b
        /// </summary>
        /// <param name="aSlope">直線之斜率</param>
        /// <param name="aRawDataValue">RawData之值</param>
        /// <param name="ab">b值</param>
        /// <returns></returns>
        public float GetEquationOfLine(float aSlope, float aRawDataValue, float ab)
        {
            return aSlope * aRawDataValue + ab;
        }

        #endregion
    }
}

