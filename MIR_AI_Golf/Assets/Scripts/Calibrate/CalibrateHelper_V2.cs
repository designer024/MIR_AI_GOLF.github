using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.AssignDataHelper;
using EthanLin.Variation;
using EthanLin.JsonHelper;
using EthanLin.Config;

namespace EthanLin.CalibrateHelper
{
    public class CalibrateHelper_V2 : CalibrateConfig
    {
        [SerializeField] private DataStringToQuaternionHelper _dataStringToQuaternionHelper;
        [SerializeField] private VectorVariationManager _vectorVariationManager;

        /// <summary>
        /// For ID to RawData 部件之投影箭頭 mapping
        /// </summary>
        private Dictionary<int, GameObject> _bodyObjectsDictionary = new Dictionary<int, GameObject>();

        /// <summary>
        /// index 比照 SerialPortDataHandler, 1: 右前臂, 2: 右上臂, 3: 胸, 4: 左上臂, 5: 左前臂,
        /// 6: 左小腿, 7: 左大腿, 8: 臀, 9: 右大腿, 10: 右小腿
        /// </summary>
        private CalibrateTableHelper[] _calibrateTableHelpers;
        private Dictionary<int, CalibrateTableHelper> _calibrateTableHelperDictionary = new Dictionary<int, CalibrateTableHelper>();
        
        /// <summary>
        /// For Name to Quaternion mapping
        /// </summary>
        private Dictionary<int, Quaternion> _nameToQuaternionDictionary = new Dictionary<int, Quaternion>();
        /// <summary>
        /// For Name to Quaternion mapping
        /// </summary>
        public Dictionary<int, Quaternion> GetNameToQuaternionDictionary => _nameToQuaternionDictionary;

        /// <summary>
        /// 共需多少個參考方位校正, 目前4個
        /// </summary>
        private const int TOTAL_REF_DIRECTION = 4;
        /// <summary>
        /// 參考方位, 0: 北, 1: 東, 2: 南, 3: 西
        /// </summary>
        private string[] _calibrateRef = new string[] { "NORTH", "EAST", "SOUTH", "WEST" };

        /// <summary>
        /// 目前取得方位的次數
        /// </summary>
        private int _currentCalibrateIndex;
        
        /// <summary>
        /// 是否已校正
        /// </summary>
        // public bool IsCalibrated = false;
        
        /// <summary>
        /// 為了存Json用的
        /// </summary>
        private Dictionary<int, SensorVector[]> _sensorVectorDirectory = new Dictionary<int, SensorVector[]>();
        /// <summary>
        /// 為了存Json用的
        /// </summary>
        public Dictionary<int, SensorVector[]> GetSensorVectorDirectory => _sensorVectorDirectory;
        
        #region UI

        // [SerializeField] private Text _debugTxt;
        [SerializeField] private Button _calibrateButton;

        #endregion

        #region 箭頭部件

        // [SerializeField] private GameObject _chestObject;
        // public GameObject GetChestObject => _chestObject;
        [SerializeField] private GameObject _chestRawData_Projected;
        
        // [SerializeField] private GameObject _leftUpperArmObject, _leftForeArmObject;
        // public GameObject GetLeftUpperArmObject => _leftUpperArmObject;
        // public GameObject GetLeftForeArmObject => _leftForeArmObject;
        [SerializeField] private GameObject _leftUpperArmRawData_Projected;
        [SerializeField] private GameObject _leftForeArmRawData_Projected;
        
        // [SerializeField] private GameObject _rightUpperArmObject, _rightForeArmObject;
        // public GameObject GetRightUpperArmObject => _rightUpperArmObject;
        // public GameObject GetRightForeArmObject => _rightForeArmObject;
        [SerializeField] private GameObject _rightUpperArmRawData_Projected;
        [SerializeField] private GameObject _rightForeArmRawData_Projected;

        // [SerializeField] private GameObject _pelvisObject;
        // public GameObject GetPelvisObject => _pelvisObject;
        [SerializeField] private GameObject _pelvisRawData_Projected;

        // [SerializeField] private GameObject _leftThighObject, _leftCalfObject;
        // public GameObject GetLeftThighObject => _leftThighObject;
        // public GameObject GetLeftCalfObject => _leftCalfObject;
        [SerializeField] private GameObject _leftThighRawData_Projected;
        [SerializeField] private GameObject _leftCalfRawData_Projected;
        
        // [SerializeField] private GameObject _rightThighObject, _rightCalfObject;
        // public GameObject GetRightThighObject => _rightThighObject;
        // public GameObject GetRightCalfObject => _rightCalfObject;
        [SerializeField] private GameObject _rightThighRawData_Projected;
        [SerializeField] private GameObject _rightCalfRawData_Projected;

        #endregion

        private void Start()
        {
            // _dataStringToQuaternionHelper = GameObject.FindWithTag("DataStringToQuaternionHelper").GetComponent<DataStringToQuaternionHelper>();
            // _calibrateButton = GameObject.FindWithTag("CalibrateButton").GetComponent<Button>();
            
            // IsCalibrated = false;
            IsUsingAdvancedMode = false;
            
            SetDictionary();
            InitAllTableHelper();

            _currentCalibrateIndex = 0;
            _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = $"SET{System.Environment.NewLine}{_currentCalibrateIndex} {_calibrateRef[_currentCalibrateIndex]}";
            _calibrateButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            _calibrateButton.onClick.AddListener(SetDirections);
        }

        private void Update()
        {
            if (IsUsingAdvancedMode)
            {
                // only 上半身
                // for (int i = 1; i < 6; ++i)
                // {
                //     if (i == 3)
                //     {
                //         // 胸
                //         _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.forward * 1f, Vector3.up);
                //     }
                //     else
                //     {
                //         _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.up * -1f, Vector3.up);
                //     }
                // }
                
                // 全身
                for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
                {
                    // 站著
                    /*
                    if (i == 1)
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.up * -1f, Vector3.up);
                    }
                    else if (i == 2)
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.up * -1f, Vector3.up);
                    }
                    else if (i == 4)
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.up * -1f, Vector3.up);
                    }
                    else if (i == 5)
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.up * -1f, Vector3.up);
                    }
                    else
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[_bodyDictionary[i]] * Vector3.forward * 1f, Vector3.up);
                    }
                    */
                    
                    // 坐在地上
                    if (i == AllPartNameIndex.CHEST)
                    {
                        // 胸
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.forward * 1f, Vector3.up);
                    }
                    else if (i == AllPartNameIndex.PELVIS)
                    {
                        // 臀
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.forward * 1f, Vector3.up);
                    }
                    else
                    {
                        _bodyObjectsDictionary[i].transform.forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.up * -1f, Vector3.up);
                    }
                }

                #region 上半身

                Vector3 fixedChest_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.CHEST].GetTableNormalVector(_chestRawData_Projected.transform.forward, GetCurrentQuadrant(_chestRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.CHEST].SensorDirections)));
                Vector3 fixedChest_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.up, fixedChest_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.CHEST] = Quaternion.LookRotation(fixedChest_Forward, fixedChest_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedLU_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_UPPER_ARM].GetTableNormalVector(_leftUpperArmRawData_Projected.transform.forward, GetCurrentQuadrant(_leftUpperArmRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_UPPER_ARM].SensorDirections)));
                Vector3 fixedLU_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.forward,fixedLU_Up);
                _nameToQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] = Quaternion.LookRotation(fixedLU_Forward, fixedLU_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedRU_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_UPPER_ARM].GetTableNormalVector(_rightUpperArmRawData_Projected.transform.forward, GetCurrentQuadrant(_rightUpperArmRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_UPPER_ARM].SensorDirections)));
                Vector3 fixedRU_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.forward,fixedRU_Up);
                _nameToQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] = Quaternion.LookRotation(fixedRU_Forward, fixedRU_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedLF_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_FOREARM].GetTableNormalVector(_leftForeArmRawData_Projected.transform.forward, GetCurrentQuadrant(_leftForeArmRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_FOREARM].SensorDirections)));
                Vector3 fixedLF_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.forward, fixedLF_Up);
                _nameToQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] = Quaternion.LookRotation(fixedLF_Forward, fixedLF_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedRF_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_FOREARM].GetTableNormalVector(_rightForeArmRawData_Projected.transform.forward, GetCurrentQuadrant(_rightForeArmRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_FOREARM].SensorDirections)));
                Vector3 fixedRF_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.forward, fixedRF_Up);
                _nameToQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] = Quaternion.LookRotation(fixedRF_Forward, fixedRF_Up);
                // TODO: 判斷指向位於水平或垂直向上向下

                #endregion

                #region 下半身

                Vector3 fixedPelvis_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.PELVIS].GetTableNormalVector(_pelvisRawData_Projected.transform.forward, GetCurrentQuadrant(_pelvisRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.PELVIS].SensorDirections)));
                Vector3 fixedPelvis_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.up, fixedPelvis_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.PELVIS] = Quaternion.LookRotation(fixedPelvis_Forward, fixedPelvis_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedLT_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_THIGH].GetTableNormalVector(_leftThighRawData_Projected.transform.forward, GetCurrentQuadrant(_leftThighRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_THIGH].SensorDirections)));
                Vector3 fixedLT_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] * Vector3.forward,fixedLT_Up);
                // Vector3 fixedLT_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.LEFT_THIGH] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_THIGH].GetTableNormalVector(_leftThighRawData_Projected.transform.forward, GetCurrentQuadrant(_leftThighRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_THIGH].SensorDirections)));
                // Vector3 fixedLT_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.LEFT_THIGH] * Vector3.up, fixedLT_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.LEFT_THIGH] = Quaternion.LookRotation(fixedLT_Forward, fixedLT_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedRT_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_THIGH].GetTableNormalVector(_rightThighRawData_Projected.transform.forward, GetCurrentQuadrant(_rightThighRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_THIGH].SensorDirections)));
                Vector3 fixedRT_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] * Vector3.forward,fixedRT_Up);
                // Vector3 fixedRT_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.RIGHT_THIGH] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_THIGH].GetTableNormalVector(_rightThighRawData_Projected.transform.forward, GetCurrentQuadrant(_rightThighRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_THIGH].SensorDirections)));
                // Vector3 fixedRT_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.RIGHT_THIGH] * Vector3.up, fixedRT_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.RIGHT_THIGH] = Quaternion.LookRotation(fixedRT_Forward, fixedRT_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedLC_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_CALF] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_CALF].GetTableNormalVector(_leftCalfRawData_Projected.transform.forward, GetCurrentQuadrant(_leftCalfRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_CALF].SensorDirections)));
                Vector3 fixedLC_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_CALF] * Vector3.forward,fixedLC_Up);
                // Vector3 fixedLC_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.LEFT_CALF] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_CALF].GetTableNormalVector(_leftCalfRawData_Projected.transform.forward, GetCurrentQuadrant(_leftCalfRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.LEFT_CALF].SensorDirections)));
                // Vector3 fixedLC_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.LEFT_CALF] * Vector3.up, fixedLC_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.LEFT_CALF] = Quaternion.LookRotation(fixedLC_Forward, fixedLC_Up);
                // TODO: 判斷指向位於水平或垂直向上向下
                
                Vector3 fixedRC_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] * Vector3.up, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_CALF].GetTableNormalVector(_rightCalfRawData_Projected.transform.forward, GetCurrentQuadrant(_rightCalfRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_CALF].SensorDirections)));
                Vector3 fixedRC_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] * Vector3.forward,fixedRC_Up);
                // Vector3 fixedRC_Forward = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.RIGHT_CALF] * Vector3.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_CALF].GetTableNormalVector(_rightCalfRawData_Projected.transform.forward, GetCurrentQuadrant(_rightCalfRawData_Projected.transform.forward, _calibrateTableHelperDictionary[AllPartNameIndex.RIGHT_CALF].SensorDirections)));
                // Vector3 fixedRC_Up = Vector3.ProjectOnPlane(_dataStringToQuaternionHelper.GetNameToQuaMappingDictionary[AllPartNameIndex.RIGHT_CALF] * Vector3.up, fixedRC_Forward);
                _nameToQuaternionDictionary[AllPartNameIndex.RIGHT_CALF] = Quaternion.LookRotation(fixedRC_Forward, fixedRC_Up);
                // TODO: 判斷指向位於水平或垂直向上向下

                #endregion
            }
        }

        /// <summary>
        ///  Set ID to Name
        /// </summary>
        private void SetDictionary()
        {
            _bodyObjectsDictionary.Clear();
            _nameToQuaternionDictionary.Clear();
            
            #region 上半身
            
            _bodyObjectsDictionary.Add(AllPartNameIndex.LEFT_UPPER_ARM, _leftUpperArmRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.RIGHT_UPPER_ARM, _rightUpperArmRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.CHEST, _chestRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.LEFT_FOREARM, _leftForeArmRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.RIGHT_FOREARM, _rightForeArmRawData_Projected);
            
            _nameToQuaternionDictionary.Add(AllPartNameIndex.LEFT_UPPER_ARM, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.RIGHT_UPPER_ARM, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.CHEST, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.LEFT_FOREARM, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.RIGHT_FOREARM, Quaternion.identity);
            
            #endregion
            
            #region 下半身
            
            _bodyObjectsDictionary.Add(AllPartNameIndex.LEFT_THIGH, _leftThighRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.RIGHT_THIGH, _rightThighRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.PELVIS, _pelvisRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.LEFT_CALF, _leftCalfRawData_Projected);
            _bodyObjectsDictionary.Add(AllPartNameIndex.RIGHT_CALF, _rightCalfRawData_Projected);
            
            _nameToQuaternionDictionary.Add(AllPartNameIndex.LEFT_THIGH, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.RIGHT_THIGH, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.PELVIS, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.LEFT_CALF, Quaternion.identity);
            _nameToQuaternionDictionary.Add(AllPartNameIndex.RIGHT_CALF, Quaternion.identity);
            
            #endregion
        }
        
        /// <summary>
        /// 由 SerialPortDataHandler 去呼叫
        /// </summary>
        private void InitAllTableHelper()
        {
            // 因為是用 1 ～ 10
            _calibrateTableHelpers = new CalibrateTableHelper[11];
            
            for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
            {
                _calibrateTableHelpers[i] = new CalibrateTableHelper(i, TOTAL_REF_DIRECTION);
                _calibrateTableHelperDictionary.Add(i, _calibrateTableHelpers[i]);
            }
        }

        /// <summary>
        /// 目前依序北、東、南、西各取sensor之向量 坐在地上
        /// </summary>
        private void SetDirections()
        {
            // _debugTxt.text = "";

            if (IsUsingAdvancedMode == false)
            {
                // 全身
                for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
                {
                    Vector3 dir = Vector3.zero;

                    if (i == AllPartNameIndex.CHEST)
                    {
                        // 胸不是用up指向
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward * 1f;
                    }
                    else if (i == AllPartNameIndex.PELVIS)
                    {
                        // 臀不是用up指向
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.PELVIS] * Vector3.forward * 1f;
                    }
                    else
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.up * -1f;
                    }
                    
                    _calibrateTableHelperDictionary[i].SetDirections(_currentCalibrateIndex, dir);
                }
                
                // only 上半身的就好了
                // for (int i = 1; i < 6; ++i)
                // {
                //     Vector3 dir = Vector3.zero;
                //     
                //     if (i == AllPartNameIndex.CHEST)
                //     {
                //         // 胸不是用up指向
                //         dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.CHEST] * Vector3.forward * 1f;
                //     }
                //     else
                //     {
                //         dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.up * -1f;
                //     }
                //     
                //     _calibrateTableHelperDictionary[i].SetDirections(_currentCalibrateIndex, dir);
                // }
                _currentCalibrateIndex++;
                
                if (_currentCalibrateIndex >= TOTAL_REF_DIRECTION)
                {
                    for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
                    {
                        _calibrateTableHelperDictionary[i].GetQuadrantAngles();
                    }
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = "CALIBRATED";
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().color = Color.yellow;
                    
                    // debug
                    // _debugTxt.text = $"部位:\n{_calibrateTableHelperDictionary[_bodyDictionary[1]].PartName}\n{_calibrateTableHelperDictionary[_bodyDictionary[1]].QuadrantAngles.Length}, {_calibrateTableHelperDictionary[_bodyDictionary[1]].SensorDirections.Length}";
                    // for (int i = 0 ; i < TOTAL_REF_DIRECTION; ++i)
                    // {
                    //     _debugTxt.text += $"{_calibrateTableHelperDictionary[_bodyDictionary[1]].SensorDirections[i]}\n";
                    //     _debugTxt.text += $"{_calibrateTableHelperDictionary[_bodyDictionary[1]].QuadrantAngles[i]}度\n";
                    // }
                    
                    SetJsonDictionary();

                    // IsCalibrated = true;
                    IsUsingAdvancedMode = true;
                    IsUsingNormalMode = false;
                    _vectorVariationManager.InitAllTableHelperCalibrateHelper();
                }
                else
                {
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = $"SET\n{_currentCalibrateIndex} {_calibrateRef[_currentCalibrateIndex]}";
                }
            }
            else
            {
                // IsCalibrated = false;
                IsUsingAdvancedMode = false;
                _currentCalibrateIndex = 0;
                _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = $"SET\n{_currentCalibrateIndex} {_calibrateRef[_currentCalibrateIndex]}";
                _calibrateButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                
                _vectorVariationManager.InitAllTableHelperDataStringToQuaternionHelper();
            }
        }
        
        /// <summary>
        /// 目前依序北、東、南、西各取sensor之向量 站著
        /// </summary>
        private void SetDirections2()
        {
            // if (IsCalibrated == false)
            if (IsUsingAdvancedMode == false)
            {
                // 全身
                for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
                {
                    Vector3 dir = Vector3.zero;

                    if (i == AllPartNameIndex.LEFT_UPPER_ARM)
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_UPPER_ARM] * Vector3.up * -1f;
                    }
                    else if (i == AllPartNameIndex.LEFT_FOREARM)
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.LEFT_FOREARM] * Vector3.up * -1f;
                    }
                    else if (i == AllPartNameIndex.RIGHT_UPPER_ARM)
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_UPPER_ARM] * Vector3.up * -1f;
                    }
                    else if (i == AllPartNameIndex.RIGHT_FOREARM)
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[AllPartNameIndex.RIGHT_FOREARM] * Vector3.up * -1f;
                    }
                    else
                    {
                        dir = _dataStringToQuaternionHelper.GetRawDataQuaternionDictionary[i] * Vector3.forward * 1f;
                    }
                    
                    _calibrateTableHelperDictionary[i].SetDirections(_currentCalibrateIndex, dir);
                }
                
                _currentCalibrateIndex++;
                
                if (_currentCalibrateIndex >= TOTAL_REF_DIRECTION)
                {
                    for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
                    {
                        _calibrateTableHelperDictionary[i].GetQuadrantAngles();
                    }
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = "CALIBRATED";
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().color = Color.blue;
                    
                    // debug
                    // _debugTxt.text = $"部位:\n{_calibrateTableHelperDictionary[_bodyDictionary[1]].PartName}\n{_calibrateTableHelperDictionary[_bodyDictionary[1]].QuadrantAngles.Length}, {_calibrateTableHelperDictionary[_bodyDictionary[1]].SensorDirections.Length}";
                    // for (int i = 0 ; i < TOTAL_REF_DIRECTION; ++i)
                    // {
                    //     _debugTxt.text += $"{_calibrateTableHelperDictionary[_bodyDictionary[1]].SensorDirections[i]}\n";
                    //     _debugTxt.text += $"{_calibrateTableHelperDictionary[_bodyDictionary[1]].QuadrantAngles[i]}度\n";
                    // }
                    
                    SetJsonDictionary();

                    // IsCalibrated = true;
                    IsUsingAdvancedMode = true;
                }
                else
                {
                    _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = $"SET\n{_currentCalibrateIndex} {_calibrateRef[_currentCalibrateIndex]}";
                }
            }
            else
            {
                // IsCalibrated = false;
                IsUsingAdvancedMode = false;
                _currentCalibrateIndex = 0;
                _calibrateButton.transform.GetChild(0).GetComponent<Text>().text = $"SET\n{_currentCalibrateIndex} {_calibrateRef[_currentCalibrateIndex]}";
                _calibrateButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                
                _vectorVariationManager.InitAllTableHelperDataStringToQuaternionHelper();
            }
        }
        
        /// <summary>
        /// 儲存將各部位取得的各方位向量
        /// </summary>
        private void SetJsonDictionary()
        {
            _sensorVectorDirectory.Clear();
            
            // 因為_calibrateTableHelpers有11個
            for (int i = 1; i < _calibrateTableHelpers.Length; ++i)
            {
                SensorVector[] vectors = new SensorVector[4];
                vectors[0] = new SensorVector(_calibrateTableHelperDictionary[i].SensorDirections[0].x, _calibrateTableHelperDictionary[i].SensorDirections[0].z);
                vectors[1] = new SensorVector(_calibrateTableHelperDictionary[i].SensorDirections[1].x, _calibrateTableHelperDictionary[i].SensorDirections[1].z);
                vectors[2] = new SensorVector(_calibrateTableHelperDictionary[i].SensorDirections[2].x, _calibrateTableHelperDictionary[i].SensorDirections[2].z);
                vectors[3] = new SensorVector(_calibrateTableHelperDictionary[i].SensorDirections[3].x, _calibrateTableHelperDictionary[i].SensorDirections[3].z);
                _sensorVectorDirectory.Add(i - 1, vectors);
            }
        }
        
        /// <summary>
        /// 取得當前RawData手臂指向位於的象限, 使用Vector x z 比較
        /// </summary>
        /// <param name="aDirectionProjected">當前RawData之投影向量</param>
        /// <param name="aSensorNESW">該身體位置的四個方位</param>
        /// <returns>0: 北-東, 1: 東-南, 2: 南-西, 3: 西-北, -1: I don't know</returns>
        private int GetCurrentQuadrant(Vector3 aDirectionProjected, Vector3[] aSensorNESW)
        {
            if (aDirectionProjected.x > aSensorNESW[0].x && aDirectionProjected.z > aSensorNESW[1].z)
            {
                // 北-東
                return 0;
            }
            else if (aDirectionProjected.x > aSensorNESW[2].x && aDirectionProjected.z <= aSensorNESW[1].z)
            {
                // 東-南
                return 1;
            }
            else if (aDirectionProjected.x <= aSensorNESW[2].x && aDirectionProjected.z <= aSensorNESW[3].z)
            {
                // 南-西
                return 2;
            }
            else if (aDirectionProjected.x <= aSensorNESW[0].x && aDirectionProjected.z > aSensorNESW[3].z)
            {
                // 西-北
                return 3;
            }
            else
            {
                return -1;
            }
        }
    }
}


