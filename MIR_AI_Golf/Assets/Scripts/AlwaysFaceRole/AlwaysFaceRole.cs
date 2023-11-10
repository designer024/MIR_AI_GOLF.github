using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.AssignDataHelper;
using EthanLin.Config;
using UnityEngine.Serialization;

namespace EthanLin
{
    public class AlwaysFaceRole : MonoBehaviour
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        [SerializeField] private GameObject _spineForwardVector;
        
        #region Normal Scene用的
        
        [Header("Normal Scene用的")] [SerializeField] private GameObject _cameraParent;
        public bool IsFaceRoleInNormalScene { private set; get; }

        #endregion

        #region AR Scene用的

        [Header("AR Scene用的")] [SerializeField] private GameObject _arCameraForwardVector;

        public float Different { private set; get; }
        public bool IsFaceRoleInArScene { private set; get; }
        
        /// <summary>
        /// 是否使用轉至面向AR相機
        /// </summary>
        public bool UseFixed { private set; get; }

        #endregion

        [SerializeField] private int _sceneIndex;
        public int GetSceneIndex => _sceneIndex;
        
        private void Start()
        {
            Different = 0f;
            
            IsFaceRoleInNormalScene = false;
            IsFaceRoleInArScene = false;
        }
        
        public void FaceRole()
        {
            if (_sceneIndex == 1)
            {
                _spineForwardVector.transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, Vector3.up);
                _cameraParent.transform.rotation = Quaternion.LookRotation(_spineForwardVector.transform.forward, Vector3.up);
                IsFaceRoleInNormalScene = true;
            }
            else if (_sceneIndex == 2)
            {
                IsFaceRoleInArScene = true;
                Debug.Log($"{AllConfigs.DEBUG_TAG}, It is AR Scene.");
            }
        }
        
        public void GetDifferentAngle()
        {
            Vector3 arCameraForwardVector = Vector3.ProjectOnPlane(_arCameraForwardVector.transform.forward, Vector3.up);
            Vector3 chestForwardVector = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.back, Vector3.up);
            Different = Vector3.SignedAngle(chestForwardVector, arCameraForwardVector, Vector3.up);
            Debug.Log($"{AllConfigs.DEBUG_TAG}, 需要補{Different}°");
            UseFixed = true;
        }
    }
}