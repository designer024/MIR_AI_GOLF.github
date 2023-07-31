using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using LitJson;

using EthanLin.CalibrateHelper;
using EthanLin.Config;

namespace EthanLin.JsonHelper
{
    public class JsonHelper : MonoBehaviour
    {
        private const string TAG = nameof(JsonHelper);

        [SerializeField] private CalibrateHelper_V2 _calibrateHelperV2;
        
        // [SerializeField] private Text _debugTxt;
        
        /// <summary>
        /// For ID to Name mapping
        /// </summary>
        private Dictionary<int, string> _bodyDictionary = new Dictionary<int, string>();
        
        /// <summary>
        /// 解析Json用的
        /// </summary>
        private JsonData _jsonData;

        private JsonInfo _jsonInfo;
        
        private void Start()
        {
            SetDictionary();
            
            // Debug.Log($"CSV Root Folder Path: {CSV_ROOT_FOLDER}");
            if (!Directory.Exists(AllConfigs.JSON_ROOT_FOLDER))
            {
                Directory.CreateDirectory(AllConfigs.JSON_ROOT_FOLDER);
            }

            // ExportToJson();

            // string jsonString = JsonMapper.ToJson(_jsonInfo);
            // _debugTxt.text = jsonString;
        }
        
        private void SetDictionary()
        {
            _bodyDictionary.Clear();
            
            #region 上半身
            
            _bodyDictionary.Add(AllPartNameIndex.LEFT_UPPER_ARM, "AllPartNameIndex.LEFT_UPPER_ARM");
            _bodyDictionary.Add(AllPartNameIndex.RIGHT_UPPER_ARM, "AllPartNameIndex.RIGHT_UPPER_ARM");
            _bodyDictionary.Add(AllPartNameIndex.CHEST, "AllPartNameIndex.CHEST");
            _bodyDictionary.Add(AllPartNameIndex.LEFT_FOREARM, "AllPartNameIndex.LEFT_FOREARM");
            _bodyDictionary.Add(AllPartNameIndex.RIGHT_FOREARM, "AllPartNameIndex.RIGHT_FOREARM");
            
            #endregion
            
            #region 下半身
            
            _bodyDictionary.Add(AllPartNameIndex.LEFT_THIGH, "AllPartNameIndex.LEFT_THIGH");
            _bodyDictionary.Add(AllPartNameIndex.RIGHT_THIGH, "AllPartNameIndex.RIGHT_THIGH");
            _bodyDictionary.Add(AllPartNameIndex.PELVIS, "AllPartNameIndex.PELVIS");
            _bodyDictionary.Add(AllPartNameIndex.LEFT_CALF, "AllPartNameIndex.LEFT_CALF");
            _bodyDictionary.Add(AllPartNameIndex.RIGHT_CALF, "AllPartNameIndex.RIGHT_CALF");
            
            #endregion
        }

        private void Update()
        {
            
        }

        public void LoadTestJson() => StartCoroutine(LoadTestJsonEnumerator());

        private IEnumerator LoadTestJsonEnumerator()
        {
            // if Android platform => file:///..... 
            // if MacOs => file://
            using (UnityWebRequest webRequest = UnityWebRequest.Get($"file://{AllConfigs.JSON_ROOT_FOLDER}20230524_095801.json"))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    DownloadHandler downloadHandler = webRequest.downloadHandler;
                    if (downloadHandler.isDone)
                    {
                        PutDataIntoJson(downloadHandler.text);
                    }
                }
                else
                {
                    Debug.LogError($"{TAG} There is no any json file!!!");
                }
            }
        }

        /// <summary>
        /// Parsing Json
        /// </summary>
        /// <param name="aJsonData">Json 內容</param>
        private void PutDataIntoJson(string aJsonData)
        {
            try
            {
                _jsonData = JsonMapper.ToObject<JsonData>(aJsonData);

                Vector3 jsonVector = new Vector3(float.Parse($"{_jsonData["BodyParts"][0]["sensorVectors"][3]["dirX"]}"), 0f, float.Parse($"{_jsonData["BodyParts"][0]["sensorVectors"][3]["dirZ"]}"));
                
                Debug.Log($"{TAG}, 哪兒呢: {_jsonData["where"]}, 地點: {_jsonData["location"]}\n版本: {_jsonData["version"]}, 有{_jsonData["BodyParts"].Count}個部位\n{_jsonData["BodyParts"][0]["sensorVectors"][3]["dirZ"]}, {jsonVector}");
            }
            catch (Exception aException)
            {
                Debug.LogError($"{TAG} Json error: {aException.Message}");
            }
        }

        public void ExportToJson()
        {
            // 板橋辦公室 25.0146895,121.4616512
            
            for (int i = 0; i < _calibrateHelperV2.GetSensorVectorDirectory.Count; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Debug.Log($"{TAG} sensor{i}: ({_calibrateHelperV2.GetSensorVectorDirectory[i][j].dirX}, 0f, {_calibrateHelperV2.GetSensorVectorDirectory[i][j].dirZ})");
                }
            }
            
            BodyPart[] bodyParts = new BodyPart[10];
            for (int i = 0; i < bodyParts.Length; ++i)
            {
                SensorVector[] sensorVectors = new SensorVector[4];
                for (int j = 0; j < 4; ++j)
                {
                    sensorVectors[j] = new SensorVector(_calibrateHelperV2.GetSensorVectorDirectory[i][j].dirX, _calibrateHelperV2.GetSensorVectorDirectory[i][j].dirZ);
                }
                
                bodyParts[i] = new BodyPart(_bodyDictionary[i + 1], sensorVectors);
            }
            
            _jsonInfo = new JsonInfo("taipei", "25.0146895#121.4616512", 2, bodyParts);
            string jsonString = JsonMapper.ToJson(_jsonInfo);
            
            File.WriteAllText($"{AllConfigs.JSON_ROOT_FOLDER}{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json", jsonString);
        }

        private string DataObjectToJson<T>(T t)
        {
            return JsonMapper.ToJson(t);
        }
    }
}


