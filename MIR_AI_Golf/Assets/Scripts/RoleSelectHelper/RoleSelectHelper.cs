using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

using EthanLin.Config;

namespace EthanLin
{
    public class RoleSelectHelper : MonoBehaviour
    {
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: CH31,\n4: UnityChan")]
        [SerializeField] private Button[] _selectRoleButtons;
        public Button[] GetSelectRoleButtons() { return _selectRoleButtons; }

        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: CH31,\n4: UnityChan")]
        [SerializeField] private int _curSelectedIndex = 2;

        public int GetCurSelectedIndex => _curSelectedIndex;

        private Vector3 _baseScale;
        [SerializeField] private Slider _roleScaleSlider;

        private void Start()
        {
            _selectRoleButtons[_curSelectedIndex].GetComponent<Image>().color = new Color(0f, 0.55f, 1f, 1f);
            
            _baseScale = Vector3.one;
        }
        
        public void SelectRole(int aIndex)
        {
            if (aIndex >= _selectRoleButtons.Length)
            {
                _curSelectedIndex = _selectRoleButtons.Length - 1;
                Debug.LogError($"{AllConfigs.DEBUG_TAG}, 哪來那麼多的角色可以選呢？！");
            }
            else if (aIndex < 0)
            {
                _curSelectedIndex = 0;
                Debug.LogError($"{AllConfigs.DEBUG_TAG}, 不要再亂輸入了？！");
            }
            else
            {
                _curSelectedIndex = aIndex;
            }

            SetAllButtonsUnselected();
            _selectRoleButtons[_curSelectedIndex].GetComponent<Image>().color = new Color(0f, 0.55f, 1f, 1f);

            SetAllRoleDisappear();
            
            GameObject spawnedObject = GameObject.FindWithTag("RoleMixed");
            if (spawnedObject != null)
            {
                spawnedObject.transform.GetChild(_curSelectedIndex).transform.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 將所有按鈕變回白色
        /// </summary>
        private void SetAllButtonsUnselected()
        {
            foreach (Button btn in _selectRoleButtons)
            {
                btn.GetComponent<Image>().color = Color.white;
            }
        }
        
        /// <summary>
        /// 將全部角色Model SetActive = false
        /// </summary>
        private void SetAllRoleDisappear()
        {
            GameObject spawnedObject = GameObject.FindGameObjectWithTag("RoleMixed");
            if (spawnedObject != null)
            {
                // 0 ～ childCount - 1 因為最後一個是相機
                for (int i = 0; i < spawnedObject.transform.childCount - 1; ++i)
                {
                    spawnedObject.transform.GetChild(i).transform.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// slide to set scale
        /// </summary>
        public void SliderToSetRoleScale(float aValue)
        {
            float newX = _baseScale.x * _roleScaleSlider.value;
            float newY = _baseScale.y * _roleScaleSlider.value;
            float newZ = _baseScale.z * _roleScaleSlider.value;
            
            GameObject spawnedObject = GameObject.FindGameObjectWithTag("RoleMixed");
            if (spawnedObject != null)
            {
                spawnedObject.transform.localScale = new Vector3(newX, newY, newZ);
            }
        }
    }
}