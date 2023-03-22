using System.Collections;
using System.Collections.Generic;
using MagicLeap.XRKeyboard.Component;
using UnityEngine;
using MagicLeap.XRKeyboard.Extensions;
using Utilities;

namespace MagicLeap.XRKeyboard
{
    using System;
    using TMPro;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

 
    [Serializable]
    public class KeyboardByIdSerializedDictionary : SerializableDictionary<string, KeyboardLayout> { }

    public class Keyboard : MonoBehaviour
    {
        public enum Modifier
        {
            NEUTRAL, SHIFT, CAPS
        }


        
        public Action<Event> OnKeyUp;
  
        [HideInInspector] public Modifier ActivekeyboardModifier;
        
        [Header("Components")]
        [SerializeField] private PopupPanel _popupPanel;
        [SerializeField] private InputPreview _inputPreview;
        [SerializeField] private Button _hideButton;

   

        [Header("Panels")]
        public KeyboardByIdSerializedDictionary KeyboardPanelByLayoutId = new KeyboardByIdSerializedDictionary();

        private KeyboardLayout[] _panels;
        private TMP_InputField.ContentType _contentType = TMP_InputField.ContentType.Standard;
        private TMPInputFieldTextReceiver _inputFieldReceiver;
        private RectTransform _hideButtonRectTransform;


        void Awake()
        {
            _popupPanel = transform.GetCachedComponentInChildren(ref _popupPanel, true);
            _inputPreview = transform.GetCachedComponentInChildren(ref _inputPreview, true);
            _hideButtonRectTransform = _hideButton.GetComponentInChildren<RectTransform>(true);
        }

        void Start()
        {
            Setup();
            _popupPanel.Dismiss();
            UpdateContentType();

        }

        private void Setup()
        {   
            _panels = GetComponentsInChildren<KeyboardLayout>(true);

            KeyboardPanelByLayoutId.Clear();
            foreach (var keyboardPanel in _panels)
            {
                KeyboardPanelByLayoutId.Add(keyboardPanel.LayoutId.ToUpper(), keyboardPanel);
                var keys = keyboardPanel.GetKeys();
                foreach (var keyButton in keys)
                {
                    keyButton.OnKeyUp += OnKeyboardButtonUp;
                    keyButton.OnLongPress += ShowAccentOverlay;
                }
            }

            _popupPanel.OnKeysCreated.AddListener(OnPopupKeysCreated);
            _hideButton.onClick.AddListener(EndEdit);
       
        }

        private void OnPopupKeysCreated(KeyboardKey[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].OnKeyUp += OnKeyboardButtonUp;
                keys[i].OnLongPress += ShowAccentOverlay;
            }
        }
        public void HideAllKeyboardLayouts()
        {
            if(_panels== null)
            {
                return;
            }

            for (var i = 0; i < _panels.Length; i++)
            {
                _panels[i].HidePanel();
            }
        }

        private void OnDrawGizmosSelected()
        {
            var panels = GetComponentsInChildren<KeyboardLayout>(true);
            KeyboardPanelByLayoutId.Clear();
            
            
            foreach (var keyboardPanel in panels)
            {
                KeyboardPanelByLayoutId.Add(keyboardPanel.LayoutId.ToUpper(), keyboardPanel);
            }
        }



        public bool IsEditing()
        {
            return _inputFieldReceiver != null;
        }
        public void EndEdit()
        {
            if (_inputFieldReceiver != null)
            {
                _inputFieldReceiver.EndEdit();
                _inputFieldReceiver = null;
              
            }

            KeyboardManager.Instance.DespawnKeyboard();
        }

   
        
        

        private void OnEnable()
        {
            UpdateContentType();
        }

        public void SetKeyboard(TMPInputFieldTextReceiver inputField, TMP_InputField.ContentType contentType)
        {
            gameObject.SetActive(true);
            if (_inputFieldReceiver!= null && _inputFieldReceiver != inputField)
            {
                _inputFieldReceiver.EndEdit();
            }

            if (inputField != null)
            {
                _inputFieldReceiver = inputField;
                _inputPreview.SetTargetInputField(inputField.GetInputField());
            }

            _contentType = contentType;
            if (gameObject.activeSelf)
            {
                UpdateContentType();
            }
        }


        private void UpdateContentType()
        {
            bool isEmpty = _inputFieldReceiver == null || string.IsNullOrEmpty(_inputFieldReceiver.GetInputField().text);
            switch (_contentType)
            {
                case TMP_InputField.ContentType.Standard:
                    ChangeLayout("Letters");
                    SetModifier(isEmpty ? Modifier.SHIFT : Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.Autocorrected:
                    ChangeLayout("Letters");
                    break;
                case TMP_InputField.ContentType.IntegerNumber:
                    ChangeLayout("NumberPad");
                    break;
                case TMP_InputField.ContentType.DecimalNumber:
                    ChangeLayout("NumberPad");
                    break;
                case TMP_InputField.ContentType.Alphanumeric:
                    ChangeLayout("Letters");
                    SetModifier(isEmpty ? Modifier.SHIFT : Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.Name:
                    ChangeLayout("Letters");
                    SetModifier(isEmpty ? Modifier.SHIFT : Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.EmailAddress:
                    ChangeLayout("Letters");
                    SetModifier(Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.Password:
                    ChangeLayout("Letters");
                    SetModifier(Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.Pin:
                    ChangeLayout("NumberPad");
                    SetModifier(Modifier.NEUTRAL);
                    break;
                case TMP_InputField.ContentType.Custom:
                    ChangeLayout("Letters");
                    SetModifier(Modifier.NEUTRAL);
                    break;

            }
        }
     
        private void OnKeyboardButtonUp(string keyCode)
        {
          
            if (keyCode.ToUpper() == "SHIFT")
            {
                ChangeShift();
                return;
            }
            if (KeyboardPanelByLayoutId.Contains(keyCode.ToUpper()))
            {
                ChangeLayout(keyCode);
                return;
            }

       
            EncodeKeyEvent(keyCode);
        }

        private void EncodeKeyEvent(string keyCodeString)
        {
            UnityEngine.Event keyCodeEvent = new Event();
            keyCodeEvent.pointerType = PointerType.Touch;
           
         
            switch (ActivekeyboardModifier)
            {
                case Modifier.NEUTRAL:
                    keyCodeEvent.modifiers = EventModifiers.None;
                    break;
                case Modifier.SHIFT:
                    keyCodeEvent.modifiers = EventModifiers.Shift;
                    break;
                case Modifier.CAPS:
                    keyCodeEvent.modifiers = EventModifiers.CapsLock;
                    break;

            }
                
            if (keyCodeString == "\u0008")
            {
                keyCodeEvent.keyCode = KeyCode.Backspace;
            }

            if (keyCodeString == "\n")
            {
                keyCodeEvent.keyCode = KeyCode.Return;
            }
            bool upperCase = ActivekeyboardModifier == Modifier.SHIFT || ActivekeyboardModifier == Modifier.CAPS;
            keyCodeEvent.character = upperCase ? keyCodeString.ToUpper()[0] : keyCodeString.ToLower()[0];
            keyCodeEvent.type = EventType.KeyDown;

            OnKeyUp?.Invoke(keyCodeEvent);

            if (ActivekeyboardModifier == Modifier.SHIFT)
            {
                SetModifier(Modifier.NEUTRAL);
            }

            if (AccentPanelActive())
            {
                _popupPanel.Dismiss();
            }
        }


        public bool AccentPanelActive()
        {
            return _popupPanel.gameObject.activeInHierarchy;
        }

        public void SetModifier(Modifier modifier)
        {
            if(_panels == null || _panels.Length==0)
            {
                return;
            }

            foreach (var keyboardPanel in _panels)
            {
              
                foreach (var keyButton in keyboardPanel.GetKeys())
                {
                    switch (modifier)
                    {
                        case Modifier.NEUTRAL:
                            keyButton.UpdateActiveKey(modifier);
                            break;
                        case Modifier.SHIFT:
                        case Modifier.CAPS:
                            keyButton.UpdateActiveKey(modifier);
                            break;
                    }
                }
            }
            ActivekeyboardModifier = modifier;
        }

        private void ChangeLayout(string layoutId)
        {
            if (KeyboardPanelByLayoutId.TryGetValue(layoutId.ToUpper(), out var panel))
            {
                HideAllKeyboardLayouts();
                panel.ShowPanel();
                _inputPreview.StartResizeToRect(panel.GetRectTransform());
                
                //Shift Hide Button to edge of panel
                _hideButtonRectTransform.anchoredPosition = new Vector2(panel.GetRectTransform().offsetMax.x + (_hideButtonRectTransform.sizeDelta.x / 1.7f), -(panel.GetRectTransform().sizeDelta.y / 4.4f));
            }
            else
            {
                Debug.LogError($"Layout [{layoutId}] does not exist");
            }
        }

      
 



        public void ChangeShift()
        {

            if (ActivekeyboardModifier == Modifier.NEUTRAL)
            {
                SetModifier(Modifier.SHIFT);
            }
            else if (ActivekeyboardModifier == Modifier.SHIFT)
            {
                SetModifier(Modifier.CAPS);
            }
            else if (ActivekeyboardModifier == Modifier.CAPS)
            {
                SetModifier(Modifier.NEUTRAL);
            }
        }
 



        public void ShowAccentOverlay(List<string> specialChars, Transform keyTransform = null)
        {
            _popupPanel.ShowAccentPanel(specialChars, ActivekeyboardModifier, keyTransform);
        }

        public void ClearPreview()
        {
            if (_inputPreview != null) _inputPreview.ClearField();
        }
    }
}
