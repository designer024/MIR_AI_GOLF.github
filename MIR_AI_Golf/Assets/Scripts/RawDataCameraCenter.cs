using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using  EthanLin.AssignDataHelper;
using EthanLin.Config;

namespace EthanLin
{
    public class RawDataCameraCenter : MonoBehaviour
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        
        private Vector3 _spineForwardVector;
        
        public void FaceRole()
        {
            _spineForwardVector = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, Vector3.up);
            this.transform.rotation = Quaternion.LookRotation(_spineForwardVector, Vector3.up);
        }
    }
}