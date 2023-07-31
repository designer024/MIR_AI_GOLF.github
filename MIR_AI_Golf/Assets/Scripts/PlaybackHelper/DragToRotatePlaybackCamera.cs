using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.Playback
{
    public class DragToRotatePlaybackCamera : MonoBehaviour
    {
        [SerializeField] private PlaybackHelper _playbackHelper;
    
        private Vector3 _prevPos = Vector3.zero;
        private Vector3 _posDelta = Vector3.zero;

        // [SerializeField] private Toggle _canRotateToggle;

        private void Start()
        {
            transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            if (_playbackHelper.GetCanRotateToggle.isOn && _playbackHelper.OnSliderDragging == false)
            {
                if (Input.GetMouseButton(0))
                {
                    _posDelta = 0.5f * (Input.mousePosition - _prevPos);
                    if (Vector3.Dot(transform.up,Vector3.up) >= 0)
                    {
                        transform.Rotate(transform.up, -Vector3.Dot(_posDelta, Vector3.right), Space.World);
                    }
                    else
                    {
                        transform.Rotate(transform.up, Vector3.Dot(_posDelta, Vector3.right), Space.World);
                    }
            
                    // transform.Rotate(Vector3.right,Vector3.Dot(_posDelta,Vector3.up),Space.World);
                    transform.Rotate(transform.up,Vector3.Dot(_posDelta,Vector3.up),Space.World); // 只有饒著Y軸旋轉
                }

                _prevPos = Input.mousePosition;
            }
        }

        public void TurnOnOffToggle(bool aIsOn)
        {
            _playbackHelper.GetCanRotateToggle.isOn = aIsOn;
            if (aIsOn == false)
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }
}


