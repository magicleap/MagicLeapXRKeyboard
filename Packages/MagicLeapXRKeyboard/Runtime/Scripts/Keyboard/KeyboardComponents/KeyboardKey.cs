using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MagicLeap.XRKeyboard.Extensions;
using MagicLeap.XRKeyboard.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MagicLeap.XRKeyboard.Component
{



    public class KeyboardKey : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IDragHandler,  IPointerUpHandler
    {

        public enum KeyGroup
        {
            Normal,
            Accent
        }


        
        public Action<string> OnKeyUp;
        public Action<List<string>, Transform> OnLongPress;
        public string KeyCode ="[SET BY KEYBOARD]";
        public string Label="[SET BY KEYBOARD]";
        
        [Header("Components")]
        [SerializeField] private TMP_Text _lable;
        [SerializeField] private TMP_Text _accentLabel;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private LayoutElement _layoutElement;
        
        private KeyGroup _group;
        [SerializeField] private List<string> _accents = new List<string>();
        private Coroutine _longPressDetectorCoroutine;
        private Coroutine _longPressCoroutine;
        [SerializeField] private bool _showAccentHint;
        private bool _longPressed = false;
        private bool _isPressed;
        private float _widthScale = 1;
        private static readonly float _longPressedThreshold = .25f;

        public void Awake()
        {

            if (string.IsNullOrWhiteSpace(Label))
            {
                Label = KeyCode;
            }

        }

        private void Reset()
        {

            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            _layoutElement = gameObject.GetCachedComponent(ref _layoutElement);
            _button = gameObject.GetCachedComponentInChildren(ref _button,true);
        }


        public void Initialize(Vector2 sizeDelta, Vector2 anchoredPosition,string label, string key, KeyGroup group = KeyGroup.Normal, Keyboard.Modifier mode = Keyboard.Modifier.NEUTRAL, Vector4? margins=null, string extension = "")
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            _layoutElement = gameObject.GetCachedComponent(ref _layoutElement);
       
            _rectTransform.sizeDelta = sizeDelta;
            _rectTransform.anchoredPosition = anchoredPosition;
            _layoutElement.preferredWidth = sizeDelta.x;
            _layoutElement.preferredHeight = sizeDelta.y;
            KeyCode = key;

            Label = label;
            _showAccentHint = false;

            _iconImage.enabled = false;
            margins ??= new Vector4(50, 50, 50, 98.2f);

            _iconImage.rectTransform.offsetMin = new Vector2(margins.Value.x, margins.Value.w);
            _iconImage.rectTransform.offsetMax = new Vector2(margins.Value.z, margins.Value.y);
            _lable.margin = margins.Value;
          
        
            gameObject.name = label + extension;
            _group = group;

 
            UpdateActiveKey(mode);
        }

        public void Initialize(KeyboardLayoutData.Key key, Vector2 sizeDelta, Vector2 anchoredPosition, Vector2 preferredSize, bool showAccentHint, KeyGroup group = KeyGroup.Normal, Keyboard.Modifier mode = Keyboard.Modifier.NEUTRAL)
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            _layoutElement = gameObject.GetCachedComponent(ref _layoutElement);
            _rectTransform.sizeDelta = sizeDelta;
            _rectTransform.anchoredPosition = anchoredPosition;
            _layoutElement.preferredWidth = preferredSize.x;
            _layoutElement.preferredHeight = preferredSize.y;
            var buttonColors = _button.colors;
            buttonColors.normalColor= key.BackgroundColor;
            _button.colors = buttonColors;
            KeyCode = key.KeyCode;
            
            Label = key.KeyLabel;

            if (!string.IsNullOrWhiteSpace(key.KeyIconPath))
            {
                _showAccentHint = false;
                _iconImage.enabled = true;
                _iconImage.sprite= Resources.Load(key.KeyIconPath) as Sprite;
            }
            else
            {
                _showAccentHint = showAccentHint;
            
                _iconImage.enabled = false;
            }

       

            _iconImage.rectTransform.offsetMin = new Vector2(key.Margins.x, key.Margins.w);
            _iconImage.rectTransform.offsetMax = new Vector2(key.Margins.z, key.Margins.y);
            _lable.margin = key.Margins;
            _accents = key.Accents;
            _widthScale = key.WidthScale;
            gameObject.name = key.KeyCode;
            _group = group;
          
           
            UpdateActiveKey(mode);
        }
        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void UpdateActiveKey(Keyboard.Modifier modifier)
        {
          
            if (string.IsNullOrWhiteSpace(Label))
            {
                Label = KeyCode;
            }

            if (modifier == Keyboard.Modifier.SHIFT || modifier == Keyboard.Modifier.CAPS)
            {
                KeyCode = KeyCode.ToUpper();
                if(Label.Length==1)
                {
                    Label = Label.ToUpper();
                }
            }
            else
            {
                KeyCode = KeyCode.ToLower();
                if (Label.Length == 1)
                {
                    Label = Label.ToLower();
                }
            }

       
            string displayStringKey = KeyCode;
            if (displayStringKey.ToUpper() == "SHIFT")
            {
                if (modifier == Keyboard.Modifier.NEUTRAL)
                {
                    displayStringKey += "_NEUTRAL";
                }
                else if (modifier == Keyboard.Modifier.SHIFT)
                {
                    displayStringKey += "_SHIFT";
                }
                else if (modifier == Keyboard.Modifier.CAPS)
                {
                    displayStringKey += "_CAPS";
                }
          
            }

            if (KeyboardCollections.NonStandardKeyToDisplayString.TryGetValue(displayStringKey.ToUpper(), out var nonStandardKeyCodeText))
            {
                Label = nonStandardKeyCodeText;
          
            }

            if (KeyboardCollections.LabelsToCode.TryGetValue(KeyCode.ToUpper(), out var keyValue))
            {
                KeyCode = keyValue;

            }

            if (_accentLabel != null)
            {
                if (_showAccentHint && _accents.Count > 0)
                {
                    _accentLabel.SetText(_accents[0]);
                }
                else
                {
                    _accentLabel.text = "";
                }

            }
            _lable.text = Label;

            if (_button != null)
            {
                _button.interactable = Label.Length > 0;
            }

          
          

        }

 

        public void TextPress()
        {
            if (!_longPressed)
            {
                KeyPressEvent();
            }
            else
            {
                _longPressed = false;
            }
        }

        private void LongPressStart()
        {
            _longPressed = false;
            if (_longPressDetectorCoroutine != null)
            {
                StopCoroutine(_longPressDetectorCoroutine);
            }
            _longPressDetectorCoroutine = StartCoroutine(LongpressDetection());
        }

        private IEnumerator LongpressDetection()
        {
            float longpressThreshold = Time.time + _longPressedThreshold;
            while (_isPressed && !_longPressed)
            {
                if (Time.time > longpressThreshold)
                {
                    InvokeLongPress();
                }
                yield return null;
            }
        }

        private void InvokeLongPress()
        {
            switch (KeyCode.ToUpper())
            {
                case "\u0008": //BACKSPACE
                    if (_longPressCoroutine != null)
                    {
                        StopCoroutine(_longPressCoroutine);
                    }
                    _longPressCoroutine = StartCoroutine(BackspaceLongPress());
                    break;
                default:
                    if (_accents.Count>0)
                    {
                        OnLongPress?.Invoke(_accents, transform);
                    }
                    break;
            }

            _longPressed = true;
        }

        private IEnumerator BackspaceLongPress()
        {
            float gracePeriodThreshold = Time.time + _longPressedThreshold;
            while (Time.time < gracePeriodThreshold)
            {
                if (!_isPressed)
                {
                    break;
                }
                yield return null;
            }


            float timeStep = 0.1f;
            float nextPress = 0;
            while (_isPressed)
            {
                if (Time.time > nextPress)
                {
                    nextPress = Time.time + timeStep;
                    KeyPressEvent();
                }
                yield return null;
            }
        }


        private void KeyPressEvent()
        {
            OnKeyUp?.Invoke(KeyCode);
        }

        public float GetKeyScale()
        {
            return _widthScale;
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
           
            _isPressed = false;
            TextPress();
        }

        /// <inheritdoc />
        public void OnPointerDown(PointerEventData eventData) 
        {

            _isPressed = true;
            LongPressStart();
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        { 
         
            if (_group != KeyGroup.Accent)
            {
                return;
            }

       
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }

        /// <inheritdoc />
        public void OnPointerExit(PointerEventData eventData)
        {
            if(_group!= KeyGroup.Accent)
            {
                return;
            }
   
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }

        /// <inheritdoc />
        public void OnDrop(PointerEventData eventData)
        {
 
            if (_group != KeyGroup.Accent)
            {
                return;
            }

        
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }

        /// <inheritdoc />
        public void OnDrag(PointerEventData eventData)
        {
            //Have to have this call in order to use OnDrop()
        }

        /// <inheritdoc />
        public void OnPointerUp(PointerEventData eventData)
        {
            if(_group == KeyGroup.Normal)
            _isPressed = false;
        }
    }
}
