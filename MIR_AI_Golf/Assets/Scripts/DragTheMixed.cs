using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.AssignDataHelper;
using EthanLin.Config;

namespace EthanLin
{
    public class DragTheMixed : MonoBehaviour
    {
        [HideInInspector] public bool CanRotateRole;
	
        private Vector3 _prevPos = Vector3.zero;
        private Vector3 _posDelta = Vector3.zero;

        // private void Start() => CanRotateRole = false;
	
        private void Update ()
        {
            // Debug.Log($"{AllConfigs.DEBUG_TAG}, CanRotateRole: {CanRotateRole}");
            
            if (CanRotateRole)
            {
                if (Input.GetMouseButton(0))
                {
                    _posDelta = 0.5f * (Input.mousePosition - _prevPos);
                    if (Vector3.Dot(transform.up,Vector3.up) >= 0)
                    {
                        transform.Rotate(transform.up, -Vector3.Dot(_posDelta, Camera.main.transform.right), Space.World);
                    }
                    else
                    {
                        transform.Rotate(transform.up, Vector3.Dot(_posDelta, Camera.main.transform.right), Space.World);
                    }
            
                    // transform.Rotate(Camera.main.transform.right,Vector3.Dot(_posDelta,Camera.main.transform.up),Space.World);
                    transform.Rotate(transform.up,Vector3.Dot(_posDelta,Camera.main.transform.up),Space.World); // 只有饒著Y軸旋轉
                }

                _prevPos = Input.mousePosition;
            }
            else
            {
                // transform.forward = transform.parent.GetChild(AllConfigs.CURRENT_SELECTED_ROLE_INDEX).GetComponent<AssignDataToRoleHelper>().GetChestObject.transform.right;
            }
        }
    }
}