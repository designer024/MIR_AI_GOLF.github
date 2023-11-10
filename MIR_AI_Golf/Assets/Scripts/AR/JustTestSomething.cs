using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JustTestSomething : MonoBehaviour
{
    [SerializeField] private GameObject _cameraOffset;

    [SerializeField] private Text _text;

    private void Update()
    {
        GameObject mixed = GameObject.FindWithTag("RoleMixed");
        if (mixed != null)
        {
            _text.text = $"Camera Offset: {_cameraOffset.transform.position}{Environment.NewLine}Role: {mixed.transform.position}";
        }
    }

    public void TestToRotate(float aValue) => _cameraOffset.transform.eulerAngles = new Vector3(0f, aValue, 0f);

    public void BacKToNormalScene() => SceneManager.LoadScene(1);
    
}
