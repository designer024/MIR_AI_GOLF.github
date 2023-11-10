using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin;


namespace EthanLin.AssignDataHelper
{
    public class BallBallV2Manager : MonoBehaviour
    {
        [SerializeField] private BallBallConfigHelperV2 _ballBallConfigHelperV2;
        
        [SerializeField] private AssignDataToRoleHelper _assignDataToRoleHelper;
        [SerializeField] private MakeTwoHandsCloserV2Manager _makeTwoHandsCloserV2Manager;
        
        
        #region 球球會用的

        /// <summary>
        /// 中心點
        /// </summary>
        [Header("中心點")] [SerializeField] private GameObject _centerBall;
        /// <summary>
        /// RawData左上臂至左手腕指向
        /// </summary>
        [SerializeField] private GameObject _arrowLU_WristL;
        /// <summary>
        /// RawData右上臂至右手腕指向
        /// </summary>
        [SerializeField] private GameObject _arrowRU_WristR;
        
        /// <summary>
        /// RawData左上臂至球球指向
        /// </summary>
        [SerializeField] private GameObject _arrowLU_CenterBall;
        /// <summary>
        /// RawData右上臂至球球指向
        /// </summary>
        [SerializeField] private GameObject _arrowRU_CenterBall;
        
        /// <summary>
        /// 修正後左上臂至左手腕的指向
        /// </summary>
        [SerializeField] private GameObject _fixedLeftVectorObject;
        /// <summary>
        /// 修正後右上臂至右手腕的指向
        /// </summary>
        [SerializeField] private GameObject _fixedRightVectorObject;

        [SerializeField] private GameObject _fixedLeftRotationAxis;
        public GameObject GetFixedLeftRotationAxis => _fixedLeftRotationAxis;
        [SerializeField] private GameObject _fixedRightRotationAxis;
        public GameObject GetFixedRightRotationAxis => _fixedRightRotationAxis;
        
        /// <summary>
        /// 左 白 與 橘的夾角
        /// </summary>
        private float _angleBetweenWAndO_left = 0f;
        /// <summary>
        /// 右 白 與 橘的夾角
        /// </summary>
        private float _angleBetweenWAndO_right = 0f;

        public float FixedAngleToTurn_left { private set; get; }
        public float FixedAngleToTurn_right { private set; get; }

        #endregion

        public void SetAssignDataToRoleHelper(GameObject aRole)
        {
            _assignDataToRoleHelper = aRole.GetComponent<AssignDataToRoleHelper>();
        }

        public void SetMakeTwoHandsCloserV2Manager(MakeTwoHandsCloserV2Manager aMakeTwoHandsCloserV2Manager)
        {
            _makeTwoHandsCloserV2Manager = aMakeTwoHandsCloserV2Manager;
        }

        private void Update()
        {
            if (_ballBallConfigHelperV2.GetIsUsingBallBall && _assignDataToRoleHelper != null)
            {
                GetVector();
            }
        }

        private void GetVector()
        {
            _centerBall.transform.position = (_assignDataToRoleHelper.GetLeftHandObject.transform.position + _assignDataToRoleHelper.GetRightHandObject.transform.position) / 2f;
            
            // RawData左上臂至左手腕指向
            _arrowLU_WristL.transform.up = _assignDataToRoleHelper.GetLeftUpperArmObject.transform.position - _assignDataToRoleHelper.GetLeftHandObject.transform.position;
            // RawData右上臂至右手腕指向
            _arrowRU_WristR.transform.up = _assignDataToRoleHelper.GetRightUpperArmObject.transform.position - _assignDataToRoleHelper.GetRightHandObject.transform.position;
            
            // RawData 左上臂 到 球球 之指向
            _arrowLU_CenterBall.transform.up = _assignDataToRoleHelper.GetLeftUpperArmObject.transform.position - _centerBall.transform.position;
            // RawData 右上臂 到 球球 之指向
            _arrowRU_CenterBall.transform.up = _assignDataToRoleHelper.GetRightUpperArmObject.transform.position - _centerBall.transform.position;

            _angleBetweenWAndO_left = Vector3.Angle(_arrowLU_WristL.transform.up, _arrowLU_CenterBall.transform.up);
            _angleBetweenWAndO_right = Vector3.Angle(_arrowRU_WristR.transform.up, _arrowRU_CenterBall.transform.up);
            
            // Debug.Log($"左{_angleBetweenWAndO_left}度, 右{_angleBetweenWAndO_right}度");
            
            // 左手的
            if (_angleBetweenWAndO_left <= _ballBallConfigHelperV2.GetLeftThreshold + 0.05f)
            {
                float output = _angleBetweenWAndO_left * _ballBallConfigHelperV2.GetLeftDiscount;
                _fixedLeftVectorObject.transform.up = Vector3.Lerp(_arrowLU_WristL.transform.up, _arrowLU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_left + 0.1f));
            }
            else if (_angleBetweenWAndO_left < _ballBallConfigHelperV2.GetLeftBackToRawData + 0.05f && _angleBetweenWAndO_left > _ballBallConfigHelperV2.GetLeftThreshold + 0.05f)
            {
                float output = _ballBallConfigHelperV2.GetSlopV2(_ballBallConfigHelperV2.GetLeftThreshold, _ballBallConfigHelperV2.GetLeftDiscount, _ballBallConfigHelperV2.GetLeftBackToRawData) * _angleBetweenWAndO_left + _ballBallConfigHelperV2.GetB_V2(_ballBallConfigHelperV2.GetLeftThreshold, _ballBallConfigHelperV2.GetLeftDiscount, _ballBallConfigHelperV2.GetLeftBackToRawData);
                _fixedLeftVectorObject.transform.up = Vector3.Lerp(_arrowLU_WristL.transform.up, _arrowLU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_left + 0.1f));
            }
            else
            {
                float output = _angleBetweenWAndO_left * 1f;
                _fixedLeftVectorObject.transform.up = Vector3.Lerp(_arrowLU_WristL.transform.up, _arrowLU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_left + 0.1f));
            }
            
            // 右手的
            if (_angleBetweenWAndO_right <= _ballBallConfigHelperV2.GetRightThreshold + 0.05f)
            {
                float output = _angleBetweenWAndO_right * _ballBallConfigHelperV2.GetRightDiscount;
                _fixedRightVectorObject.transform.up = Vector3.Lerp(_arrowRU_WristR.transform.up, _arrowRU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_right + 0.1f));
            }
            else if (_angleBetweenWAndO_right < _ballBallConfigHelperV2.GetRightBackToRawData + 0.05f && _angleBetweenWAndO_right > _ballBallConfigHelperV2.GetRightThreshold + 0.05f)
            {
                float output = _ballBallConfigHelperV2.GetSlopV2(_ballBallConfigHelperV2.GetRightThreshold, _ballBallConfigHelperV2.GetRightDiscount, _ballBallConfigHelperV2.GetRightBackToRawData) * _angleBetweenWAndO_right + _ballBallConfigHelperV2.GetB_V2(_ballBallConfigHelperV2.GetRightThreshold, _ballBallConfigHelperV2.GetRightDiscount, _ballBallConfigHelperV2.GetRightBackToRawData);
                _fixedRightVectorObject.transform.up = Vector3.Lerp(_arrowRU_WristR.transform.up, _arrowRU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_right + 0.1f));
            }
            else
            {
                float output = _angleBetweenWAndO_right * 1f;
                _fixedRightVectorObject.transform.up = Vector3.Lerp(_arrowRU_WristR.transform.up, _arrowRU_CenterBall.transform.up, (output + 0.1f) / (_angleBetweenWAndO_right + 0.1f));
            }

            FixedAngleToTurn_left = Vector3.Angle(_arrowLU_WristL.transform.up, _fixedLeftVectorObject.transform.up);
            FixedAngleToTurn_right = Vector3.Angle(_arrowRU_WristR.transform.up, _fixedRightVectorObject.transform.up);
            
            _fixedLeftRotationAxis.transform.up = Vector3.Cross(_arrowLU_WristL.transform.up, _fixedLeftVectorObject.transform.up);
            _fixedRightRotationAxis.transform.up = Vector3.Cross(_arrowRU_WristR.transform.up, _fixedRightVectorObject.transform.up);
            
            _makeTwoHandsCloserV2Manager.MakeCloser();
        }
    }
}

