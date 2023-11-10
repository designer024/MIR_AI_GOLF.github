using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EthanLin.Config;

namespace EthanLin
{
    public class RoleSelectHelper : MonoBehaviour
    {
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
            SetAllButtonsUnselected();

            _currentSelectedIndex = aRoleIndex;
            
            _selectRoleButtons[_currentSelectedIndex].image.color = new Color(0f, 0.8f, 1f, 1f);
            GameObject curRole = Instantiate(_rolePrefabs[_currentSelectedIndex]);
            curRole.transform.position = Vector3.zero;
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