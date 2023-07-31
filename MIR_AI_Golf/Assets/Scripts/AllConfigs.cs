using System.IO;
using UnityEngine;

namespace EthanLin.Config
{
    public class AllConfigs
    {
        public const string DEBUG_TAG = "EthanLinBluetoothDataLib";

        /// <summary>
        /// for now Space Capsule Website
        /// </summary>
        public const string SPACE_CAPSULE_WEBSITE = "https://ethanspacecapsule.github.io/SpaceCapsule.github.io/";
    
        private const string ANDROID_ROOT_STORAGE_PATH = "/storage/emulated/0/";
    
        /// <summary>
        /// 儲存CSV之資料夾, old => $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}CSV_DATA{Path.DirectorySeparatorChar}"
        /// </summary>
        public static string CSV_ROOT_FOLDER = $"{ANDROID_ROOT_STORAGE_PATH}.SpaceCapsuleBluetoothCSV{Path.DirectorySeparatorChar}";
    
        /// <summary>
        /// 儲存Json之資料夾
        /// </summary>
        public static string JSON_ROOT_FOLDER = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}JSON_DATA{Path.DirectorySeparatorChar}";
    
        /// <summary>
        /// CSV總表檔名
        /// </summary>
        public const string TOTAL_DATA_CSV_FILE_NAME = "totalData.csv";
    
        /// <summary>
        /// Screen shot 儲存路徑
        /// </summary>
        public static string SCREENSHOT_FILE_PATH = $"{ANDROID_ROOT_STORAGE_PATH}Pictures/SpaceCapsule_Bluetooth/";

        /// <summary>
        /// Screen shot 儲存名稱前綴
        /// </summary>
        public const string SCREENSHOT_FILE_NAME = "SpaceCapsule-";
    }
}