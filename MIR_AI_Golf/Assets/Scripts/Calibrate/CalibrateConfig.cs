using UnityEngine;

namespace EthanLin.CalibrateHelper
{
    public class CalibrateConfig : MonoBehaviour
    {
        /// <summary>
        /// 是否使用進階模式的校正
        /// </summary>
        protected bool IsUsingAdvancedMode = false;
        /// <summary>
        /// 是否使用進階模式的校正
        /// </summary>
        public bool GetIsUsingAdvancedMode => IsUsingAdvancedMode;
        /// <summary>
        /// 是否使用普通模式的校正
        /// </summary>
        protected bool IsUsingNormalMode = false;
        /// <summary>
        /// 是否使用普通模式的校正
        /// </summary>
        public bool GetIsUsingNormalMode => IsUsingNormalMode;
    }
}