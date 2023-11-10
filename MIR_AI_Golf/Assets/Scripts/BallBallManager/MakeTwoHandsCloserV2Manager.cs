using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin;

namespace EthanLin.AssignDataHelper
{
    public class MakeTwoHandsCloserV2Manager : MonoBehaviour
    {
        [SerializeField] private AssignDataToRoleHelper _assignDataToRoleHelper;
        [SerializeField] private BallBallV2Manager _ballBallV2Manager;
        [SerializeField] private BallBallConfigHelperV2 _ballBallConfigHelperV2;
         
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
        /// 腰
        /// </summary>
        [Header("腰")] [SerializeField] private GameObject _pelvisObject;
        /// <summary>
        /// 左小腿
        /// </summary>
        [Header("左小腿")] [SerializeField] private GameObject _leftCalfObject;
        /// <summary>
        /// 右小腿
        /// </summary>
        [Header("右小腿")] [SerializeField] private GameObject _rightCalfObject;
        
        #endregion
        
        public void SetAssignDataToRoleHelper(GameObject aRole)
        {
            _assignDataToRoleHelper = aRole.GetComponent<AssignDataToRoleHelper>();
        }
        
        public void MakeCloser()
        {
            _chestObject.transform.rotation = _assignDataToRoleHelper.GetChestObject.transform.rotation;

            MakeCloserV2();
            
            // _pelvisObject.transform.rotation = _playbackHistoryHelper.GetCurrentPelvisRecordQuaternion;
            //
            // _leftThighObject.transform.rotation = _playbackHistoryHelper.GetCurrentLtRecordQuaternion;
            // _rightThighObject.transform.rotation = _playbackHistoryHelper.GetCurrentRtRecordQuaternion;
            //
            // _leftCalfObject.transform.rotation = _playbackHistoryHelper.GetCurrentLcRecordQuaternion;
            // _rightCalfObject.transform.rotation = _playbackHistoryHelper.GetCurrentRcRecordQuaternion;
            
            // if (_ballBallConfigHelperV2.GetIsUsingBallBall && _assignDataToRoleHelper != null)
            // {
            //     
            // }
        }
        
        public void SetAllToIdentity()
        {
            _chestObject.transform.rotation = Quaternion.identity;

            _leftUpperArmObject.transform.rotation = Quaternion.identity;
            _rightUpperArmObject.transform.rotation = Quaternion.identity;
        
            _leftForeArmObject.transform.rotation = Quaternion.identity;
            _rightForeArmObject.transform.rotation = Quaternion.identity;
            
            _pelvisObject.transform.rotation = Quaternion.identity;
            
            _leftThighObject.transform.rotation = Quaternion.identity;
            _rightThighObject.transform.rotation = Quaternion.identity;
            
            _leftCalfObject.transform.rotation = Quaternion.identity;
            _rightCalfObject.transform.rotation = Quaternion.identity;
        }

        private void MakeCloserV2()
        {
            _leftUpperArmObject.transform.rotation = Quaternion.AngleAxis(_ballBallV2Manager.FixedAngleToTurn_left * _ballBallConfigHelperV2.GetLeftUpperFinalRatio, _ballBallV2Manager.GetFixedLeftRotationAxis.transform.up) * _assignDataToRoleHelper.GetLeftUpperArmObject.transform.rotation;
            _rightUpperArmObject.transform.rotation = Quaternion.AngleAxis(_ballBallV2Manager.FixedAngleToTurn_right * _ballBallConfigHelperV2.GetRightUpperFinalRatio, _ballBallV2Manager.GetFixedRightRotationAxis.transform.up) * _assignDataToRoleHelper.GetRightUpperArmObject.transform.rotation;
            
            
            // _leftForeArmObject.transform.rotation = _assignQuaternionDataToRoleManager.GetLeftForeArmObject.transform.rotation;
            _leftForeArmObject.transform.rotation = Quaternion.AngleAxis(_ballBallV2Manager.FixedAngleToTurn_left * _ballBallConfigHelperV2.GetLeftForeFinalRatio, _ballBallV2Manager.GetFixedLeftRotationAxis.transform.up) * _assignDataToRoleHelper.GetLeftForeArmObject.transform.rotation;

            // _rightForeArmObject.transform.rotation = _assignQuaternionDataToRoleManager.GetRightForeArmObject.transform.rotation;
            _rightForeArmObject.transform.rotation = Quaternion.AngleAxis(_ballBallV2Manager.FixedAngleToTurn_right * _ballBallConfigHelperV2.GetRightForeFinalRatio , _ballBallV2Manager.GetFixedRightRotationAxis.transform.up) * _assignDataToRoleHelper.GetRightForeArmObject.transform.rotation;
        }
    }
}