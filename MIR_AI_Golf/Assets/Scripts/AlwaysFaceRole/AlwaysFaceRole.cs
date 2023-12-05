using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.AssignDataHelper;
using EthanLin.Config;
using UnityEngine.UI;

namespace EthanLin
{
    public class AlwaysFaceRole : MonoBehaviour
    {
        [SerializeField] private int _sceneIndex;
        public int GetSceneIndex => _sceneIndex;
        
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        [SerializeField] private GameObject _spineForwardVector;
        
        // [Header("旋轉角色用的")] [SerializeField] private Text _rotateRoleSliderText;
        [Header("旋轉角色用的")] [SerializeField] private Slider _rotateRoleSlider;
        
        #region Normal Scene用的
        
        [Header("Normal Scene用的")] [SerializeField] private GameObject _cameraParent;
        public bool IsFaceRoleInNormalScene { private set; get; }

        #endregion

        #region AR Scene用的

        [Header("AR Scene用的")] [SerializeField] private GameObject _arCameraForwardVector;

        public float Different { private set; get; }
        public bool IsFaceRoleInArScene { private set; get; }

        #endregion
        
        
        private void Start()
        {
            Different = 0f;
            
            IsFaceRoleInNormalScene = false;
            IsFaceRoleInArScene = false;
            
            _rotateRoleSlider.transform.gameObject.SetActive(false);
        }
        
        public void SetRotateRoleSliderActive(bool aActive) => _rotateRoleSlider.transform.gameObject.SetActive(aActive);
        
        public void FaceRole()
        {
            if (_sceneIndex == 1)
            {
                _spineForwardVector.transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, Vector3.up);
                _cameraParent.transform.rotation = Quaternion.LookRotation(_spineForwardVector.transform.forward, Vector3.up);
                IsFaceRoleInNormalScene = true;

                float curAngle = Vector3.SignedAngle(_cameraParent.transform.forward, Vector3.forward, Vector3.up);
                _rotateRoleSlider.value = curAngle;
                // _rotateRoleSliderText.text = $"{curAngle}";
            }
        }
        
        public void GetDifferentAngle()
        {
            Vector3 arCameraForwardVector = Vector3.ProjectOnPlane(_arCameraForwardVector.transform.forward, Vector3.up);
            Vector3 chestForwardVector = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.back, Vector3.up);
            Different = Vector3.SignedAngle(chestForwardVector, arCameraForwardVector, Vector3.up);
            if (_sceneIndex == 2)
            {
                _rotateRoleSlider.value = Different;
                // _rotateRoleSliderText.text = $"{Different}";
            }
            Debug.Log($"{AllConfigs.DEBUG_TAG}, 需要補{Different}°");
            IsFaceRoleInArScene = true;
        }

        public void RotateCameraParent(float aValue)
        {
            if (_sceneIndex == 1)
            {
                _cameraParent.transform.rotation = Quaternion.AngleAxis(-aValue, Vector3.up);
                // _rotateRoleSliderText.text = $"{-aValue}";
            }
        }

        public void RotateRoleDirectly(float aValue)
        {
            if (_sceneIndex == 2)
            {
                Different = aValue;
                // _rotateRoleSliderText.text = $"{Different}";
            }
        }
    }
}