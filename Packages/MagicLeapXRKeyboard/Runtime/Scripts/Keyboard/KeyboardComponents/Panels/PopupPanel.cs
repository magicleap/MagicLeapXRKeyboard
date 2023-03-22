using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MagicLeap.XRKeyboard.Component;
using UnityEngine.Events;
using MagicLeap.XRKeyboard.Extensions;

namespace MagicLeap.XRKeyboard
{
    public class PopupPanel : MonoBehaviour
    {
        public UnityEvent<KeyboardKey[]> OnKeysCreated;
        
        [Header("Prefabs")]
        [SerializeField] private KeyboardKey keyPrefab;
       
        [Header("Components")]
        [SerializeField] private LayoutGroup _container;
        [SerializeField] private Button _dissmissButton;
        
        [Header("Settings")]
        [SerializeField]
        private Vector2 _keySize = new Vector2(300, 300);
        [SerializeField]
        private Vector3 anchorOffset = new Vector3(0, 250, 250);
        [SerializeField]
        private Vector3 horizontalOffset = new Vector3(200, 0, 0);
        [SerializeField]
        private float timeout = 10;
        
      

        private Transform _anchor;
        private RectTransform _containerRectTransform;
        private List<string> _keyCodes;
        private KeyboardKey[] _keys;
        private Coroutine _timeoutCoroutine;

        [EasyButtons.Button]
        public void TogglePanelVisibility()
        {
            if (gameObject.activeSelf == false)
            {
                ClearPanel();
                GeneratePanel(Keyboard.Modifier.NEUTRAL);
            }

          
            gameObject.SetActive(!gameObject.activeSelf);

        }

        [EasyButtons.Button]
        public void TestShowAccentPanel()
        {
            var accents = new List<string>(){ "@", "æ", "ã", "å", "ā", "à", "á", "â", "ä"};
            if (_anchor == null)
            {
                _anchor = FindObjectsOfType<KeyboardKey>().FirstOrDefault((key) => key.KeyCode.ToUpper() == "A").transform;
            }
            ShowAccentPanel(accents, Keyboard.Modifier.NEUTRAL);
 
        }


        private void Start()
        {
            _dissmissButton.onClick.AddListener(Dismiss);
        }

        public void ShowAccentPanel(List<string> keys, Keyboard.Modifier modifier, Transform anchor)
        {
            _anchor = anchor;
            ShowAccentPanel(keys, modifier);
        }
        public void ShowAccentPanel(List<string> keys, Keyboard.Modifier modifier)
        {
            _containerRectTransform = _container.GetCachedComponent(ref _containerRectTransform);
            transform.position = _anchor.position;
            transform.rotation = _anchor.rotation;

            transform.localPosition += anchorOffset + HorizontalOffset(_anchor, keys.Count);
           
            this._keyCodes = keys;
          
            gameObject.SetActive(true);
            ClearPanel();
            GeneratePanel(modifier);


       
            if (_timeoutCoroutine != null)
            {
                StopCoroutine(_timeoutCoroutine);
            }
            _timeoutCoroutine = StartCoroutine(TimeOutPanel(timeout));
        }

       
        public void HideAccentPanel()
        {
            if (gameObject.activeSelf == true)
            {
              
                gameObject.SetActive(false);
            }
        }

        public void ClearPanel()
        {
            while (_container.transform.childCount > 0)
            {
                DestroyImmediate(_container.transform.GetChild(0).gameObject);
            }
        }


        public void GeneratePanel(Keyboard.Modifier modifier)
        {
            _keys = new KeyboardKey[_keyCodes.Count];
            for (var i = 0; i < _keyCodes.Count; i++)
            {
                var special = _keyCodes[i];
                var newKey = Instantiate(keyPrefab, _container.transform);


                newKey.Initialize(_keySize, new Vector2(_keySize.x * i, 0), special, special, KeyboardKey.KeyGroup.Accent, modifier);
                _keys[i] = newKey;
              
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_containerRectTransform);
            OnKeysCreated?.Invoke(_keys);
        }

      
        public Vector3 HorizontalOffset(Transform _keyTransform, int keyCount)
        {
            float midPoint = _keyTransform.parent.parent.childCount / 2f;
          
            float dir = -Mathf.Clamp(_keyTransform.parent.GetSiblingIndex() - midPoint, -1, 1);
        
            
            return horizontalOffset * dir * (keyCount / 2.0f);
        }
        public IEnumerator TimeOutPanel(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            HideAccentPanel();
        }

        public void Dismiss()
        {
            HideAccentPanel();
    
        }
    }
}
