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
        public static string CSV_ROOT_FOLDER = $"{ANDROID_ROOT_STORAGE_PATH}.SpaceCapsuleBtDemo{Path.DirectorySeparatorChar}";
    
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
        public static string SCREENSHOT_FILE_PATH = $"{ANDROID_ROOT_STORAGE_PATH}Pictures{Path.DirectorySeparatorChar}SpaceCapsule{Path.DirectorySeparatorChar}";

        public static int CURRENT_SELECTED_ROLE_INDEX;

        /// <summary>
        /// Screen shot 儲存名稱前綴
        /// </summary>
        public const string SCREENSHOT_FILE_NAME = "SpaceCapsule-";
        
        public const string QUIT_APP_MESSAGE = "ARE YOU SURE TO QUIT THIS GREAT APP";
        public const string RETURN_TO_MAIN_SCENE = "ARE YOU SURE TO RETURN TO MAIN SCENE";
        
        /// <summary>
        /// for PlayerPrefs 只使用上半身的 key
        /// </summary>
        public const string PLAYERPREFS_ONLY_UPPER_KEY = "onlyUpper";
        /// <summary>
        /// for PlayerPrefs 是否使用打折 key
        /// </summary>
        public const string PLAYERPREFS_USE_VARIATION_KEY = "useVariation";
        /// <summary>
        /// for PlayerPrefs 是否使用背景音樂 key
        /// </summary>
        public const string PLAYERPREFS_BGM_KEY = "BGM";
        /// <summary>
        /// for PlayerPrefs 是否使用按鍵音效 key
        /// </summary>
        public const string PLAYERPREFS_BUTTON_SOUND_KEY = "buttonSound";
        /// <summary>
        /// for PlayerPrefs role can rotate at Demo scene key
        /// </summary>
        public const string PLAYERPREFS_DEMO_CAN_ROTATE_KEY = "demoCanRotate";
    }
}