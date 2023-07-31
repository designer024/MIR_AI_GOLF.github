using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EthanLin.Playback
{
    public class WhiteBoardLineRenderer : MonoBehaviour
    {
        // for debug
        // [SerializeField] private Text _text;
        
        public GameObject DrawPanel;
        
        [SerializeField] private GameObject _linePrefab;
        private GameObject _currentLine;
        private LineRenderer _lineRenderer;
        private List<Vector3> _positionsList = new List<Vector3>();
        
        [SerializeField] private Camera _drawCamera;
        
        /// <summary>
        /// 顯示當前筆刷顏色
        /// </summary>
        [SerializeField] private Image _currentBrushColorImage;
        /// <summary>
        /// 顯示當前筆刷顏色
        /// </summary>
        private Color _currentDrawColor = new Color(1f, 0.5f, 0f, 1f);

        /// <summary>
        /// set line width slider
        /// </summary>
        [SerializeField] private Slider _lineWidthSlider;
        /// <summary>
        /// set line width slider
        /// </summary>
        public Slider GetLineWidthSlider => _lineWidthSlider;
        /// <summary>
        /// 當前線粗
        /// </summary>
        private float _currentLineWidth = 0.005f;

        [SerializeField] private GameObject _selectColorPanel;
        private bool _isSelectColorOn;

        private readonly float _linePointZ = 0.5f;

        /// <summary>
        /// 0 = 普通模式, 4 = 畫矩形
        /// </summary>
        public static int DrawMode = 0;
        /// <summary>
        /// 普通模式按鈕
        /// </summary>
        [SerializeField] private Button _drawModeButton;
        /// <summary>
        /// 畫矩形按鈕
        /// </summary>
        [SerializeField] private Button _rectangleModeButton;
        private Vector3 _rectangleInitialPoint = Vector3.zero;
        private Vector3 _currentRectanglePoint = Vector3.zero;
        
        private void Start()
        {
            _currentBrushColorImage.color = _currentDrawColor;
            
            DrawPanel.SetActive(false);
            _isSelectColorOn = false;
            _selectColorPanel.SetActive(_isSelectColorOn);

            DrawMode = 0;
            SetDrawMode(DrawMode);
        }

        private void Update()
        {
            Debug.Log($"xxxxxxxxxxx, {PlaybackHelper.IsDrawing}");
            // Debug.Log($"xxxxxxxxxxx, {_isSelectColorOn}");
            
            if (DrawMode == 0)
            {
                if (Input.GetMouseButtonDown(0) && PlaybackHelper.IsDrawing)
                {
                    CreateLine();
                }

                if (Input.GetMouseButton(0) && PlaybackHelper.IsDrawing)
                {
                    Vector3 tempPos = Vector3.zero;
                    tempPos.x = _drawCamera.ScreenToWorldPoint(Input.mousePosition).x * -1f;
                    tempPos.y = _drawCamera.ScreenToWorldPoint(Input.mousePosition).y * 1f;
                    tempPos.z = _linePointZ;
                    UpdateLine(tempPos, _currentDrawColor, _currentLineWidth);
                }
            }
            else if (DrawMode == 4)
            {
                if (Input.GetMouseButtonDown(0) && PlaybackHelper.IsDrawing/* && PlaybackHelper.IsUsingPaintTool == false*/)
                {
                    CreateRectangleInitialPoint(_currentDrawColor, _currentLineWidth);
                }

                if (Input.GetMouseButton(0) && PlaybackHelper.IsDrawing/* && PlaybackHelper.IsUsingPaintTool == false*/)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _currentRectanglePoint.x = _drawCamera.ScreenToWorldPoint(Input.mousePosition).x * -1f;
                        _currentRectanglePoint.y = _drawCamera.ScreenToWorldPoint(Input.mousePosition).y * 1f;
            
                        _lineRenderer.SetPosition(0, new Vector3(_rectangleInitialPoint.x, _rectangleInitialPoint.y, _linePointZ));
                        _lineRenderer.SetPosition(1, new Vector3(_rectangleInitialPoint.x, _currentRectanglePoint.y, _linePointZ));
                        _lineRenderer.SetPosition(2, new Vector3(_currentRectanglePoint.x, _currentRectanglePoint.y, _linePointZ));
                        _lineRenderer.SetPosition(3, new Vector3(_currentRectanglePoint.x, _rectangleInitialPoint.y, _linePointZ));
                    }
                }
            }
        }

        #region 普通模式

        private void CreateLine()
        {
            _positionsList.Clear();
            
            _currentLine = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
            _lineRenderer = _currentLine.GetComponent<LineRenderer>();
            _lineRenderer.loop = false;
            _lineRenderer.material.color = _currentDrawColor;
            _lineRenderer.startWidth = _currentLineWidth;
            _lineRenderer.endWidth = _currentLineWidth;
            
            Vector3 point0 = Vector3.zero;
            point0.x = _drawCamera.ScreenToWorldPoint(Input.mousePosition).x * -1f;
            point0.y = _drawCamera.ScreenToWorldPoint(Input.mousePosition).y * 1f;
            point0.z = _linePointZ;
            _positionsList.Add(point0);
            Vector3 point1 = Vector3.zero;
            point1.x = _drawCamera.ScreenToWorldPoint(Input.mousePosition).x * -1f;
            point1.y = _drawCamera.ScreenToWorldPoint(Input.mousePosition).y * 1f;
            point1.z = _linePointZ;
            _positionsList.Add(point1);
            _lineRenderer.SetPosition(0, _positionsList[0]);
            _lineRenderer.SetPosition(1, _positionsList[1]);
        }

        private void UpdateLine(Vector3 aNewPos, Color aLineColor, float aLineWidth)
        {
            _positionsList.Add(aNewPos);
            _lineRenderer.positionCount++;
            _lineRenderer.material.color = aLineColor;
            _lineRenderer.startWidth = aLineWidth;
            _lineRenderer.endWidth = aLineWidth;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, aNewPos);
        }

        #endregion
        
        #region 畫矩形

        private void CreateRectangleInitialPoint(Color aLineColor, float aLineWidth)
        {
            _positionsList.Clear();
            
            _currentLine = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
            _lineRenderer = _currentLine.GetComponent<LineRenderer>();
            _lineRenderer.loop = true;
            _lineRenderer.positionCount = 4;
            _lineRenderer.material.color = aLineColor;
            _lineRenderer.startWidth = aLineWidth;
            _lineRenderer.endWidth = aLineWidth;
            _rectangleInitialPoint.x = _drawCamera.ScreenToWorldPoint(Input.mousePosition).x * -1f;
            _rectangleInitialPoint.y = _drawCamera.ScreenToWorldPoint(Input.mousePosition).y * 1f;
            _rectangleInitialPoint.z = _linePointZ;
            
            _lineRenderer.SetPosition(0, _rectangleInitialPoint);
            _lineRenderer.SetPosition(1, _rectangleInitialPoint);
            _lineRenderer.SetPosition(2, _rectangleInitialPoint);
            _lineRenderer.SetPosition(3, _rectangleInitialPoint);
        }

        #endregion

        /// <summary>
        /// 開啟選擇顏色面板
        /// </summary>
        public void TurnOnOffSelectColorPanel()
        {
            Debug.Log("TurnOnOffSelectColorPanel_0");
            _isSelectColorOn = !_isSelectColorOn;
            Debug.Log($"TurnOnOffSelectColorPanel_1_{_isSelectColorOn}");
            _selectColorPanel.SetActive(_isSelectColorOn);
            Debug.Log($"TurnOnOffSelectColorPanel_2_{_isSelectColorOn}");
        }

        /// <summary>
        /// 改變線的顏色
        /// </summary>
        public void SelectColor(Image aImageColor)
        {
            _currentDrawColor = aImageColor.color;
            _currentBrushColorImage.color = _currentDrawColor;
            _isSelectColorOn = false;
            _selectColorPanel.SetActive(_isSelectColorOn);
        }

        /// <summary>
        /// 改變線的顏色
        /// </summary>
        public void SelectColor(Color aDrawColor)
        {
            _currentDrawColor = aDrawColor;
            _currentBrushColorImage.color = _currentDrawColor;
        }

        /// <summary>
        /// 刪除所有的線
        /// </summary>
        /// <param name="aDeleteAll">true刪除所有的, false刪除最後一次繪製的</param>
        public void ClearAllLine(bool aDeleteAll)
        {
            GameObject[] linesDraw = GameObject.FindGameObjectsWithTag("DrawLine");
            if (linesDraw != null && linesDraw.Length > 0)
            {
                if (aDeleteAll)
                {
                    foreach (var line in linesDraw)
                    {
                        Destroy(line);
                    }
                }
                else
                {
                    Destroy(linesDraw[^1]);
                    Destroy(linesDraw[^2]);
                }
            }
        }

        /// <summary>
        /// Slide to set line width
        /// </summary>
        public void SetLineThickness(float aValue)
        {
            _currentLineWidth = aValue;
        }

        /// <summary>
        /// set draw mode
        /// </summary>
        /// <param name="aDrawMode">0 = 普通模式, 4 = 畫矩形</param>
        public void SetDrawMode(int aDrawMode)
        {
            DrawMode = aDrawMode;
            _drawModeButton.image.color = DrawMode == 0 ? Color.white : new Color(0.45f, 0.45f, 0.45f, 1f);
            _rectangleModeButton.image.color = DrawMode == 4 ? Color.white : new Color(0.45f, 0.45f, 0.45f, 1f);
        }
    }
}