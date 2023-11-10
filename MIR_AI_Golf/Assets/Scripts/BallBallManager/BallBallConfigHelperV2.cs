using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.AssignDataHelper
{
    public class BallBallConfigHelperV2 : MonoBehaviour
    {
        /// <summary>
        /// 是否要使用球球
        /// </summary>
        [Header("是否要使用球球")] [SerializeField] private bool _isUsingBallBall = false;
        /// <summary>
        /// 是否要使用球球
        /// </summary>
        public bool GetIsUsingBallBall => _isUsingBallBall;
        /// <summary>
        /// 是否要使用球球
        /// </summary>
        public void SetIsUsingBallBall(bool aIsUsing) => _isUsingBallBall = aIsUsing;
        
        #region 左手的

        /// <summary>
        /// 左手的 閥值
        /// </summary>
        [Header("左手的 閥值")] [Range(0f, 25f)] [SerializeField] private float _leftThreshold;
        /// <summary>
        /// 左手的 閥值 Get
        /// </summary>
        public float GetLeftThreshold => _leftThreshold;
        /// <summary>
        /// 左手的 閥值 Set
        /// </summary>
        public void SetLeftThreshold(float aNewValue) => _leftThreshold = aNewValue;
        /// <summary>
        /// 左手的 打折的比例
        /// </summary>
        [Header("左手的 打折的比例")] [Range(0.0f, 2.0f)] [SerializeField] private float _leftDiscount;
        /// <summary>
        /// 左手的 打折的比例 Get
        /// </summary>
        public float GetLeftDiscount => _leftDiscount;
        /// <summary>
        /// 左手的 打折的比例 Set
        /// </summary>
        public void SetLeftDiscount(float aNewValue) => _leftDiscount = aNewValue;
        /// <summary>
        /// 左手的  啥時回RawData
        /// </summary>
        [Header("左手的  啥時回RawData")] [Range(25f, 75f)] [SerializeField] private float _leftBackToRawData;
        /// <summary>
        /// 左手的  啥時回RawData Get
        /// </summary>
        public float GetLeftBackToRawData => _leftBackToRawData;
        /// <summary>
        /// 左手的  啥時回RawData Set
        /// </summary>
        public void SetLeftBackToRawData(float aNewValue) => _leftBackToRawData = aNewValue;
        /// <summary>
        /// 左上臂 旋轉乘以的倍數
        /// </summary>
        [Header("左上臂 旋轉乘以的倍數")] [Range(0.1f, 2f)] [SerializeField] private float _leftUpperFinalRatio;
        /// <summary>
        /// 左上臂 旋轉乘以的倍數
        /// </summary>
        public float GetLeftUpperFinalRatio => _leftUpperFinalRatio;
        /// <summary>
        /// 左上臂 旋轉乘以的倍數
        /// </summary>
        public void SetLeftUpperFinalRatio(float aNewValue) => _leftUpperFinalRatio = aNewValue;
        /// <summary>
        /// 左前臂 旋轉乘以的倍數
        /// </summary>
        [Header("左前臂 旋轉乘以的倍數")] [Range(0.1f, 2f)] [SerializeField] private float _leftForeFinalRatio;
        /// <summary>
        /// 左前臂 旋轉乘以的倍數
        /// </summary>
        public float GetLeftForeFinalRatio => _leftForeFinalRatio;
        /// <summary>
        /// 左前臂 旋轉乘以的倍數
        /// </summary>
        public void SetLeftForeFinalRatio(float aNewValue) => _leftForeFinalRatio = aNewValue;

        #endregion
        
        #region 右手的

        /// <summary>
        /// 右手的 閥值
        /// </summary>
        [Header("右手的 閥值")] [Range(0f, 25f)] [SerializeField] private float _rightThreshold;
        /// <summary>
        /// 右手的 閥值 Get
        /// </summary>
        public float GetRightThreshold => _rightThreshold;
        /// <summary>
        /// 右手的 閥值 Set
        /// </summary>
        public void SetRightThreshold(float aNewValue) => _rightThreshold = aNewValue;
        /// <summary>
        /// 右手的 打折的比例
        /// </summary>
        [Header("右手的 打折的比例")] [Range(0.0f, 2.0f)] [SerializeField] private float _rightDiscount;
        /// <summary>
        /// 右手的 打折的比例 Get
        /// </summary>
        public float GetRightDiscount => _rightDiscount;
        /// <summary>
        /// 右手的 打折的比例 Set
        /// </summary>
        public void SetRightDiscount(float aNewValue) => _rightDiscount = aNewValue;
        /// <summary>
        /// 右手的  啥時回RawData
        /// </summary>
        [Header("右手的  啥時回RawData")] [Range(25f, 75f)] [SerializeField] private float _rightBackToRawData;
        /// <summary>
        /// 右手的  啥時回RawData Get
        /// </summary>
        public float GetRightBackToRawData => _rightBackToRawData;
        /// <summary>
        /// 右手的  啥時回RawData Set
        /// </summary>
        public void SetRightBackToRawData(float aNewValue) => _rightBackToRawData = aNewValue;
        /// <summary>
        /// 右上臂 旋轉乘以的倍數
        /// </summary>
        [Header("右上臂 旋轉乘以的倍數")] [Range(0.1f, 2f)] [SerializeField] private float _rightUpperFinalRatio;
        /// <summary>
        /// 右上臂 旋轉乘以的倍數
        /// </summary>
        public float GetRightUpperFinalRatio => _rightUpperFinalRatio;
        /// <summary>
        /// 右上臂 旋轉乘以的倍數
        /// </summary>
        public void SetRightUpperFinalRatio(float aNewValue) => _rightUpperFinalRatio = aNewValue;
        /// <summary>
        /// 右前臂 旋轉乘以的倍數
        /// </summary>
        [Header("右前臂 旋轉乘以的倍數")] [Range(0.1f, 2f)] [SerializeField] private float _rightForeFinalRatio;
        /// <summary>
        /// 右前臂 旋轉乘以的倍數
        /// </summary>
        public float GetRightForeFinalRatio => _rightForeFinalRatio;
        /// <summary>
        /// 右前臂 旋轉乘以的倍數
        /// </summary>
        public void SetRightForeFinalRatio(float aNewValue) => _rightForeFinalRatio = aNewValue;

        #endregion
        
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
    }
}