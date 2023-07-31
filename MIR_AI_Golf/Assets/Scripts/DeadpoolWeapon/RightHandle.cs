using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthanLin
{
    public class RightHandle : MonoBehaviour
    {
        [SerializeField] private GameObject _gunOFF, _gunON, _gunLight;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ParticleSystem _particleSystem;
        
        /// <summary>
        /// 0:空手, 1:槍, 2:劍
        /// </summary>
        private int _state = 0;
        
        private void Start()
        {
            _gunOFF.SetActive(true);
            _gunON.SetActive(false);
            
            _state = 0;
        }
    
        private void OnTriggerEnter(Collider aOther)
        {
            switch (aOther.tag)
            {
                case "HolsterR":
                    if (_state == 0) //拔槍
                    {
                        _gunOFF.SetActive(false);
                        _gunON.SetActive(true);
                        _state = 1;
                    }
                    else if (_state == 1) //收槍
                    {
                        _gunOFF.SetActive(true);
                        _gunON.SetActive(false);
                        _state = 0;
                    }
                    break;
                
                case "FireTrigger":
                    if (_state == 1)
                    {
                        _particleSystem.Play();
                        _audioSource.Play();
                        _gunLight.SetActive(true);
                    }
                    break;
            }
        }
    
        private void OnTriggerExit(Collider aOther)
        {
            if (aOther.tag == "FireTrigger") //停止射擊
            {
                _particleSystem.Stop();
                _audioSource.Stop();
                _gunLight.SetActive(false);
            }
        }
    }
}


