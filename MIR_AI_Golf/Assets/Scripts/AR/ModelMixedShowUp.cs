using UnityEngine;

namespace EthanLin
{
    /// <summary>
    /// 將 RoleSelectHelper 預設的角色 Show 出來
    /// </summary>
    public class ModelMixedShowUp : MonoBehaviour
    {
        private void Start()
        {
            RoleSelectHelper roleSelectHelper = GameObject.FindWithTag("RoleSelectHelper").GetComponent<RoleSelectHelper>();

            for (int i = 0 ; i < transform.childCount - 1; ++i)
            {
                transform.GetChild(i).transform.gameObject.SetActive(false);
            }

            transform.GetChild(roleSelectHelper.GetCurSelectedIndex).transform.gameObject.SetActive(true);
        }
    }
}