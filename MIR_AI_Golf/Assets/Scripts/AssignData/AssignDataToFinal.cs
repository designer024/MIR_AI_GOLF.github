using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.AssignDataHelper
{
    public class AssignDataToFinal : MonoBehaviour
    {
        [SerializeField] private AssignDataToRoleHelper _assignDataToRoleHelper;
        [SerializeField] private LeftLegDetectCollider _leftLegDetectCollider;
        [SerializeField] private RightLegDetectCollider _rightLegDetectCollider;
        
        #region 各個關節物件 
    
        /// <summary>
        /// 左上臂
        /// </summary>
        [Header("左上臂")] [SerializeField] private GameObject _leftUpperArmObject;
        /// <summary>
        /// 右上臂
        /// </summary>
        [Header("右上臂")] [SerializeField] private GameObject _rightUpperArmObject;
        /// <summary>
        /// 胸
        /// </summary>
        [Header("胸")] [SerializeField] private GameObject _chestObject;
        /// <summary>
        /// 左前臂
        /// </summary>
        [Header("左前臂")] [SerializeField] private GameObject _leftForeArmObject;
        /// <summary>
        /// 右前臂
        /// </summary>
        [Header("右前臂")] [SerializeField] private GameObject _rightForeArmObject;
        /// <summary>
        /// 左大腿
        /// </summary>
        [Header("左大腿")] [SerializeField] private GameObject _leftThighObject;
        /// <summary>
        /// 右大腿
        /// </summary>
        [Header("右大腿")] [SerializeField] private GameObject _rightThighObject;
        /// <summary>
        /// 臀
        /// </summary>
        [Header("臀")] [SerializeField] private GameObject _pelvisObject;
        /// <summary>
        /// 左小腿
        /// </summary>
        [Header("左小腿")] [SerializeField] private GameObject _leftCalfObject;
        /// <summary>
        /// 右小腿
        /// </summary>
        [Header("右小腿")] [SerializeField] private GameObject _rightCalfObject;
        /// <summary>
        /// 左腳踝
        /// </summary>
        [Header("左腳踝")] [SerializeField] private GameObject _leftAnkleObject;
        /// <summary>
        /// 右腳踝
        /// </summary>
        [Header("右腳踝")] [SerializeField] private GameObject _rightAnkleObject;

        #endregion
        
        // 左小腿合法的Quaternion
        private Quaternion _leftCalfLegalQuaternion;
        // 右小腿合法的Quaternion
        private Quaternion _rightCalfLegalQuaternion;
        // 左小腿合法的Quaternion
        private Quaternion _leftAnkleLegalQuaternion;
        // 右小腿合法的Quaternion
        private Quaternion _rightAnkleLegalQuaternion;

        private void Update()
        {
            _chestObject.transform.rotation = _assignDataToRoleHelper.GetChestObject.transform.rotation;

            _leftUpperArmObject.transform.rotation = _assignDataToRoleHelper.GetLeftUpperArmObject.transform.rotation;
            _leftForeArmObject.transform.rotation = _assignDataToRoleHelper.GetLeftForeArmObject.transform.rotation;

            _rightUpperArmObject.transform.rotation = _assignDataToRoleHelper.GetRightUpperArmObject.transform.rotation;
            _rightForeArmObject.transform.rotation = _assignDataToRoleHelper.GetRightForeArmObject.transform.rotation;

            _pelvisObject.transform.rotation = _assignDataToRoleHelper.GetPelvisObject.transform.rotation;
            
            _leftThighObject.transform.rotation = _assignDataToRoleHelper.GetLeftThighObject.transform.rotation;
            _rightThighObject.transform.rotation = _assignDataToRoleHelper.GetRightThighObject.transform.rotation;
            
            if (_leftLegDetectCollider.IsLeftLegLegal)
            {
                _leftCalfLegalQuaternion = _assignDataToRoleHelper.GetLeftCalfObject.transform.rotation;
                _leftCalfObject.transform.rotation = _leftCalfLegalQuaternion;
                
                _leftAnkleLegalQuaternion = _assignDataToRoleHelper.GetLeftAnkleObject.transform.rotation;
                _leftAnkleObject.transform.rotation = _leftAnkleLegalQuaternion;
            }
            else
            {
                _leftCalfObject.transform.rotation = _leftCalfLegalQuaternion;
                _leftAnkleObject.transform.rotation = _leftAnkleLegalQuaternion;
            }

            if (_rightLegDetectCollider.IsRightLegLegal)
            {
                _rightCalfLegalQuaternion = _assignDataToRoleHelper.GetRightCalfObject.transform.rotation;
                _rightCalfObject.transform.rotation = _rightCalfLegalQuaternion;
                
                _rightAnkleLegalQuaternion = _assignDataToRoleHelper.GetRightAnkleObject.transform.rotation;
                _rightAnkleObject.transform.rotation = _rightAnkleLegalQuaternion;
            }
            else
            {
                _rightCalfObject.transform.rotation = _rightCalfLegalQuaternion;
                _rightAnkleObject.transform.rotation = _rightAnkleLegalQuaternion;
            }
        }
    }
}