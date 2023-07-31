using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using EthanLin.AndroidBluetoothLib;
using EthanLin.Config;

namespace EthanLin
{
    public class ScreenShotHelper : MonoBehaviour
    {
        [SerializeField] private BluetoothManager _bluetoothManager;
    
        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!Directory.Exists(AllConfigs.SCREENSHOT_FILE_PATH))
                {
                    Directory.CreateDirectory(AllConfigs.SCREENSHOT_FILE_PATH);
                }
                else
                {
                    Debug.Log($"{AllConfigs.DEBUG_TAG} {AllConfigs.SCREENSHOT_FILE_PATH} already exist!!!");
                }
            }
        }
    
        public void TakeScreenShot()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                StartCoroutine(TakeScreenShotAndSave());
            }
        }
    
        private IEnumerator TakeScreenShotAndSave()
        {
            // We should only read the screen buffer after rendering is complete
            yield return new WaitForEndOfFrame();
    
            // Create a texture the size of the screen, RGB24 format
            int width = Screen.width;
            int height = Screen.height;
            // 若要有透明背景, TextureFormat設為RGBA32
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    
            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            tex.Apply();
    
            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);
    
            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes($"{AllConfigs.SCREENSHOT_FILE_PATH}{AllConfigs.SCREENSHOT_FILE_NAME}{DateTime.Now.ToString("yyyyMMddHHmmss")}.png", bytes);
            // Toast to tell user image saved
            if (_bluetoothManager.GetBluetoothHelper != null)
            {
                _bluetoothManager.GetBluetoothHelper.ToastToTellImageSaved();
            }
        }
    }
}