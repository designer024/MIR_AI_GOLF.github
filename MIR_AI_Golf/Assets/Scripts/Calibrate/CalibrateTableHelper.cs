using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.CalibrateHelper
{
    /// <summary>
    /// 建立Table
    /// </summary>
    public class CalibrateTableHelper
    {
        /// <summary>
        /// 部位名稱
        /// </summary>
        public readonly int PartIndex;
        /// <summary>
        /// Sensor之北東南西四個向量
        /// </summary>
        public readonly Vector3[] SensorDirections;
        /// <summary>
        /// 四個象限間的夾角
        /// </summary>
        public readonly float[] QuadrantAngles;
        /// <summary>
        /// 共需多少個參考方位校正, 目前4個
        /// </summary>
        private readonly int _departureNumber = 0;
        
        // public float ratio { private set; get; }
        private float _ratio = 0f;
        // public Vector3 fixedDirection_Projected { private set; get; }
        private Vector3 _fixedDirectionProjected = Vector3.zero;
        
        /// <summary>
        /// 建立Table 建構子
        /// </summary>
        /// <param name="aPartIndex">部位ID</param>
        /// <param name="aDepartureNumber">目前以北東南西4個分切數量</param>
        public CalibrateTableHelper(int aPartIndex, int aDepartureNumber)
        {
            PartIndex = aPartIndex;
            _departureNumber = aDepartureNumber;
            SensorDirections = new Vector3[_departureNumber];
            QuadrantAngles = new float[_departureNumber];
        }

        /// <summary>
        /// 目前依序設定北、東、南、西4毎方位
        /// </summary>
        /// <param name="aCurrentCalibrateIndex">目前取得方位的次數</param>
        /// <param name="aDirection">目前取得方位的向量</param>
        public void SetDirections(int aCurrentCalibrateIndex, Vector3 aDirection)
        {
            SensorDirections[aCurrentCalibrateIndex] = new Vector3(aDirection.x, 0f, aDirection.z);
        }

        /// <summary>
        /// 取得每象限間之夾角
        /// </summary>
        public void GetQuadrantAngles()
        {
            for (int i = 0; i < _departureNumber; ++i)
            {
                if (i == 3)
                {
                    // 西-北
                    QuadrantAngles[i] = Vector3.Angle(SensorDirections[i], SensorDirections[0]);
                }
                else
                {
                    QuadrantAngles[i] = Vector3.Angle(SensorDirections[i], SensorDirections[i + 1]);
                }
            }
        }
        
        /// <summary>
        /// 依當前象限建立table 的 法向量
        /// </summary>
        /// <param name="aCurrentVectorProjected">當前RawData之投影向量</param>
        /// <param name="aCurrentQuadrant">當前RawData之向量所指的象限</param>
        public Vector3 GetTableNormalVector(Vector3 aCurrentVectorProjected, int aCurrentQuadrant)
        {
            Vector3 normalVector = Vector3.zero;
            
            switch (aCurrentQuadrant)
            {
                // 北-東
                case 0:
                    _ratio = Vector3.Angle(aCurrentVectorProjected, SensorDirections[aCurrentQuadrant]) / QuadrantAngles[aCurrentQuadrant];
                    _fixedDirectionProjected = Vector3.Lerp(Vector3.forward, Vector3.right, _ratio);
                    break;
                
                // 東-南
                case 1:
                    _ratio = Vector3.Angle(aCurrentVectorProjected, SensorDirections[aCurrentQuadrant]) / QuadrantAngles[aCurrentQuadrant];
                    _fixedDirectionProjected = Vector3.Lerp(Vector3.right, Vector3.back, _ratio);
                    break;
                
                // 南-西
                case 2:
                    _ratio = Vector3.Angle(aCurrentVectorProjected, SensorDirections[aCurrentQuadrant]) / QuadrantAngles[aCurrentQuadrant];
                    _fixedDirectionProjected = Vector3.Lerp(Vector3.back, Vector3.left, _ratio);
                    break;
                
                // 西-北
                case 3:
                    _ratio = Vector3.Angle(aCurrentVectorProjected, SensorDirections[aCurrentQuadrant]) / QuadrantAngles[aCurrentQuadrant];
                    _fixedDirectionProjected = Vector3.Lerp(Vector3.left, Vector3.forward, _ratio);
                    break;
            }
            
            normalVector = Vector3.Cross(_fixedDirectionProjected, Vector3.up);

            return normalVector;
        }
    }
}


