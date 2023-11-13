using System;
using System.Collections;
using System.Collections.Generic;
using EthanLin.Config;
using UnityEngine;
using UnityEngine.UI;

namespace EthanLin.AssignDataHelper
{
    public class DataStringToQuaternionHelper : MonoBehaviour
    {
        // [SerializeField] private FaceRoleDelegateManager _faceRoleDelegateManager;
        [SerializeField] private AlwaysFaceRole _alwaysFaceRole;
        
        private Dictionary<int, Quaternion> _rawDataQuaternionDictionary = new Dictionary<int, Quaternion>();
        public Dictionary<int, Quaternion> GetRawDataQuaternionDictionary => _rawDataQuaternionDictionary;
        
        /// <summary>
        /// 是否有正確取得藍牙資料
        /// </summary>
        public bool GetBluetoothDataReceived { private set; get; }

        // [SerializeField] private Text _testText;
        // private bool _testChestData;

        private void Start()
        {
            // _testChestData = false;
            // _testText.text = $"{_testChestData}";
            InitQuaternionDictionary();
        }

        public void OnApplicationQuit()
        {
            // _testChestData = false;
            // _testText.text = $"{_testChestData}";
        }
        
        private void InitQuaternionDictionary()
        {
            _rawDataQuaternionDictionary.Clear();

            for (int i = 1; i < 11; ++i)
            {
                _rawDataQuaternionDictionary.Add(i, Quaternion.identity);
            }
        }

        public void ConvertDataStringToQuaternion(string aDataString)
        {
            if (aDataString.Contains("#"))
            {
                int upperId;
                // int downId;
                Quaternion tempUpperQuaternion;
                // Quaternion tempDownQuaternion;

                string[] stringArray = aDataString.Split('#');
                if (stringArray.Length == 5)
                // if (stringArray.Length == 10)
                {
                    // Debug.Log($"EthanLinBluetoothDataLib {aDataString}");

                    upperId = int.Parse(stringArray[0]);
                    tempUpperQuaternion.x = float.Parse(stringArray[1]);
                    tempUpperQuaternion.y = float.Parse(stringArray[2]);
                    tempUpperQuaternion.z = float.Parse(stringArray[3]);
                    tempUpperQuaternion.w = float.Parse(stringArray[4]);

                    _rawDataQuaternionDictionary[upperId] = tempUpperQuaternion;

                    if (upperId == AllPartNameIndex.CHEST)
                    {
                        // Normal模式下
                        if (_alwaysFaceRole.GetSceneIndex == 1 && _alwaysFaceRole.IsFaceRoleInNormalScene == false)
                        {
                            _alwaysFaceRole.FaceRole();
                        }
                    }
                    
                    // _rawDataQuaternionDictionary[int.Parse(stringArray[0])] = new Quaternion(float.Parse(stringArray[1]), float.Parse(stringArray[2]), float.Parse(stringArray[3]), float.Parse(stringArray[4]));
                    
                    // downId = int.Parse(stringArray[5]);
                    // tempDownQuaternion.x = float.Parse(stringArray[6]);
                    // tempDownQuaternion.y = float.Parse(stringArray[7]);
                    // tempDownQuaternion.z = float.Parse(stringArray[8]);
                    // tempDownQuaternion.w = float.Parse(stringArray[9]);
                    //
                    // _rawDataQuaternionDictionary[downId] = tempDownQuaternion;
                    // _rawDataQuaternionDictionary[int.Parse(stringArray[5])] = new Quaternion(float.Parse(stringArray[6]), float.Parse(stringArray[7]), float.Parse(stringArray[8]), float.Parse(stringArray[9]));
                    
                    // Debug.Log($"EthanLinBluetoothDataLib 上半身id: {int.Parse(stringArray[0])},  下半身id: {int.Parse(stringArray[5])}");
                    GetBluetoothDataReceived = true;
                }
                else
                {
                    GetBluetoothDataReceived = true;
                    Debug.LogError($"EthanLinBluetoothDataLib data string has problem!!, it is {aDataString}");
                }
            }
        }
    }
}


