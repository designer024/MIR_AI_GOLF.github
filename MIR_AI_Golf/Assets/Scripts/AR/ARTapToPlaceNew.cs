using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace EthanLin
{
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARTapToPlaceNew : MonoBehaviour
    {
        [SerializeField] private ARPlaneManager _arPlaneManager;
        
        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private RoleSelectHelper _roleSelectHelper;
        
        [SerializeField] private GameObject _objectToPlace;

        
        private Vector2 touchPosition;

        private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private void Start()
        {
            DestroyAllArObject();
            
            foreach (var btn in _roleSelectHelper.GetSelectRoleButtons())
            {
                btn.interactable = false;
            }
        }

        private void Update()
        {
            if (!CanGetTouchPosition(out Vector2 touchPosition) || !transform.gameObject.GetComponent<ARPlaneManager>().enabled)
            {
                return;
            }

            if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                // DestroyAllArObject();
                
                var hitPose = hits[0].pose;

                GameObject mixed = Instantiate(_objectToPlace, hitPose.position, hitPose.rotation);
                foreach (Button btn in _roleSelectHelper.GetSelectRoleButtons())
                {
                    btn.interactable = true;
                }
                TogglePlaneDetection();
                
                // else
                // {
                //     // 移動位置
                //     _spawnedObject.transform.position = hitPose.position;
                // }
            }
        }
        private bool CanGetTouchPosition(out Vector2 touchPosition)
        {
            touchPosition = default;

            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            return false;
        }

        private void TogglePlaneDetection()
        {
            _arPlaneManager.enabled = !_arPlaneManager.enabled;

            foreach (ARPlane plane in _arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(_arPlaneManager.enabled);
            }
        }    
        
        /// <summary>
        /// 將Mixed GameObject 砍掉
        /// </summary>
        private void DestroyAllArObject()
        {
            GameObject[] rolesMixed = GameObject.FindGameObjectsWithTag("RoleMixed");
            if (rolesMixed != null && rolesMixed.Length > 0)
            {
                foreach (var role in rolesMixed)
                {
                    Destroy(role);
                }
            }

            GameObject[] roles = GameObject.FindGameObjectsWithTag("Role");
            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    Destroy(role);
                }
            }
        }
    }
}


