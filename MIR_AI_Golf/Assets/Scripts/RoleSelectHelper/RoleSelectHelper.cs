using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.Config;
using EthanLin.AssignDataHelper;

namespace EthanLin
{
    public class RoleSelectHelper : MonoBehaviour
    {
        [SerializeField] private BallBallConfigHelperV2 _ballBallConfigHelperV2;
        [SerializeField] private BallBallV2Manager _ballBallV2Manager;
        
        /// <summary>
        /// 彩色角色要站的位置
        /// </summary>
        [Header("彩色角色要站的位置")] [SerializeField] private GameObject _colorfulRoleStandingPoint;
        /// <summary>
        /// 彩色角色要站的位置
        /// </summary>
        public GameObject GetColorfulRoleStandingPoint => _colorfulRoleStandingPoint;
        
        /// <summary>
        /// Role Mixed
        /// </summary>
        [Header("Role Mixed Normal場景用的")] [SerializeField] private GameObject _roleMixedObject;
        
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: Leonard")]
        [SerializeField] private GameObject[] _roleRawData;
        
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: Leonard")]
        [SerializeField] private GameObject[] _rolePrefabs;
        
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: Leonard")]
        [SerializeField] private Button[] _selectRoleButtons;
        
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: Leonard")]
        [SerializeField] private int _currentSelectedIndex = 0;

        private void Start() => SelectRole(_currentSelectedIndex);
        
        /// <summary>
        /// 選擇角色
        /// </summary>
        /// <param name="aRoleIndex">0: Miku, 1: 死侍, 2: 失敗的人, 3: Leonard</param>
        public void SelectRole(int aRoleIndex)
        {
            KillAllRole();
            KillAllRoleMixed();
            SetAllButtonsUnselected();

            _currentSelectedIndex = aRoleIndex;
            AllConfigs.CURRENT_SELECTED_ROLE_INDEX = _currentSelectedIndex;
            
            _selectRoleButtons[_currentSelectedIndex].image.color = new Color(0f, 0.8f, 1f, 1f);
            GameObject curRole = Instantiate(_rolePrefabs[_currentSelectedIndex]);
            // 設定位置
            curRole.transform.position = _ballBallConfigHelperV2.GetIsUsingBallBall ? new Vector3(100f, 0f, 0f) : _colorfulRoleStandingPoint.transform.position;
            _roleMixedObject.transform.position = _ballBallConfigHelperV2.GetIsUsingBallBall ? _colorfulRoleStandingPoint.transform.position : new Vector3(100f, 0f, 0f);
            
            _ballBallV2Manager.SetAssignDataToRoleHelper(curRole);

            _roleRawData[aRoleIndex].SetActive(true);
            
            _roleMixedObject.transform.GetChild(_currentSelectedIndex).transform.gameObject.SetActive(true);
            MakeTwoHandsCloserV2Manager makeTwoHandsCloserV2Manager = _roleMixedObject.transform.GetChild(_currentSelectedIndex).GetComponent<MakeTwoHandsCloserV2Manager>();
            if (makeTwoHandsCloserV2Manager != null)
            {
                makeTwoHandsCloserV2Manager.SetAssignDataToRoleHelper(curRole);
                _ballBallV2Manager.SetMakeTwoHandsCloserV2Manager(makeTwoHandsCloserV2Manager);
            }
        }

        /// <summary>
        /// Kill all role spawned
        /// </summary>
        private void KillAllRole()
        {
            GameObject[] roles = GameObject.FindGameObjectsWithTag("Role");
            if (roles != null && roles.Length > 0)
            {
                foreach (GameObject role in roles)
                {
                    Destroy(role);
                }
            }
            
            foreach (GameObject role in _roleRawData)
            {
                role.SetActive(false);
            }
        }

        private void KillAllRoleMixed()
        {
            for (int i = 0 ; i < _roleMixedObject.transform.childCount - 1; ++i)
            {
                _roleMixedObject.transform.GetChild(i).transform.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 將所有按鈕變回白色
        /// </summary>
        private void SetAllButtonsUnselected()
        {
            foreach (var btn in _selectRoleButtons)
            {
                btn.image.color = Color.white;
            }
        }
    }
}