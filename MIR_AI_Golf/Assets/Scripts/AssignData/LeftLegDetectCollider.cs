using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftLegDetectCollider : MonoBehaviour
{

    public bool IsLeftLegLegal { private set; get; }

    private void Start()
    {
        IsLeftLegLegal = true;
    }

    private void OnTriggerEnter(Collider aOther)
    {
        if (aOther.tag.Equals("LeftLegWall"))
        {
            IsLeftLegLegal = false;
        }
        // Debug.Log($"{aOther.transform.tag}");
    }
    
    private void OnTriggerExit(Collider aOther)
    {
        if (aOther.tag.Equals("LeftLegWall"))
        {
            IsLeftLegLegal = true;
        }
        // Debug.Log($"");
    }
}
