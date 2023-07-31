using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EthanLin.Playback
{
    public class ReadCsvDataButtonFunction : MonoBehaviour
    {
        private IReadCsvButton _iReadCsvButton;

        private SoundManager _soundManager;

        [SerializeField] private Text _dateTimeTxt;
    
        private void Start()
        {
            _iReadCsvButton = GameObject.FindWithTag("PlaybackHelper").GetComponent<PlaybackHelper>();
            _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        }
    
        public void ReadCustomCsvData()
        {
            if (_soundManager != null)
            {
                _soundManager.PlayTheSound();
            }
        
            if (_iReadCsvButton != null)
            {
                _iReadCsvButton.OnReadCsvButtonPress(_dateTimeTxt.text);
            }
        }

        public void UploadCsvToCloud()
        {
            if (_soundManager != null)
            {
                _soundManager.PlayTheSound();
            }
        }
    }
}


