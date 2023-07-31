using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.AndroidBluetoothLib;

public class RecordMotionHelper : MonoBehaviour
{
    [SerializeField] private BluetoothManager _bluetoothManager;
    
    [SerializeField] private Animator _recordButtonAnimator;
    /// <summary>
    /// 是否記錄動作
    /// </summary>
    private bool _isRecordMotion;

    private void Start()
    {
        _isRecordMotion = false;
    }

    /**
     * 開始或停止錄製
     */
    public void StartStopRecord()
    {
        _isRecordMotion = !_isRecordMotion;
        _recordButtonAnimator.Play(_isRecordMotion ? "StartRecord" : "StopRecord");
        _bluetoothManager.GetBluetoothHelper.StartStopRecord();
    }
}
