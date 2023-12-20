using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap.XRKeyboard.Component;
using UnityEngine;
using MagicLeap.XRKeyboard.Extensions;
using UnityEngine.Serialization;

namespace MagicLeap.XRKeyboard
{

    /// <summary>
    /// Groups the <see cref="KeyboardKey"/>s and <see cref="KeyboardRow"/>s and communicates to the <see cref="KeyboardBuilder"/> to generate a new layout if needed. 
    /// </summary>
    [RequireComponent(typeof(KeyboardBuilder))]
    public class KeyboardLayout : MonoBehaviour
    {
   
        [FormerlySerializedAs("keyboardBuilder")]
        public KeyboardBuilder KeyboardBuilder;
        public string LayoutId;
        [FormerlySerializedAs("regenKeyboardOnStart")]
        [Tooltip("When enabled, the keyboard will regenerate from the key map on start")]
        public bool RegenKeyboardOnStart;

        private RectTransform _rectTransform;
   
        
        // Start is called before the first frame update
        void Start()
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            KeyboardBuilder = GetComponent<KeyboardBuilder>();

            if (RegenKeyboardOnStart) KeyboardBuilder.RegenerateKeyboard();
        }

        private void Reset()
        {
            KeyboardBuilder = GetComponent<KeyboardBuilder>();
            
        }

        public Transform KeyboardContainer()
        {
            return KeyboardBuilder.KeyboardContainer;
        }
        private void OnDrawGizmosSelected()
        {
            KeyboardBuilder = gameObject.GetCachedComponent(ref KeyboardBuilder);
            if (KeyboardBuilder)
            {
                LayoutId = KeyboardBuilder.LayoutId;
            }
        }

        public IReadOnlyCollection<KeyboardKey> GetKeys()
        {
            return GetComponentsInChildren<KeyboardKey>(true);
         
        }

        public IReadOnlyCollection<KeyboardRow> GetRows()
        {
            return KeyboardBuilder.KeyRows;
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
