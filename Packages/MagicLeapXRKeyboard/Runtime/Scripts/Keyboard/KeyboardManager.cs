using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap.XRKeyboard.Component;
using MagicLeap.XRKeyboard.Extensions;
using TMPro;
using UnityEngine;
using MagicLeap.XRKeyboard.Utilities;

namespace MagicLeap.XRKeyboard
{
    /// <summary>
    /// Allows users to toggle the <see cref="Keyboard"/> on and off.
    /// </summary>
    public class KeyboardManager : MonoBehaviour
    {
        public static KeyboardManager Instance { get; private set; }

        [SerializeField] private bool _showKeyboardOnStart = false;
        [SerializeField] private FollowUser _followUser;
        [SerializeField] private Keyboard _keyboard;
        public TMP_InputField CurrentInputField => _keyboard.CurrentInputField;
        private bool _keyboardActive = false;
        private Collider[] _colliders;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
           
        }


        private void DelayCall(float delay, Action action)
        {
            StartCoroutine(DelayCallCoroutine(delay, action));
        }

        private IEnumerator DelayCallCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        private void Start()
        {
            _colliders = GetComponentsInChildren<Collider>(true);
            
            _keyboardActive = _keyboard.gameObject.activeInHierarchy;
            _followUser = transform.GetCachedComponentInChildren(ref _followUser, true);
            if (_showKeyboardOnStart)
            {
                ShowKeyboard(null, TMP_InputField.ContentType.Alphanumeric, TouchScreenKeyboardType.Default);
            }
            else
            {
                DespawnKeyboard();
            }
        }


        public void ToggleFollow(bool on)
        {
            if (_followUser)
            {
                _followUser.enabled = on;
            }
        }


        public virtual Keyboard ShowKeyboard(TMPInputFieldTextReceiver inputFieldReceiver, TMP_InputField.ContentType contentType, TouchScreenKeyboardType keyboardType)
        {
        
            if (_followUser)
            {
     
                _followUser.enabled = true;
                _followUser.Recenter();
            }
            DelayCall(.16f, () =>
                            {
                                for (var i = 0; i < _colliders.Length; i++)
                                {
                                    if (_colliders[i] != null)
                                    {
                                        _colliders[i].enabled = true;
                                    }
                                }
                            });
    
            _keyboard.SetKeyboard(inputFieldReceiver, contentType, keyboardType);
            _keyboardActive = true;
            _keyboard.gameObject.SetActive(_keyboardActive);
            return _keyboard;
        }

        public void DespawnKeyboard()
        {
            if (_keyboardActive)
            {
                for (var i = 0; i < _colliders.Length; i++)
                {
                    if(_colliders[i] !=null)
                    {
                        _colliders[i].enabled = false;
                    }
                }

                _keyboardActive = false;
                if(_keyboard && _keyboard.gameObject)
                {
                    _keyboard.gameObject.SetActive(_keyboardActive);
                }
            }
        }

        public Keyboard GetKeyboard()
        {
            return _keyboard;
        }
    }
}
