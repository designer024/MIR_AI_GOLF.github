using UnityEngine;

using EthanLin.Config;

namespace EthanLin.AndroidBluetoothLib
{
    public class BluetoothHelper : BluetoothAbstractHelper
    {
        public AndroidJavaObject deviceListJavaObject { private set; get; }
        public AndroidJavaObject deviceMapJavaObject { private set; get; }

        /// <summary>
        /// 建構子
        /// </summary>
        public BluetoothHelper() : base("com.ethanlin.btlibrary", "UnityBluetoothDataLib")
        {
            AndroidBluetoothJavaObject = AndroidBluetoothJavaClass.CallStatic<AndroidJavaObject>("getInstance");
            deviceListJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceList");
            deviceMapJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceMap");
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
                AndroidBluetoothJavaObject.Call("requestManageExternalStoragePermissionWithPath", AllConfigs.CSV_ROOT_FOLDER);
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
            deviceListJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceList");
            if (deviceListJavaObject != null)
            {
                return deviceListJavaObject.Call<int>("size");
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
            deviceMapJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceMap");
            if (deviceMapJavaObject != null)
            {
                return deviceMapJavaObject.Call<int>("size");
            }
            else
            {
                return -1;
            }
        }

        public AndroidJavaObject GetBluetoothDeviceFromMapWithAddress(string aDeviceAddress)
        {
            deviceMapJavaObject = AndroidBluetoothJavaClass.GetStatic<AndroidJavaObject>("deviceMap");
            return deviceMapJavaObject.Call<AndroidJavaObject>("get", aDeviceAddress);
        }

        public string GetDeviceName(AndroidJavaObject aAndroidJavaObject)
        {
            if (AndroidBluetoothJavaObject != null)
            {
                return AndroidBluetoothJavaObject.Call<string>("getDeviceName", aAndroidJavaObject);
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, AndroidBluetoothJavaObject is null!!!");
                return "AndroidInstanceJavaObject is null";
            }
        }
        
        public string GetDeviceAddress(AndroidJavaObject aAndroidJavaObject)
        {
            if (AndroidBluetoothJavaObject != null)
            {
                return AndroidBluetoothJavaObject.Call<string>("getDeviceAddress", aAndroidJavaObject);
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, AndroidBluetoothJavaObject is null!!!");
                return "AndroidInstanceJavaObject is null";
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
        /// 連接裝置 with list index
        /// </summary>
        /// <param name="aDeviceAddress">device address</param>
        public void ConnectBluetoothDevice(string aDeviceAddress)
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("connectBluetoothDevice", aDeviceAddress);
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

        /// <summary>
        /// 是否只用上半身
        /// </summary>
        /// <param name="aOnlyUpper"> true: 只用上半身</param>
        public void SwitchUpperOrFullBody(bool aOnlyUpper)
        {
            if (AndroidBluetoothJavaObject != null)
            {
                AndroidBluetoothJavaObject.Call("switchUpperOrFullBody", aOnlyUpper);
            }
            else
            {
                Debug.LogError($"{AllConfigs.DEBUG_TAG} Error, bluetooth library Java object is null!!!");
            }
        }
    }
}


