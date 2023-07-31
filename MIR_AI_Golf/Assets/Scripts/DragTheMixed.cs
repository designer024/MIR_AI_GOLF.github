using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.AssignDataHelper;

namespace EthanLin
{
    public class DragTheMixed : MonoBehaviour
    {
        [HideInInspector] public bool CanRotateRole;
	
        private Vector3 _prevPos = Vector3.zero;
        private Vector3 _posDelta = Vector3.zero;

        private void Start() => CanRotateRole = true;
	
        private void Update ()
        {
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
                int currentRole = GameObject.FindWithTag("RoleSelectHelper").GetComponent<RoleSelectHelper>().GetCurSelectedIndex;
                transform.forward = transform.parent.GetChild(currentRole).GetComponent<AssignDataToRoleHelper>().GetChestObject.transform.right;
            }
        }
    }
}