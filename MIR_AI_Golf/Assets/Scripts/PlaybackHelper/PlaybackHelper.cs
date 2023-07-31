using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EthanLin.Config;

namespace EthanLin.Playback
{
    public class PlaybackHelper : MonoBehaviour, IReadCsvButton
    {
        private const string TAG = nameof(PlaybackHelper);

        [SerializeField] private WhiteBoardLineRenderer _whiteBoardLineRenderer;
        
        [SerializeField] private Slider _playbackSlider;
        [SerializeField] private Text _currentPlayFrameText;

        
        /// <summary>
        /// load button text field
        /// </summary>
        [Header("載入按鈕之文字")] [SerializeField] private Text _loadButtonText;
        /// <summary>
        /// 目前選擇之記錄標題
        /// </summary>
        [Header("目前選擇之記錄標題")] [SerializeField] private Text _currentPlayedInfoText;
        /// <summary>
        /// 播放暫停按鈕
        /// </summary>
        [Header("播放暫停按鈕")] [SerializeField] private Button _playPauseButton;
        [SerializeField] private RawImage _playImage, _pauseImage;
        /// <summary>
        /// 開啟白板筆按鈕
        /// </summary>
        [Header("開啟白板筆按鈕")] [SerializeField] private Button _drawButton;

        /// <summary>
        /// 是否開啟畫筆功能
        /// </summary>
        public static bool IsDrawing { private set; get; }
        /// <summary>
        /// 是否開啟畫筆工具功能
        /// </summary>
        public static bool IsUsingPaintTool { private set; get; }
        public bool OnSliderDragging { private set; get; }

        public int TotalDataAmount { private set; get; }
        
        private int _chestTotalDataAmount;
        private int _luTotalDataAmount;
        private int _ruTotalDataAmount;
        private int _lfTotalDataAmount;
        private int _rfTotalDataAmount;
        private Quaternion[] _chestRecordQuaternions;
        private Quaternion[] _luRecordQuaternions;
        private Quaternion[] _ruRecordQuaternions;
        private Quaternion[] _lfRecordQuaternions;
        private Quaternion[] _rfRecordQuaternions;

        /// <summary>
        /// 當下的步數
        /// </summary>
        public int CurrentStep { private set; get; }

        

        /// <summary>
        /// 是否可以播放
        /// </summary>
        private bool _canBePlayed;
        /// <summary>
        /// 是否正在播放中
        /// </summary>
        private bool _isPlaying;

        private string _totalDataContent = "";
        private int _rootTotalDataAmount;
        private Dictionary<int, string> _totalDataCsvIndexDictionary = new Dictionary<int, string>();
        /// <summary>
        /// 存每個CSV檔案之內容Dictionary
        /// </summary>
        private Dictionary<string, string> _csvFileContentDictionary = new Dictionary<string, string>();

        /// <summary>
        /// load all csv animator
        /// </summary>
        [SerializeField] private Animator _loadAllCsvAnimator;
        /// <summary>
        /// 依總表內容記錄數量，對應生成相同數量之按鈕 prefab
        /// </summary>
        [SerializeField] private GameObject _readCsvDataButtonPrefab;
        /// <summary>
        /// 依總表內容記錄數量，對應生成相同數量之按鈕
        /// </summary>
        private GameObject[] _readCsvDataButtonsGameObjects;
        [SerializeField] private Transform _readCsvDataButtonTransform;

        /// <summary>
        /// 旋轉模型Toggle
        /// </summary>
        [SerializeField] private Toggle _canRotateToggle;
        /// <summary>
        /// 旋轉模型Toggle
        /// </summary>
        public Toggle GetCanRotateToggle => _canRotateToggle;
        /// <summary>
        /// 畫線的Toggle
        /// </summary>
        [SerializeField] private Toggle _drawLineToggle;
        /// <summary>
        /// 畫線的Toggle
        /// </summary>
        public Toggle GetDrawLineToggle => _drawLineToggle;

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
        
        #endregion
        
        private void Start()
        {
            Init();
        }

        private void OnDisable()
        {
            StopAllTask();
            Init();
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// init
        /// </summary>
        private void Init()
        {
            _loadButtonText.text = "LOAD";
            _playPauseButton.interactable = false;
            _drawLineToggle.transform.gameObject.SetActive(false);
            IsDrawing = false;
            IsUsingPaintTool = false;
            _drawButton.transform.gameObject.SetActive(false);
            _currentPlayedInfoText.text = "INFO";
            _playbackSlider.interactable = false;
            _canRotateToggle.isOn = false;

            ResetPlaybackSlider();

            DestroyAllReadButtons();

            SetAllJointObjectToIdentity();

            if (_chestRecordQuaternions != null)
            {
                _chestRecordQuaternions = null;
            }
            if (_luRecordQuaternions != null)
            {
                _luRecordQuaternions = null;
            }
            if (_ruRecordQuaternions != null)
            {
                _ruRecordQuaternions = null;
            }
            if (_lfRecordQuaternions != null)
            {
                _lfRecordQuaternions = null;
            }
            if (_rfRecordQuaternions != null)
            {
                _rfRecordQuaternions = null;
            }

            _chestTotalDataAmount = 0;
            _luTotalDataAmount = 0;
            _ruTotalDataAmount = 0;
            _lfTotalDataAmount = 0;
            _rfTotalDataAmount = 0;
            
            _canBePlayed = false;
            _isPlaying = false;
            _playImage.enabled = !_isPlaying;
            _pauseImage.enabled = _isPlaying;
        }

        /// <summary>
        /// 將所有關節物件 rotation 設為 identity
        /// </summary>
        private void SetAllJointObjectToIdentity()
        {
            _chestObject.transform.rotation = Quaternion.identity;
            _leftUpperArmObject.transform.rotation = Quaternion.identity;
            _rightUpperArmObject.transform.rotation = Quaternion.identity;
            _leftForeArmObject.transform.rotation = Quaternion.identity;
            _rightForeArmObject.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// 砍掉所有場景上的Read Csv Data Button
        /// </summary>
        private void DestroyAllReadButtons()
        {
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("ReadCsvDataButton");
            if (buttons != null && buttons.Length > 0)
            {
                foreach (var btn in buttons)
                {
                    Destroy(btn);
                }
            }
        }

        private void StopAllTask()
        {
            CancelInvoke();
            
            StopAllCoroutines();
        }

        /// <summary>
        /// 實作interface read csv button
        /// </summary>
        /// <param name="aDateTimeIndex"></param>
        public void OnReadCsvButtonPress(string aDateTimeIndex)
        {
            _drawLineToggle.transform.gameObject.SetActive(true);
            ResetPlaybackSlider();
            SetAllJointObjectToIdentity();
            _currentPlayedInfoText.text = $"{aDateTimeIndex}";
            
            StopCoroutine(ReadCustomCsvDataIEnumerator(aDateTimeIndex));
            StartCoroutine(ReadCustomCsvDataIEnumerator(aDateTimeIndex));
            
            _playPauseButton.interactable = true;
            _playbackSlider.interactable = true;
            
            // 開啟白板筆按鈕
            _drawButton.transform.gameObject.SetActive(true);
            
            // close scroll view
            _loadAllCsvAnimator.Play("closing");
            
            // Clear all line renderer
            var foundLineRenderObjects = FindObjectsOfType(typeof(LineRenderScripts));
            if (foundLineRenderObjects != null && foundLineRenderObjects.Length > 0)
            {
                for (int i = 0; i < foundLineRenderObjects.Length; ++i)
                {
                    var obj = (LineRenderScripts)foundLineRenderObjects[i];
                    obj.ClearAllLines();
                    obj.InitLineRender();
                }
            }
        }
        
        /// <summary>
        /// 載入總表
        /// </summary>
        public void LoadTotalSheetCsv()
        {
            DestroyAllReadButtons();
            
            StartCoroutine(LoadTotalSheetCsvEnumerator());
            
            _loadAllCsvAnimator.Play("opening");
        }
        /// <summary>
        /// 載入總表, 並產生選擇按鈕
        /// </summary>
        private IEnumerator LoadTotalSheetCsvEnumerator()
        {
            // if Android platform => file:///..... 
            // if MacOS => file://
            using (UnityWebRequest webRequest = UnityWebRequest.Get($"file:///{AllConfigs.CSV_ROOT_FOLDER}{AllConfigs.TOTAL_DATA_CSV_FILE_NAME}"))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    DownloadHandler downloadHandler = webRequest.downloadHandler;
                    if (downloadHandler.isDone)
                    {
                        _csvFileContentDictionary.Clear();
                        _totalDataCsvIndexDictionary.Clear();
                        if (_readCsvDataButtonsGameObjects != null)
                        {
                            _readCsvDataButtonsGameObjects = null;
                        }
                        _totalDataContent = downloadHandler.text;
                        // Debug.Log($"總表: {_totalDataContent}");
                        string[] totalData = _totalDataContent.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
                        if (totalData.Length >= 5)
                        {
                            _rootTotalDataAmount = (totalData.Length - 3) / 2;
                            // _totalSheetText.text = $"{_totalDataContent}共{_rootTotalDataAmount}筆";
                            _readCsvDataButtonsGameObjects = new GameObject[_rootTotalDataAmount];
                            for (int i = 0; i < _rootTotalDataAmount; ++i)
                            {
                                _totalDataCsvIndexDictionary.Add(i, totalData[(i + 1) * 2]);
                                // Debug.Log($"@@QQ@@ 0 第{i}筆: {_totalDataCsvIndexDictionary[i]}, {totalData[(i + 1) * 2 + 1]}");
                                _readCsvDataButtonsGameObjects[i] = Instantiate(_readCsvDataButtonPrefab, _readCsvDataButtonTransform);
                                _readCsvDataButtonsGameObjects[i].name = $"ReadCsvDataButton_{i}";
                                _readCsvDataButtonsGameObjects[i].transform.GetChild(0).GetComponent<Text>().text = $"{_totalDataCsvIndexDictionary[i]}";
                                _csvFileContentDictionary.Add(_totalDataCsvIndexDictionary[i], "blank");
                            }
                            _loadButtonText.text = "RELOAD";
                        }
                        else
                        {
                            Debug.LogError($"There is no any record!!!");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Error: {webRequest.error}");
                }
            }
        }
        
        private void ResetPlaybackSlider()
        {
            _playbackSlider.minValue = 0;
            _playbackSlider.maxValue = 1;
            _playbackSlider.value = 0;
        }
        
        private IEnumerator ReadCustomCsvDataIEnumerator(string aDateTimeIndex)
        {
            // if Android platform => file:///..... 
            // if MacOs => file://
            using (UnityWebRequest webRequest = UnityWebRequest.Get($"file:///{AllConfigs.CSV_ROOT_FOLDER}CSV_DATA_{aDateTimeIndex}.csv"))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    DownloadHandler downloadHandler = webRequest.downloadHandler;
                    if (downloadHandler.isDone)
                    {
                        // Debug.Log($"ooxxooxx {downloadHandler.text}");
                        string[] totalData = downloadHandler.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
                        // add to dictionary
                        _csvFileContentDictionary[aDateTimeIndex] = downloadHandler.text;
                        
                        TotalDataAmount = (totalData.Length - 11) / 10;
                        // _totalSheetText.text = $"Total Data Amount: {TotalDataAmount}";
                        if (TotalDataAmount > 0)
                        {
                            _chestRecordQuaternions = new Quaternion[TotalDataAmount];
                            _luRecordQuaternions = new Quaternion[TotalDataAmount];
                            _lfRecordQuaternions = new Quaternion[TotalDataAmount];
                            _ruRecordQuaternions = new Quaternion[TotalDataAmount];
                            _rfRecordQuaternions = new Quaternion[TotalDataAmount];
                            
                            for (int dataAmount = 0; dataAmount < TotalDataAmount - 1; ++dataAmount)
                            {
                                string[] chestQuaternions = totalData[(dataAmount + 1) * 10].Split('#');
                                string[] luQuaternions = totalData[(dataAmount + 1) * 10 + 1].Split('#');
                                string[] lfQuaternions = totalData[(dataAmount + 1) * 10 + 2].Split('#');
                                string[] ruQuaternions = totalData[(dataAmount + 1) * 10 + 3].Split('#');
                                string[] rfQuaternions = totalData[(dataAmount + 1) * 10 + 4].Split('#');
                                _chestRecordQuaternions[dataAmount] = new Quaternion(float.Parse(chestQuaternions[0]), float.Parse(chestQuaternions[1]), float.Parse(chestQuaternions[2]), float.Parse(chestQuaternions[3]));
                                _luRecordQuaternions[dataAmount] = new Quaternion(float.Parse(luQuaternions[0]), float.Parse(luQuaternions[1]), float.Parse(luQuaternions[2]), float.Parse(luQuaternions[3]));
                                _lfRecordQuaternions[dataAmount] = new Quaternion(float.Parse(lfQuaternions[0]), float.Parse(lfQuaternions[1]), float.Parse(lfQuaternions[2]), float.Parse(lfQuaternions[3]));
                                _ruRecordQuaternions[dataAmount] = new Quaternion(float.Parse(ruQuaternions[0]), float.Parse(ruQuaternions[1]), float.Parse(ruQuaternions[2]), float.Parse(ruQuaternions[3]));
                                _rfRecordQuaternions[dataAmount] = new Quaternion(float.Parse(rfQuaternions[0]), float.Parse(rfQuaternions[1]), float.Parse(rfQuaternions[2]), float.Parse(rfQuaternions[3]));
                            }
                            
                            _playbackSlider.maxValue = TotalDataAmount;
                            _currentPlayFrameText.text = $"1/{TotalDataAmount}";
                            _canBePlayed = true;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Error: {webRequest.error}");
                }
            }
        }

        /// <summary>
        /// play or pause csv data playback
        /// </summary>
        public void PlayPauseCsvData()
        {
            if (_canBePlayed)
            {
                _isPlaying = !_isPlaying;
                _playImage.enabled = !_isPlaying;
                _pauseImage.enabled = _isPlaying;
                if (_isPlaying)
                {
                    CancelInvoke(nameof(PauseCsvData));
                    InvokeRepeating(nameof(PlayCsvData), 0f, 0.02f);
                }
                else
                {
                    CancelInvoke(nameof(PlayCsvData));
                    InvokeRepeating(nameof(PauseCsvData), 0f, 1f);
                }
            }
        }

        /// <summary>
        /// 播放 Csv Data
        /// </summary>
        private void PlayCsvData()
        {
            if (CurrentStep < _playbackSlider.maxValue && _isPlaying)
            {
                _chestObject.transform.rotation = _chestRecordQuaternions[CurrentStep];
                
                _leftUpperArmObject.transform.rotation = _luRecordQuaternions[CurrentStep];
                _leftForeArmObject.transform.rotation = _lfRecordQuaternions[CurrentStep];
                    
                _rightUpperArmObject.transform.rotation = _ruRecordQuaternions[CurrentStep];
                _rightForeArmObject.transform.rotation = _rfRecordQuaternions[CurrentStep];
                
                CurrentStep++;
                _playbackSlider.value = CurrentStep;
                _currentPlayFrameText.text = $"{CurrentStep}/{TotalDataAmount}";
            }
            else if (CurrentStep == _playbackSlider.maxValue)
            {
                _playbackSlider.value = 0;
                CancelInvoke(nameof(PlayCsvData));
                _isPlaying = false;
                _playImage.enabled = !_isPlaying;
                _pauseImage.enabled = _isPlaying;
            }
        }

        private void PauseCsvData()
        {
            if (CurrentStep < _playbackSlider.maxValue && _isPlaying == false)
            {
                _chestObject.transform.rotation = _chestRecordQuaternions[CurrentStep];
                
                _leftUpperArmObject.transform.rotation = _luRecordQuaternions[CurrentStep];
                _leftForeArmObject.transform.rotation = _lfRecordQuaternions[CurrentStep];
                    
                _rightUpperArmObject.transform.rotation = _ruRecordQuaternions[CurrentStep];
                _rightForeArmObject.transform.rotation = _rfRecordQuaternions[CurrentStep];
            }
        }
        
        /// <summary>
        /// 可以直接拖接Slider
        /// </summary>
        public void OnSliderChanged(float aCurrentFrame)
        {
            if (_canBePlayed)
            {
                CurrentStep = (int)aCurrentFrame;

                if (CurrentStep < _playbackSlider.maxValue)
                {
                    _chestObject.transform.rotation = _chestRecordQuaternions[CurrentStep];
                
                    _leftUpperArmObject.transform.rotation = _luRecordQuaternions[CurrentStep];
                    _leftForeArmObject.transform.rotation = _lfRecordQuaternions[CurrentStep];
                
                    _rightUpperArmObject.transform.rotation = _ruRecordQuaternions[CurrentStep];
                    _rightForeArmObject.transform.rotation = _rfRecordQuaternions[CurrentStep];
                    
                    _currentPlayFrameText.text = $"{CurrentStep}/{TotalDataAmount}";
                }
                else if (CurrentStep == _playbackSlider.maxValue)
                {
                    _playbackSlider.value = 0;
                    CancelInvoke(nameof(PlayCsvData));
                    _isPlaying = false;
                    _playImage.enabled = !_isPlaying;
                    _pauseImage.enabled = _isPlaying;
                }
            }
        }

        /// <summary>
        /// it cannot rotate model when dragging playback slider
        /// </summary>
        public void SetSliderRotate(bool aIsOnSlider) => OnSliderDragging = aIsOnSlider;
        
        /// <summary>
        /// when using paint tools, stop painting
        /// </summary>
        /// <param name="aIsUsing"></param>
        public void SetIsUsingPaintTool(bool aIsUsing) => IsUsingPaintTool = aIsUsing;


        /// <summary>
        /// 開啟或關閉畫筆功能
        /// </summary>
        public void TurnOnOffDrawing()
        {
            _whiteBoardLineRenderer.ClearAllLine(true);
            
            IsDrawing = !IsDrawing;
            WhiteBoardLineRenderer.DrawMode = 0;
            _whiteBoardLineRenderer.SetDrawMode(WhiteBoardLineRenderer.DrawMode);
            _whiteBoardLineRenderer.DrawPanel.SetActive(IsDrawing);

            _canRotateToggle.interactable = !IsDrawing;
            if (_canRotateToggle.isOn)
            {
                _canRotateToggle.isOn = false;
            }
            _drawButton.image.color = IsDrawing ? Color.white : new Color(0.45f, 0.45f, 0.45f, 1f);
        }
        
        // public void TurnOffDrawing()
        // {
        //     IsDrawing = false;
        //     _displayDetailToggle.interactable = !IsDrawing;
        //     WhiteBoardLineRenderer.DrawMode = 0;
        //     _whiteBoardLineRenderer.SetDrawMode(WhiteBoardLineRenderer.DrawMode);
        //     _whiteBoardLineRenderer.DrawPanel.SetActive(IsDrawing);
        //     _canRotateToggle.interactable = !IsDrawing;
        //     _drawButton.image.color = new Color(0.45f, 0.45f, 0.45f, 1f);
        // }

        public void SetCanDraw(bool aIsOn) => _drawLineToggle.isOn = aIsOn;

        /// <summary>
        /// return to start up scene
        /// </summary>
        public void ReturnStartUpScene() => SceneManager.LoadScene(0);
    }
}

