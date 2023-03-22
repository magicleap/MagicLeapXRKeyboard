using System.Collections;
using MagicLeap.XRKeyboard.Extensions;

namespace MagicLeap.XRKeyboard.Component
{
    using System;
    using System.Linq;
    using System.Reflection;
    using TMPro;
    using UnityEngine;

    public class InputPreview : MonoBehaviour
    {
        //Reflections to show the Caret even when text UI is not selected
        private static readonly FieldInfo _allowInputFieldInfo = typeof(TMP_InputField).GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly PropertyInfo meshPropertyInfo = typeof(TMP_InputField).GetProperty("mesh", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly MethodInfo fillVBOMethodInfo = typeof(TMP_InputField).GetMethod("OnFillVBO", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly MethodInfo setCaretActiveMethodInfo = typeof(TMP_InputField).GetMethod("SetCaretActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

        [Header("Components")]
        [SerializeField] private TMP_InputField _previewInputField;
        [SerializeField] private RectTransform _rectTransform;
        public float YPositionOffset =0;
        private TMP_InputField _targetInputField;
        private bool _deselect;
        private bool _select;
        private bool _updateCaret;
        private int _targetCaret = 0;
        private bool _previewSelected;
       
        public RectTransform GetRectTransform()
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            return _rectTransform;
        }
        private void Awake()
        {
            _previewInputField = gameObject.GetCachedComponentInChildren(ref _previewInputField);
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            SubscribeToInputFiledEvents();
        }

        private void Start() 
        {
            _previewInputField.shouldHideMobileInput = true;
        }

        private void SubscribeToInputFiledEvents()
        {
            //Subscribe to text selection so the preview text selection effects the target text
            _previewInputField.onTextSelection.AddListener(OnTextSelect);
            _previewInputField.onEndTextSelection.AddListener(OnTextSelect);

            _previewInputField.onSelect.AddListener((_) =>
                                                    {
                                                        _previewSelected = true;
                                                    });
            _previewInputField.onDeselect.AddListener((_) =>
                                                    {
                                                        _previewSelected = false;
                                                    });

        }
        /// <summary>
        /// Unsubscribes target input field events and resets variables (position, selection, caret, etc)
        /// </summary>
        public void ClearField()
        {
      
            if (_targetInputField != null)
            {
              
                UpdateMesh(_targetInputField,false);
                _targetInputField.ReleaseSelection();
                _allowInputFieldInfo.SetValue(_targetInputField, false);
                _targetInputField.onTextSelection.RemoveListener(OnTextSelect);
                _targetInputField.onEndTextSelection.RemoveListener(OnTextSelect);
                _targetInputField.onValueChanged.RemoveListener(OnTargetValueChanged);
                _targetInputField.onSelect.RemoveListener(OnSelectTargetInput);
                _targetInputField.DeactivateInputField();
                _targetInputField = null;

            }

            _previewSelected = false;
            _deselect = false;
            _select = false;
            _previewInputField.text = "";
            _previewInputField.ReleaseSelection();
            _previewInputField.ForceLabelUpdate();

        
        }
        


        /// <summary>
        /// Sets the target input field and subscribes to it's events.
        /// Clears old input if selected
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetTargetInputField(TMP_InputField newTarget)
        {
            ClearField();
            _targetInputField = newTarget;
            newTarget.onTextSelection.AddListener(OnTextSelect);
            newTarget.onEndTextSelection.AddListener(OnTextSelect);
            newTarget.onValueChanged.AddListener(OnTargetValueChanged);
            newTarget.onSelect.AddListener(OnSelectTargetInput);
            _previewInputField.characterValidation = newTarget.characterValidation;
            _previewInputField.contentType = newTarget.contentType;
            _previewInputField.characterLimit = newTarget.characterLimit;
            _previewInputField.lineType = newTarget.lineType;
            _previewInputField.lineLimit = newTarget.lineLimit;
            _allowInputFieldInfo.SetValue(_previewInputField, true);
            CopyCaretAndSelection(newTarget, _previewInputField,true);
            
            _previewInputField.text = newTarget.text;
            _previewInputField.textComponent.rectTransform.anchoredPosition = new Vector2(0, 0);


        }

        /// <summary>
        /// If target text changes, update the caret position. (we cache the caret to set it to it's correct spot after TextMesh updates
        /// </summary>
        /// <param name="newText"></param>
        private void OnTargetValueChanged(string newText)
        {
            _previewInputField.text = (newText);
            _updateCaret = true;
            _targetCaret = _targetInputField.caretPosition;
        
           _previewSelected = false;
            if (_targetInputField.caretPosition != _previewInputField.caretPosition)
            {
              CopyCaretAndSelection(_targetInputField, _previewInputField);
            }

        }

        /// <summary>
        /// If we select the target input field. take away selection control from preview (otherwise the caret will not move to the selected location)
        /// </summary>
        private void OnSelectTargetInput(string currentText)
        {
            _previewSelected = false;
        }

        /// <summary>
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here)
        /// </summary>
        private void OnTextSelect(string selectedString, int start, int end)
        {
            if (_previewInputField == null || _targetInputField == null)
            {
                return;
            }
            _deselect = start == end;
            _select = start != end;
            if (_previewSelected)
            {
                CopyCaretAndSelection(_previewInputField, _targetInputField);
            }
            else
            {
                CopyCaretAndSelection(_targetInputField, _previewInputField);
            }
        }

        /// <summary>
        /// copies the selection variables from one input field to another. Also Updates the mesh so that the caret draws
        /// </summary>
        private void CopyCaretAndSelection(TMP_InputField from, TMP_InputField to, bool force = false)
        {
            if (to == null || to == null)
            {
                return;
            }
            if (to.caretPosition != from.caretPosition || force)
            {
              
                to.caretPosition = from.caretPosition;
                UpdateMesh(from);
                UpdateMesh(to);
        
            }
            
            to.stringPosition = from.stringPosition;
            to.selectionStringAnchorPosition = from.selectionStringAnchorPosition;
            to.selectionStringFocusPosition = from.selectionStringFocusPosition;
            to.selectionFocusPosition = from.selectionFocusPosition;
            to.selectionAnchorPosition = from.selectionAnchorPosition;
            to.caretBlinkRate = .85f;
            from.caretBlinkRate = .85f;
           
        }

        /// <summary>
        /// Reflections to update the caret
        /// </summary>
        private void UpdateMesh(TMP_InputField tmpInput, bool select = true)
        {
            _allowInputFieldInfo.SetValue(tmpInput, select);
            if (select)
            {
                setCaretActiveMethodInfo.Invoke(tmpInput, null);
            }
            if (tmpInput.IsActive())
            {
                object mesh = meshPropertyInfo.GetValue(tmpInput);
                if (mesh!= null)
                {
                    fillVBOMethodInfo.Invoke(tmpInput, new[] { mesh });
               
                }
            }

        }
        /// <summary>
        /// Aligns panel to the top edge of the given rect.
        /// </summary>
        public void StartResizeToRect(RectTransform panel)
        {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, panel.offsetMax.y + _rectTransform.sizeDelta.y +YPositionOffset);
            _rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x, _rectTransform.sizeDelta.y);
            if(gameObject.activeSelf)
            {
                StartCoroutine(DoUpdatePreviewSize(panel));
            }
        }
        //calls after canvas updates
        IEnumerator DoUpdatePreviewSize(RectTransform panel)
        {
            yield return null;
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, panel.offsetMax.y + _rectTransform.sizeDelta.y + YPositionOffset);
            _rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x, _rectTransform.sizeDelta.y);
        }

        private void Update()
        {
            if (_targetInputField)
            {
           
                    //If the caret is moved and no selection is being made, update the caret on the target
                    if (_previewSelected && !_previewInputField.HasSelection())
                    {
                        if(_targetInputField.caretPosition != _previewInputField.caretPosition)
                        {
                            CopyCaretAndSelection(_previewInputField, _targetInputField);
                        }
                    }

                    //If the caret is moved and no selection is being made, update the caret on the preview
                    if (!_previewSelected && !_targetInputField.HasSelection())
                    {
                        if (_targetInputField.caretPosition != _previewInputField.caretPosition)
                        {
                            CopyCaretAndSelection(_targetInputField, _previewInputField);
                        }
                    }

                    //Update caret after text is added to the input field
                    if (_updateCaret)
                    {
                        _previewSelected = false;
                        _previewInputField.caretPosition = _targetCaret;
                        CopyCaretAndSelection(_previewInputField,_targetInputField);
                        _updateCaret = false;
                        
                    }

                    //Hide visualized selection
                    if (_deselect)
                    {
                        _targetInputField.ReleaseSelection();
                        _previewInputField.ReleaseSelection();
                        UpdateMesh(_targetInputField);
                        UpdateMesh(_previewInputField);
                        _targetInputField.ForceLabelUpdate();
                        _previewInputField.ForceLabelUpdate();
                        _deselect = false;
                    }

                    //Show Selection
                    if (_select)
                    {
                        UpdateMesh(_targetInputField);
                        UpdateMesh(_previewInputField);
                        _targetInputField.ForceLabelUpdate();
                        _previewInputField.ForceLabelUpdate();
                        _select = false;
                    }

            }

        }

    }
}
