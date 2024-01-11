using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLegDetectCollider : MonoBehaviour
{
    public bool IsRightLegLegal { private set; get; }

    private void Start()
    {
        IsRightLegLegal = true;
    }

    private void OnTriggerEnter(Collider aOther)
    {
        if (aOther.tag.Equals("RightLegWall"))
        {
            IsRightLegLegal = false;
        }
        // Debug.Log($"{aOther.transform.tag}");
    }
    
    private void OnTriggerExit(Collider aOther)
    {
        if (aOther.tag.Equals("RightLegWall"))
        {
            IsRightLegLegal = true;
        }
        // Debug.Log($"");
    }
}
