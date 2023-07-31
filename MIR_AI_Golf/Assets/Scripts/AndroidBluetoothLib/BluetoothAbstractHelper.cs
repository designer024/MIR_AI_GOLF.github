using UnityEngine;

namespace EthanLin.AndroidBluetoothLib
{
    public abstract class BluetoothAbstractHelper
    {
        private AndroidJavaObject _unityCurrentActivity;
        protected AndroidJavaObject UnityContext;
    
        protected AndroidJavaObject AndroidBluetoothJavaClass;
        protected AndroidJavaObject AndroidBluetoothJavaObject;

        protected BluetoothAbstractHelper(string aPackageName, string aClassName)
        {
            _unityCurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            UnityContext = _unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        
            AndroidBluetoothJavaClass = new AndroidJavaObject($"{aPackageName}.{aClassName}");
        }
    }
}


