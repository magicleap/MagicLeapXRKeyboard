using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using MagicLeap.XRKeyboard.Extensions;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MagicLeap.XRKeyboard.Component
{


    public class TMPInputFieldTextReceiver : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private TMP_InputField _textMeshInputField;
        private Keyboard _keyboard;
        private bool _editing;


        private void Start()
        {
            _textMeshInputField = transform.GetCachedComponentInChildren(ref _textMeshInputField);
            _textMeshInputField.onSubmit.AddListener(OnSubmit);

            _textMeshInputField.shouldHideMobileInput = true;

        }

        private void Reset()
        {
            _textMeshInputField = transform.GetCachedComponentInChildren(ref _textMeshInputField);
        }

        public TMP_InputField GetInputField()
        {
            return _textMeshInputField;
        }

        private void OnSubmit(string submitString)
        {
            DisableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        public void OnSelect(BaseEventData eventData)
        {
            EnableInput();
        }
        
        public void EnableInput()
        {
            if (_keyboard == null && _editing==false)
            {
                _keyboard = KeyboardManager.Instance.ShowKeyboard(this, _textMeshInputField.contentType);

                _keyboard.OnKeyUp += HandleKeyPress;
              
                _editing = true;
            }
        }

        public void EndEdit()
        {
            if (_keyboard != null && _editing)
            {
              
                _keyboard.OnKeyUp -= HandleKeyPress;
                _keyboard.ClearPreview();
                _keyboard = null;
            }

            _editing = false;
        }

        public void DisableInput()
        {
            EndEdit();
            KeyboardManager.Instance.DespawnKeyboard();

        }

      

        private void HandleKeyPress(Event keyEvent)
        {
           
            _textMeshInputField.ProcessEvent(keyEvent);
            _textMeshInputField.ForceLabelUpdate();
         

            if (keyEvent.keyCode == KeyCode.Return && _textMeshInputField.lineType != TMP_InputField.LineType.MultiLineNewline)
            {
                _textMeshInputField.onSubmit?.Invoke(_textMeshInputField.text);
                _textMeshInputField.DeactivateInputField();
            }

        }


        public void Clear()
        {
            
            if (_textMeshInputField != null) { _textMeshInputField.text = string.Empty; }
            if (_keyboard != null) _keyboard.ClearPreview();
        }
    }
}
