using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin.JsonHelper
{
    public class JsonInfo
    {
        /// <summary>
        /// friendly name
        /// </summary>
        public string where;
        /// <summary>
        /// GPS
        /// </summary>
        public string location;
        public int version;
        public BodyPart[] BodyParts;
    
    
        public JsonInfo() { }
    
        public JsonInfo(string aWhere, string aLocation, int aVersion, BodyPart[] aBodyParts)
        {
            where = aWhere;
            location = aLocation;
            version = aVersion;
            BodyParts = aBodyParts;
        }
    }
    
    public class BodyPart
    {
        public string bodyPartName;
        public SensorVector[] sensorVectors;
        
        public BodyPart() {}
    
        public BodyPart(string aBodyPartName, SensorVector[] aSensorVectors)
        {
            bodyPartName = aBodyPartName;
            sensorVectors = aSensorVectors;
        }
    }
    
    public class SensorVector
    {
        public double dirX;
        public double dirZ;
    
        public SensorVector()
        {
        }
    
        public SensorVector(double aDirX, double aDirZ)
        {
            dirX = aDirX;
            dirZ = aDirZ;
        }
    }
}