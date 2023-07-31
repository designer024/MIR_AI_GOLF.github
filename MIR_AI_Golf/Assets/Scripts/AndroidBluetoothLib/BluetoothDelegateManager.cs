using UnityEngine;

namespace EthanLin.AndroidBluetoothLib
{
    public class BluetoothDelegateManager : MonoBehaviour
    {
        public delegate void BluetoothScanResultBehavior(bool aResult);
        public static event BluetoothScanResultBehavior scanDeviceResultEvent;
        public void UpdateScanResult(bool aResult) 
        {
            if (scanDeviceResultEvent != null) 
            {
                scanDeviceResultEvent(aResult);
            }
        }
    }
}


