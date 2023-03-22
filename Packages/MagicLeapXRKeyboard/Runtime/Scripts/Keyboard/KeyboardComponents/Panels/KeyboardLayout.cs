using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap.XRKeyboard.Component;
using UnityEngine;
using MagicLeap.XRKeyboard.Extensions;

namespace MagicLeap.XRKeyboard
{

    [RequireComponent(typeof(KeyboardBuilder))]
    public class KeyboardLayout : MonoBehaviour
    {
   
        public KeyboardBuilder keyboardBuilder;
        public string LayoutId;
        [Tooltip("When enabled, the keyboard will regenerate from the key map on start")]
        public bool regenKeyboardOnStart;

        private RectTransform _rectTransform;
   
        
        // Start is called before the first frame update
        void Start()
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            keyboardBuilder = GetComponent<KeyboardBuilder>();

            if (regenKeyboardOnStart) keyboardBuilder.RegenerateKeyboard();
        }

        private void Reset()
        {
            keyboardBuilder = GetComponent<KeyboardBuilder>();
            
        }

        private void OnDrawGizmosSelected()
        {
            keyboardBuilder = gameObject.GetCachedComponent(ref keyboardBuilder);
            if (keyboardBuilder)
            {
                LayoutId = keyboardBuilder.LayoutId;
            }
        }

        public IReadOnlyCollection<KeyboardKey> GetKeys()
        {
            return GetComponentsInChildren<KeyboardKey>(true);
         
        }

        public IReadOnlyCollection<KeyboardRow> GetRows()
        {
            return keyboardBuilder.KeyRows;
        }
        

        public RectTransform GetRectTransform()
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            return _rectTransform;
        }

        public void ShowPanel()
        {
            EnableInput();
            gameObject.SetActive(true);
            StopAllCoroutines();
        }

        public void HidePanel()
        {
            DisableInput();
            gameObject.SetActive(false);
        }

      

        public void DisableInput()
        {


            UnityEngine.UI.Button[] interactionButtons = GetComponentsInChildren<UnityEngine.UI.Button>();

            foreach (UnityEngine.UI.Button interactionButton in interactionButtons)
            {
                interactionButton.interactable = false;
            }
        }

        public void EnableInput()
        {
            var interactionButtons = GetComponentsInChildren<KeyboardKey>();

            foreach (var interactionButton in interactionButtons)
            {
                if (interactionButton.GetComponent<KeyboardKey>().KeyCode != "")
                {
                    interactionButton.SetInteractable(true);
                }
            }
        }
    }
}
