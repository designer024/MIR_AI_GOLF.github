using System.Collections;
using System.Collections.Generic;
using EthanLin.Config;
using UnityEngine;

namespace EthanLin
{
    public class DragTheMixedManager : MonoBehaviour
    {
        public void SetCanRotate(bool aCanRotate)
        {
            DragTheMixed dragTheMixed = GameObject.FindWithTag("DragTheMixed").GetComponent<DragTheMixed>();
            if (dragTheMixed != null)
            {
                dragTheMixed.CanRotateRole = aCanRotate;
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG}, DragTheMixed is null!");
            }
        }
    }
}