using EthanLin.Config;
using UnityEngine;

namespace EthanLin
{
    /// <summary>
    /// 將預設的角色 Show 出來
    /// </summary>
    public class ModelMixedShowUp : MonoBehaviour
    {
        private void Start()
        {
            for (int i = 0 ; i < transform.childCount - 1; ++i)
            {
                transform.GetChild(i).transform.gameObject.SetActive(false);
            }

            transform.GetChild(AllConfigs.CURRENT_SELECTED_ROLE_INDEX).transform.gameObject.SetActive(true);
        }
    }
}