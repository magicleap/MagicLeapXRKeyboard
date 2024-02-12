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
        [SerializeField] private FollowTarget _followTarget;
        [SerializeField] private Keyboard _keyboard;
        private bool _keyboardActive = false;
        private bool _lookAtUser;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
           
        }

        private void Start()
        {
          
            _keyboardActive = _keyboard.gameObject.activeInHierarchy;
            _followTarget = transform.GetCachedComponentInChildren(ref _followTarget, true);
            if (_showKeyboardOnStart)
            {
                ShowKeyboard(null, TMP_InputField.ContentType.Alphanumeric);
            }
            else
            {
                DespawnKeyboard();
            }
        }

        public void LookAtUser(bool on)
        {
            _lookAtUser = on;
         
        }

        public void ToggleFollow(bool on)
        {
            if (_followTarget)
            {
                _followTarget.enabled = on;
            }
        }

        private void Update()
        {
            if (_lookAtUser)
            {
                // Look at the Camera but remain upright

                Vector3 directionToCamera = Camera.main.transform.position - transform.position;
                Vector3 correctedDirection = new Vector3(directionToCamera.x, directionToCamera.y, -directionToCamera.z);
                Quaternion rotation = Quaternion.LookRotation(correctedDirection, Vector3.up);
                transform.rotation = rotation;
            }
        }


        public virtual Keyboard ShowKeyboard(TMPInputFieldTextReceiver inputFieldReceiver, TMP_InputField.ContentType contentType)
        {
            if (_followTarget)
            {
                _lookAtUser = false;
                _followTarget.enabled = true;
                _followTarget.Recenter();
            }

            _keyboard.SetKeyboard(inputFieldReceiver, contentType);
            _keyboardActive = true;
            _keyboard.gameObject.SetActive(_keyboardActive);
            return _keyboard;
        }

        public void DespawnKeyboard()
        {
            if (_keyboardActive)
            {
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
