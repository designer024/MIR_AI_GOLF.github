using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace EthanLin
{
    public class ScanButtonUi : MonoBehaviour
    {
        [SerializeField] private Text _label;
        [SerializeField] private RawImage _icon;

        public void SetLabelAndIcon(Color aColor)
        {
            _label.color = aColor;
            _icon.color = aColor;
        }
        
        public void SetLabel(Color aColor) => _label.color = aColor;
        public void SetIcon(Color aColor) => _icon.color = aColor;
    }
}