using UnityEngine;

namespace EthanLin.Playback
{
    /// <summary>
    /// 計算取得一些詳情資料
    /// </summary>
    public class CalculatorDataDetail : MonoBehaviour
    {
        /// <summary>
        /// 取得向量角度
        /// </summary>
        /// <param name="aVectorA">A向量</param>
        /// <param name="aVectorB">B向量</param>
        /// <param name="aRefVector">參考向量</param>
        public static float GetAngle(Vector3 aVectorA, Vector3 aVectorB, Vector3 aRefVector)
        {
            if (Vector3.SignedAngle(aVectorA, aVectorB, aRefVector) < 0f)
            {
                return Vector3.Angle(aVectorA, aVectorB) + 180f;
            }
            else
            {
                return Vector3.Angle(aVectorA, aVectorB);
            }
        }
    
        /// <summary>
        /// 取得RPM值
        /// </summary>
        public static float GetRpm(float aSourceA, float aSourceB, float aTimeStampA, float aTimeStampB)
        {
            return (aSourceB - aSourceA) * 3600f / (aTimeStampB - aTimeStampA);
        }
    
        /// <summary>
        /// 取得平均值
        /// </summary>
        public static float GetAverage(float[] aSources)
        {
            float sum = 0;
            
            if (aSources.Length < 1)
            {
                return -99999f;
            }
            else
            {
                
                for (int i = 0; i < aSources.Length; ++i)
                {
                    sum = sum + aSources[i];
                }
    
                return sum / aSources.Length;
            }
        }
    }
}