using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.Variation;

namespace EthanLin.AssignDataHelper
{
    public class ChestAndPelvisFinalAdjustHelper : MonoBehaviour
    {
        /// <summary>
        /// 每次調整的值
        /// </summary>
        private const float VALUE_GAP = 0.5f;
        
        [SerializeField] private VariationConfig _variationConfig;

        [SerializeField] private Slider _chestPitchSlider;
        [SerializeField] private Slider _chestYawSlider;
        [SerializeField] private Slider _chestRollSlider;
        [SerializeField] private Slider _pelvisPitchSlider;
        [SerializeField] private Slider _pelvisYawSlider;
        [SerializeField] private Slider _pelvisRollSlider;

        /// <summary>
        /// 取得設定值
        /// </summary>
        public void GetValues()
        {
            _chestPitchSlider.value = _variationConfig.chestPitchAdjustValue;
            _chestYawSlider.value = _variationConfig.chestYawAdjustValue;
            _chestRollSlider.value = _variationConfig.chestRollAdjustValue;
            
            _pelvisPitchSlider.value = _variationConfig.pelvisPitchAdjustValue;
            _pelvisYawSlider.value = _variationConfig.pelvisYawAdjustValue;
            _pelvisRollSlider.value = _variationConfig.pelvisRollAdjustValue;
        }
        
        #region 胸

        /// <summary>
        /// 調整 胸 的 Pitch
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustChestPitch(int aIncreaseDecrease)
        {
            float value = _variationConfig.chestPitchAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.chestPitchAdjustValue = 20f;
            }
            else if (value < -20f)
            {
                _variationConfig.chestPitchAdjustValue = -20f;
            }
            else
            {
                _variationConfig.chestPitchAdjustValue = value;
            }

            _chestPitchSlider.value = _variationConfig.chestPitchAdjustValue;
        }

        public void SliderAdjustChestPitch(float aValue) => _variationConfig.chestPitchAdjustValue = aValue;
        
        /// <summary>
        /// 調整 胸 的 Yaw
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustChestYaw(int aIncreaseDecrease)
        {
            float value = _variationConfig.chestYawAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.chestYawAdjustValue = 20f;
            }
            else if (value < -20f)
            {
                _variationConfig.chestYawAdjustValue = -20f;
            }
            else
            {
                _variationConfig.chestYawAdjustValue = value;
            }

            _chestYawSlider.value = _variationConfig.chestYawAdjustValue;
        }

        public void SliderAdjustChestYaw(float aValue) => _variationConfig.chestYawAdjustValue = aValue;
        
        /// <summary>
        /// 調整 胸 的 Roll
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustChestRoll(int aIncreaseDecrease)
        {
            float value = _variationConfig.chestRollAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.chestRollAdjustValue = 20f;
            }
            else if (value < -20f)
            {
                _variationConfig.chestRollAdjustValue = -20f;
            }
            else
            {
                _variationConfig.chestRollAdjustValue = value;
            }
            
            _chestRollSlider.value = _variationConfig.chestRollAdjustValue;
        }
        
        public void SliderAdjustChestRoll(float aValue) => _variationConfig.chestRollAdjustValue = aValue;

        #endregion
        
        #region 腰

        /// <summary>
        /// 調整 腰 的 Pitch
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustPelvisPitch(int aIncreaseDecrease)
        {
            float value = _variationConfig.pelvisPitchAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.pelvisPitchAdjustValue = 20f;
            }
            else if (value < -20f)
            {
                _variationConfig.pelvisPitchAdjustValue = -20f;
            }
            else
            {
                _variationConfig.pelvisPitchAdjustValue = value;
            }
            
            _pelvisPitchSlider.value = _variationConfig.pelvisPitchAdjustValue;
        }

        public void SliderAdjustPelvisPitch(float aValue) => _variationConfig.pelvisPitchAdjustValue = aValue;
        
        /// <summary>
        /// 調整 腰 的 Yaw
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustPelvisYaw(int aIncreaseDecrease)
        {
            float value = _variationConfig.pelvisYawAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.pelvisYawAdjustValue = 20f;
            }
            else if (value < -20f)
            {
                _variationConfig.pelvisYawAdjustValue = -20f;
            }
            else
            {
                _variationConfig.pelvisYawAdjustValue = value;
            }
            
            _pelvisYawSlider.value = _variationConfig.pelvisYawAdjustValue;
        }
        
        public void SliderAdjustPelvisYaw(float aValue) => _variationConfig.pelvisYawAdjustValue = aValue;
        
        /// <summary>
        /// 調整 腰 的 Roll
        /// </summary>
        /// <param name="aIncreaseDecrease">1: Increase, -1: Decrease</param>
        public void AdjustPelvisRoll(int aIncreaseDecrease)
        {
            float value = _variationConfig.pelvisRollAdjustValue;
            value += aIncreaseDecrease * VALUE_GAP;
            if (value > 20f)
            {
                _variationConfig.pelvisRollAdjustValue = 15f;
            }
            else if (value < -20f)
            {
                _variationConfig.pelvisRollAdjustValue = -15f;
            }
            else
            {
                _variationConfig.pelvisRollAdjustValue = value;
            }
            
            _pelvisRollSlider.value = _variationConfig.pelvisRollAdjustValue;
        }
        
        public void SliderAdjustPelvisRoll(float aValue) => _variationConfig.pelvisRollAdjustValue = aValue;

        #endregion
    }
}

