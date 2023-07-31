using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EthanLin.Playback
{
    public class LineRenderScripts : MonoBehaviour
    {
        // [SerializeField] private GameObject[] _testPoints;
        //
        // private LineRenderer[] _lineRenderers;
        //
        // private int[] _linePointsIndex;
        //
        // private int[] _lengthOfLineRenderers;
        //
        // [Header("顏色數量須與Test Points一樣")] [SerializeField] private Color[] _lineColors;

        [SerializeField] private PlaybackHelper _playbackHelper;

        [SerializeField] private GameObject _drawPoint;
        [SerializeField] private LineRenderer _lineRenderer;
        public int _drawPointIndex;
        public int _lengthOfLineRenderer;
        [Header("線的顏色")] [SerializeField] private Color _lineColor;

        /// <summary>
        /// 要不要劃線
        /// </summary>
        private bool _canDrawLine = false;

        // [SerializeField] private bool DrawLine = false;


        private void Start()
        {
            _drawPoint = transform.gameObject;

            _canDrawLine = false;

            if (_playbackHelper == null)
            {
                _playbackHelper = GameObject.FindWithTag("PlaybackHelper").GetComponent<PlaybackHelper>();
            }
            
            InitLineRender();
        }

        private void FixedUpdate()
        {
            if (_canDrawLine && _playbackHelper.GetDrawLineToggle.isOn && _playbackHelper.CurrentStep > 10 && _playbackHelper.CurrentStep < _playbackHelper.TotalDataAmount - 10)
            {
                _lineRenderer.startWidth = 0.01f;
                _lineRenderer.endWidth = 0.01f;

                _lengthOfLineRenderer++;
                _lineRenderer.positionCount = _lengthOfLineRenderer;

                while (_drawPointIndex < _lengthOfLineRenderer)
                {
                    _lineRenderer.SetPosition(_drawPointIndex, _drawPoint.transform.position);
                    _drawPointIndex++;
                }
            }
        }

        public void InitLineRender()
        {
            if (_canDrawLine == false)
            {
                // _lineRenderer = _drawPoint.GetComponent<LineRenderer>();
                
                _drawPointIndex = 0;
                _lengthOfLineRenderer = 0;

                _lineRenderer.useWorldSpace = true;
                // set line color
                _lineRenderer.material.color = _lineColor;
                
                _lineRenderer.startWidth = 0f;
                _lineRenderer.endWidth = 0f;
                _lineRenderer.SetPosition(_drawPointIndex, _drawPoint.transform.position);
                
                _canDrawLine = true;
            }
        }

        public void ClearAllLines()
        {
            _lineRenderer.positionCount = 0;

            _canDrawLine = false;
        }
    }
}