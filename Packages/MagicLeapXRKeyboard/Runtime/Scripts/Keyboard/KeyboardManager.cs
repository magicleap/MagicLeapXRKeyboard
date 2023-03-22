using System.Collections.Generic;
using MagicLeap.XRKeyboard.Component;
using MagicLeap.XRKeyboard.Extensions;
using TMPro;
using UnityEngine;

namespace MagicLeap.XRKeyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        public static KeyboardManager Instance { get; private set; }

        [SerializeField] private bool _showKeyboardOnStart = false;
        [SerializeField] private PlaceInFront _placeInFront;
        [SerializeField] private Keyboard _keyboard;
        private bool _keyboardActive = false;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
           
        }

        private void Start() {
            _keyboardActive = _keyboard.gameObject.activeInHierarchy;
            _placeInFront = transform.GetCachedComponentInChildren(ref _placeInFront, true);
            if (_showKeyboardOnStart)
            {
                ShowKeyboard(null, TMP_InputField.ContentType.Alphanumeric);
            }
            else
            {
                DespawnKeyboard();
            }
        }



        public virtual Keyboard ShowKeyboard(TMPInputFieldTextReceiver inputFieldReceiver, TMP_InputField.ContentType contentType)
        {
            if (_placeInFront)
            {
                _placeInFront.SnapToTarget();
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
