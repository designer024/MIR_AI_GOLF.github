using UnityEngine;

using EthanLin.Config;

namespace EthanLin.AndroidBluetoothLib
{
    public class BluetoothHelper : BluetoothAbstractHelper
    {
        private AndroidJavaObject _deviceListJavaObject;
        private AndroidJavaObject _deviceMapJavaObject;

        /// <summary>
        /// 建構子
        /// </summary>
        public BluetoothHelper() : base("com.ethanlin.btlibrary", "UnityBluetoothDataLib")
        {
            AndroidBluetoothJavaObject = AndroidBluetoothJavaClass.CallStatic<AndroidJavaObject>("getInstance");
        }
        
        /// <summary>
        /// Init bluetooth manager
        /// </summary>
        public void InitBluetoothManager()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("initBluetoothManager", UnityContext);
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 請求藍牙權限
        /// </summary>
        public void RequestBluetoothPermission()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("requestBluetoothPermission");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 請求管理存取權限
        /// </summary>
        public void RequestManageExternalPermission()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("requestManageExternalStoragePermission");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 掃瞄藍牙裝置
        /// </summary>
        public void ScanBluetoothDevices()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("scanBluetoothDevices");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        public void StopScan()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("stopScan");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 取得device list size
        /// </summary>
        public int GetAndroidDeviceListSize()
        {
            _deviceListJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceList");
            if (_deviceListJavaObject != null)
            {
                return _deviceListJavaObject.Call<int>("size");
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 取得device map size
        /// </summary>
        public int GetAndroidDeviceMapSize()
        {
            _deviceMapJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceMap");
            if (_deviceMapJavaObject != null)
            {
                return _deviceMapJavaObject.Call<int>("size");
            }
            else
            {
                return -1;
            }
        }

        public AndroidJavaObject GetBluetoothDeviceFromMapWithAddress(string aDeviceAddress)
        {
            _deviceMapJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceMap");
            return _deviceMapJavaObject.Call<AndroidJavaObject>("get", aDeviceAddress);
        }

        public string GetAndroidDeviceListDeviceName(int aIndex)
        {
            _deviceListJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceList");
            if (_deviceListJavaObject != null)
            {
                return _deviceListJavaObject.Call<AndroidJavaObject>("get", aIndex).Call<string>("getName");
            }
            else
            {
                return "Nothing";
            }
        }

        public string GetAndroidDeviceListDeviceAddress(int aIndex)
        {
            _deviceListJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceList");
            if (_deviceListJavaObject != null)
            {
                return _deviceListJavaObject.Call<AndroidJavaObject>("get", aIndex).Call<string>("getAddress");
            }
            else
            {
                return "Nothing";
            }
        }

        /// <summary>
        /// 連接裝置 with list index
        /// </summary>
        /// <param name="aIndex">device list index</param>
        public void ConnectBluetoothDevice(int aIndex)
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("connectBluetoothDevice", aIndex);
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }
        
        /// <summary>
        /// 斷開所有藍牙裝置
        /// </summary>
        public void DisconnectAllBluetoothDevice()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("disconnectAllBluetoothDevice");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 開始或停止錄製
        /// </summary>
        public void StartStopRecord()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("startStopRecord");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }

        /// <summary>
        /// 螢幕擷圖Toast
        /// </summary>
        public void ToastToTellImageSaved()
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("toastToTellImageSaved");
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }
    }
}


