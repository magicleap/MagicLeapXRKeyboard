using MagicLeap.XRKeyboard.Extensions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace MagicLeap.XRKeyboard.Component
{
    public class InputPreview : MonoBehaviour
    {
     

        [Header("Components")]
        [SerializeField] private TMP_InputField _previewInputField;
        [SerializeField] private RectTransform _rectTransform;
        
        private TMP_InputField _targetInputField;
        private bool _deselect;
        private bool _select;
        private bool _updateCaret;
        private int _targetCaret = 0;
        private bool _previewSelected;
        private int _startSelectionIndex;
        private int _endSelectionIndex;
        private bool _selecting;


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
            _previewInputField.shouldHideSoftKeyboard = true;
        }

        private void SubscribeToInputFiledEvents()
        {
            //Subscribe to text selection so the preview text selection effects the target text.
           _previewInputField.onTextSelection.AddListener(OnPreviewTextSelect);
            _previewInputField.onEndTextSelection.AddListener(OnPreviewTextSelectEnd);

            _previewInputField.onSelect.AddListener((_) =>
                                                    {
                                                        _previewSelected = true;
                                                    });

        }
        /// <summary>
        /// Unsubscribes target input field events and resets variables (position, selection, caret, etc).
        /// </summary>
        public void ClearField()
        {
      
            if (_targetInputField != null)
            {
              
                UpdateMesh(_targetInputField,false);
                _targetInputField.ReleaseSelection();
                _targetInputField.SetAllowInput(false);
                _targetInputField.onTextSelection.RemoveListener(OnTargetTextSelect);
                _targetInputField.onEndTextSelection.RemoveListener(OnTargetTextSelectEnd);
                _targetInputField.onValueChanged.RemoveListener(OnTargetValueChanged);
                _targetInputField.onSelect.RemoveListener(OnSelectTargetInput);
                _targetInputField.onSelect.RemoveListener(OnTargetSelect);
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

        private void OnTargetSelect(string s)
        {
            _previewSelected = false;
        }
        
        /// <summary>
        /// Sets the target input field and subscribes to it's events.
        /// <remarks>Clears old input if selected.</remarks>
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetTargetInputField(TMP_InputField newTarget)
        {
           
        ClearField();
        _targetInputField = newTarget;
        newTarget.onTextSelection.AddListener(OnTargetTextSelect);
        newTarget.onEndTextSelection.AddListener(OnTargetTextSelectEnd);
        newTarget.onValueChanged.AddListener(OnTargetValueChanged);
        newTarget.onSelect.AddListener(OnSelectTargetInput);
        newTarget.onSelect.AddListener(OnTargetSelect);
        
        _previewInputField.characterValidation = newTarget.characterValidation;
        _previewInputField.contentType = newTarget.contentType;
        _previewInputField.characterLimit = newTarget.characterLimit;
        _previewInputField.lineType = newTarget.lineType;
        _previewInputField.lineLimit = newTarget.lineLimit;
        _previewInputField.SetAllowInput(true);
        CopyCaretAndSelection(newTarget, _previewInputField,true);

        _previewInputField.text = newTarget.text;
        _previewInputField.textComponent.rectTransform.anchoredPosition = new Vector2(0, 0);
        _updateCaret = false;

        }

        /// <summary>
        /// If target text changes, update the caret position.
        /// <remarks>We cache the caret to set it to it's correct spot after TextMesh updates.</remarks>
        /// </summary>
        private void OnTargetValueChanged(string newText)
        {
            _previewInputField.text = (newText);
            _targetCaret = _targetInputField.caretPosition;
        
           _previewSelected = false;
            if (_targetInputField.caretPosition != _previewInputField.caretPosition)
            {
              CopyCaretAndSelection(_targetInputField, _previewInputField);
            }

        }

        /// <summary>
        /// If we select the target input field. take away selection control from preview (otherwise the caret will not move to the selected location).
        /// </summary>
        private void OnSelectTargetInput(string currentText)
        {
            _previewSelected = false;
        }

        /// <summary>
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here).
        /// </summary>
        private void OnPreviewTextSelect(string selectedString, int start, int end)
        {


            _previewSelected = true;
     
            if (_previewInputField == null || _targetInputField == null)
            {
                return;
            }
            if (_startSelectionIndex == start && _endSelectionIndex == end)
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

            _startSelectionIndex = start;
            _endSelectionIndex = end;
        }
        
        /// <summary>
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here).
        /// </summary>
        private void OnPreviewTextSelectEnd(string selectedString, int start, int end)
        {

            if (_previewInputField == null || _targetInputField == null)
            {
                return;
            }

            if (_startSelectionIndex == start && _endSelectionIndex == end)
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

            _startSelectionIndex = start;
            _endSelectionIndex = end;
        }
        
        /// <summary>
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here).
        /// </summary>
        private void OnTargetTextSelectEnd(string selectedString, int start, int end)
        {
            _targetInputField.SetStringSelectPosition(_targetInputField.GetStringPPositionInternal());
            if (_previewInputField == null || _targetInputField == null)
            {
                return;
            }

            if (_startSelectionIndex == start && _endSelectionIndex == end)
            {
                return;
            }

            _startSelectionIndex = start;
            _endSelectionIndex = end;
        }
        /// <summary>
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here).
        /// </summary>
        private void OnTargetTextSelect(string selectedString, int start, int end)
        {
            _previewSelected = false;
        
            if (_previewInputField == null || _targetInputField == null)
            {
                return;
            }
          
            if (_startSelectionIndex == start && _endSelectionIndex == end)
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

            _startSelectionIndex = start;
            _endSelectionIndex = end;
        }

        /// <summary>
        /// Copies the selection variables from one input field to another. Also Updates the mesh so that the caret draws
        /// </summary>
        private void CopyCaretAndSelection(TMP_InputField from, TMP_InputField to, bool force = false, bool updateAnchorPosition = true)
        {
            if (from == null || to == null)
            {
                return;
            }
         
                
            if ((to.caretPosition != from.caretPosition || force))
            {
             
                to.caretPosition = from.caretPosition;
                if (force)
                {
                    UpdateMesh(from);
                    UpdateMesh(to);
                }
                else
                {
                    if (!from.HasSelection())
                    {
                        UpdateMesh(from);
                        UpdateMesh(to);
                    }
                    else
                    {
                        DoHighlight(from, to);
                        DoHighlight(from, from);
                    }
                }
   
             
            }

            to.stringPosition = from.stringPosition;
            to.selectionStringAnchorPosition = from.selectionStringAnchorPosition;
            to.selectionStringFocusPosition = from.selectionStringFocusPosition;
            to.selectionFocusPosition = from.selectionFocusPosition;
            if (updateAnchorPosition)
            {
                to.selectionAnchorPosition = from.selectionAnchorPosition;
            }

            to.caretBlinkRate = .85f;
            from.caretBlinkRate = .85f;
        }

        /// <summary>
        /// Reflections to update the caret
        /// </summary>
        private void UpdateMesh(TMP_InputField tmpInput, bool select = true)
        {
            tmpInput.SetAllowInput(select);
            if (select)
            {
                tmpInput.SetCaretActive();
            }
            if (tmpInput.IsActive())
            {
                tmpInput.Rebuild(CanvasUpdate.LatePreRender);
                tmpInput.TryFillVBO();
            
            }

        }

        private void DoHighlight(TMP_InputField from, TMP_InputField to)
        {
            to.SetCaretActive();
       
            var cr = to.GetCachedInputRenderer();
            var mesh = to.GetMesh();
            if (mesh == null)
            {
                return;
            }

            using (var vbo = new VertexHelper())
            {
               
                var caretPosition = to.GetCaretPositionFromStringIndex(from.selectionStringAnchorPosition);
                var caretSelectPosition = to.GetCaretPositionFromStringIndex(from.selectionStringFocusPosition);
                

                int startChar = Mathf.Max(0, caretPosition);
                int endChar = Mathf.Max(0, caretSelectPosition);

               
                if (startChar > endChar)
                {
                    (startChar, endChar) = (endChar, startChar);
                }

                endChar -= 1;


                var text = to.GetTextComponent();
                var textInfo = text.textInfo;
                int currentLineIndex = textInfo.characterInfo[startChar].lineNumber;
                int nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;
                var selectionColor = from.selectionColor;
                UIVertex vert = UIVertex.simpleVert;
                vert.uv0 = Vector2.zero;
                vert.color = selectionColor;

                int currentChar = startChar;
                while (currentChar <= endChar && currentChar < textInfo.characterCount)
                {
                    if (currentChar == nextLineStartIdx || currentChar == endChar)
                    {
                        TMP_CharacterInfo startCharInfo = textInfo.characterInfo[startChar];
                        TMP_CharacterInfo endCharInfo = textInfo.characterInfo[currentChar];

                        // Extra check to handle Carriage Return
                        if (currentChar > 0 && endCharInfo.character == 10 && textInfo.characterInfo[currentChar - 1].character == 13)
                            endCharInfo = textInfo.characterInfo[currentChar - 1];

                        Vector2 startPosition = new Vector2(startCharInfo.origin, textInfo.lineInfo[currentLineIndex].ascender);
                        Vector2 endPosition = new Vector2(endCharInfo.xAdvance, textInfo.lineInfo[currentLineIndex].descender);

                        var startIndex = vbo.currentVertCount;
                        vert.position = new Vector3(startPosition.x, endPosition.y, 0.0f);
                        vbo.AddVert(vert);

                        vert.position = new Vector3(endPosition.x, endPosition.y, 0.0f);
                        vbo.AddVert(vert);

                        vert.position = new Vector3(endPosition.x, startPosition.y, 0.0f);
                        vbo.AddVert(vert);

                        vert.position = new Vector3(startPosition.x, startPosition.y, 0.0f);
                        vbo.AddVert(vert);

                        vbo.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                        vbo.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 0);

                        startChar = currentChar + 1;
                        currentLineIndex++;

                        if (currentLineIndex < textInfo.lineCount)
                            nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;
                    }

                    currentChar++;
                }

                vbo.FillMesh(mesh);
                cr.SetMesh(mesh);
            }
        }
      
        public void SetParent(RectTransform panel)
        {
            _rectTransform.SetParent(panel);
            _rectTransform.SetAsFirstSibling();
        }

        public void SetParent(Transform panel)
        {
            _rectTransform.SetParent(panel,false);
            _rectTransform.SetAsFirstSibling();
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
                           CopyCaretAndSelection(_previewInputField, _targetInputField, false, false);
                        }
                    }

                    //If the caret is moved and no selection is being made, update the caret on the preview
                    if (!_previewSelected && !_targetInputField.HasSelection())
                    {
                        if (_targetInputField.caretPosition != _previewInputField.caretPosition)
                        {
                          CopyCaretAndSelection(_targetInputField, _previewInputField,false, false);
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
                        _select = false;
                    }
            }
        }
    }
}
