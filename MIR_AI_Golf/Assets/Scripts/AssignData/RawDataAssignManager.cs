using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.Config;

namespace EthanLin.AssignDataHelper
{
    public class RawDataAssignManager : MonoBehaviour
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        
        #region 各個關節物件

        /// <summary>
        /// 左上臂
        /// </summary>
        [Header("左上臂")] [SerializeField] private GameObject _leftUpperArmObject;
        /// <summary>
        /// 左上臂
        /// </summary>
        public GameObject GetLeftUpperArmObject => _leftUpperArmObject;
        /// <summary>
        /// 右上臂
        /// </summary>
        [Header("右上臂")] [SerializeField] private GameObject _rightUpperArmObject;
        /// <summary>
        /// 右上臂
        /// </summary>
        public GameObject GetRightUpperArmObject => _rightUpperArmObject;
        /// <summary>
        /// 胸
        /// </summary>
        [Header("胸")] [SerializeField] private GameObject _chestObject;
        /// <summary>
        /// 胸
        /// </summary>
        public GameObject GetChestObject => _chestObject;
        /// <summary>
        /// 胸前的東東
        /// </summary>
        [Header("胸前的東東")] [SerializeField] private GameObject _frontChestObject;
        /// <summary>
        /// 胸前的東東
        /// </summary>
        public GameObject GetFrontChestObject => _frontChestObject;
        /// <summary>
        /// 左前臂
        /// </summary>
        [Header("左前臂")] [SerializeField] private GameObject _leftForeArmObject;
        /// <summary>
        /// 左前臂
        /// </summary>
        public GameObject GetLeftForeArmObject => _leftForeArmObject;
        /// <summary>
        /// Real 左前臂
        /// </summary>
        [Header("Real 左前臂")] [SerializeField] private GameObject _leftForeArmRealObject;
        /// <summary>
        /// Real 左前臂
        /// </summary>
        public GameObject GetLeftForeArmRealObject => _leftForeArmRealObject;
        /// <summary>
        /// 右前臂
        /// </summary>
        [Header("右前臂")] [SerializeField] private GameObject _rightForeArmObject;
        /// <summary>
        /// 右前臂
        /// </summary>
        public GameObject GetRightForeArmObject => _rightForeArmObject;
        /// <summary>
        /// Real 右前臂
        /// </summary>
        [Header("Real 右前臂")] [SerializeField] private GameObject _rightForeArmRealObject;
        /// <summary>
        /// Real 右前臂
        /// </summary>
        public GameObject GetRightForeArmRealObject => _rightForeArmRealObject;
        /// <summary>
        /// 左大腿
        /// </summary>
        [Header("左大腿")] [SerializeField] private GameObject _leftThighObject;
        /// <summary>
        /// 左大腿
        /// </summary>
        public GameObject GetLeftThighObject => _leftThighObject;
        /// <summary>
        /// 右大腿
        /// </summary>
        [Header("右大腿")] [SerializeField] private GameObject _rightThighObject;
        /// <summary>
        /// 右大腿
        /// </summary>
        public GameObject GetRightThighObject => _rightThighObject;
        /// <summary>
        /// 臀
        /// </summary>
        [Header("臀")] [SerializeField] private GameObject _pelvisObject;
        /// <summary>
        /// 臀
        /// </summary>
        public GameObject GetPelvisObject => _pelvisObject;
        /// <summary>
        /// 左小腿
        /// </summary>
        [Header("左小腿")] [SerializeField] private GameObject _leftCalfObject;
        /// <summary>
        /// 左小腿
        /// </summary>
        public GameObject GetLeftCalfObject => _leftCalfObject;
        /// <summary>
        /// 右小腿
        /// </summary>
        [Header("右小腿")] [SerializeField] private GameObject _rightCalfObject;
        /// <summary>
        /// 右小腿
        /// </summary>
        public GameObject GetRightCalfObject => _rightCalfObject;
        /// <summary>
        /// 胸與臀中間
        /// </summary>
        [Header("胸與臀中間")] [SerializeField] private GameObject _chestAndPelvisBetween;
        /// <summary>
        /// 左手掌
        /// </summary>
        [Header("左手掌")] [SerializeField] private GameObject _leftHandObject;
        /// <summary>
        /// 左手掌
        /// </summary>
        public GameObject GetLeftHandObject => _leftHandObject;
        /// <summary>
        /// 右手掌
        /// </summary>
        [Header("右手掌")] [SerializeField] private GameObject _rightHandObject;
        /// <summary>
        /// 右手掌
        /// </summary>
        public GameObject GetRightHandObject => _rightHandObject;
        /// <summary>
        /// 左腳踝
        /// </summary>
        [Header("左腳踝")] [SerializeField] private GameObject _leftAnkleObject;
        /// <summary>
        /// 左腳踝
        /// </summary>
        public GameObject GetLeftAnkleObject => _leftAnkleObject;
        /// <summary>
        /// 右腳踝
        /// </summary>
        [Header("右腳踝")] [SerializeField] private GameObject _rightAnkleObject;
        /// <summary>
        /// 右腳踝
        /// </summary>
        public GameObject GetRightAnkleObject => _rightAnkleObject;

        #endregion

        private void Update()
        {
            if (_dataStringToQuaternionHelper != null)
            {
                #region 上半身

                _chestObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST];
                
                _leftUpperArmObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM];
                _rightUpperArmObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM];
                
                _leftForeArmObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM];
                _rightForeArmObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM];

                #endregion
                
                #region 下半身

                _pelvisObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS];
                
                _leftThighObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_THIGH];
                _rightThighObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH];
                
                _leftCalfObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_CALF];
                _rightCalfObject.transform.rotation = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_CALF];

                #endregion
            }
        }
    }
}


