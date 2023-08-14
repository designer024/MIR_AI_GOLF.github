using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin
{
    public class FPS : MonoBehaviour
    {
        private void Start() => Application.targetFrameRate = 30;
    }
}