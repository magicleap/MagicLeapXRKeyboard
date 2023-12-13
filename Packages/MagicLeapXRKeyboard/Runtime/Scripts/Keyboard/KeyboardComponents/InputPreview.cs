using System.Collections;
using MagicLeap.XRKeyboard.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        private static readonly FieldInfo _stringPositionInternalFieldInfo = typeof(TMP_InputField).GetField("m_StringPosition", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_StringSelectPositionFieldInfo = typeof(TMP_InputField).GetField("m_StringSelectPosition", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
         private static readonly FieldInfo m_CachedInputRendererFieldInfo = typeof(TMP_InputField).GetField("m_CachedInputRenderer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_TextComponentFieldInfo = typeof(TMP_InputField).GetField("m_TextComponent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_CaretVisibleFieldInfo = typeof(TMP_InputField).GetField("m_CaretVisible", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo _allowInputFieldInfo = typeof(TMP_InputField).GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_CursorVertsFieldInfo = typeof(TMP_InputField).GetField("m_CursorVerts", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);


        private static readonly FieldInfo inputSystemFieldInfo = typeof(TMP_InputField).GetField("inputSystem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_ShouldUpdateIMEWindowPositionFieldInfo = typeof(TMP_InputField).GetField("m_ShouldUpdateIMEWindowPosition", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly FieldInfo m_PreviousIMEInsertionLineFieldInfo = typeof(TMP_InputField).GetField("m_PreviousIMEInsertionLine", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

            

        
            
            
        private static readonly PropertyInfo caretPositionInternalPropertyInfo  = typeof(TMP_InputField).GetProperty("caretPositionInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly PropertyInfo meshPropertyInfo = typeof(TMP_InputField).GetProperty("mesh", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

        private static readonly MethodInfo adjustRectTransformRelativeToViewportMethodInfo = typeof(TMP_InputField).GetMethod("AdjustRectTransformRelativeToViewport", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly MethodInfo fillVBOMethodInfo = typeof(TMP_InputField).GetMethod("OnFillVBO", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly MethodInfo setCaretActiveMethodInfo = typeof(TMP_InputField).GetMethod("SetCaretActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        private static readonly MethodInfo GetCaretPositionFromStringIndexMethodInfo = typeof(TMP_InputField).GetMethod("GetCaretPositionFromStringIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        
        
           

        [Header("Components")]
        [SerializeField] private TMP_InputField _previewInputField;
        [SerializeField] private RectTransform _rectTransform;
        private TMP_InputField _targetInputField;
        [SerializeField] private TMP_Text _selectedTextInfo;

        private bool _deselect;
        private bool _select;
        private bool _updateCaret;
        private int _targetCaret = 0;
        private bool _previewSelected;
        private int _startSelectionIndex;
        private int _endSelectionIndex;
        private bool _selecting;

        private BaseInput inputSystem
        {
            get
            {
                if (EventSystem.current && EventSystem.current.currentInputModule)
                    return EventSystem.current.currentInputModule.input;
                return null;
            }
        }


        public RectTransform GetRectTransform()
        {
            _rectTransform = gameObject.GetCachedComponent(ref _rectTransform);
            return _rectTransform;
        }

        private void UpdateInfo()
        {
            if (_targetInputField ==null && _selectedTextInfo == null)
            {
                return;
            }


            _selectedTextInfo.text = $"caretPosition: {_targetInputField.caretPosition}\n"
                                 + $"stringPosition: {_targetInputField.stringPosition}\n"
                                 + $"selectionStringAnchorPosition: {_targetInputField.selectionStringAnchorPosition}\n"
                                 + $"selectionStringFocusPosition: {_targetInputField.selectionStringFocusPosition}\n"
                                 + $"selectionFocusPosition: {_targetInputField.selectionFocusPosition}\n"
                                 + $"selectionAnchorPosition: {_targetInputField.selectionAnchorPosition}";
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
            //Subscribe to text selection so the preview text selection effects the target text
           _previewInputField.onTextSelection.AddListener(OnPreviewTextSelect);
            _previewInputField.onEndTextSelection.AddListener(OnPreviewTextSelectEnd);

            _previewInputField.onSelect.AddListener((_) =>
                                                    {
                                                        _previewSelected = true;
                                                    });
            _previewInputField.onDeselect.AddListener((_) =>
                                                    {
                                                        // _previewSelected = false;
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
        /// Clears old input if selected
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
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here)
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
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here)
        /// </summary>
        private void OnTargetTextSelectEnd(string selectedString, int start, int end)
        {
           // _previewSelected = false;
           
            
            m_StringSelectPositionFieldInfo.SetValue(_targetInputField, _stringPositionInternalFieldInfo.GetValue(_targetInputField));
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
           return;
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
        /// If text is selected, update the selection based on which input field was interacted with. cache the update so we can render it correctly (textmeshpro does not allow for updates here)
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
        /// copies the selection variables from one input field to another. Also Updates the mesh so that the caret draws
        /// </summary>
        private void CopyCaretAndSelection(TMP_InputField from, TMP_InputField to, bool force = false, bool updateAnchorPosition = true,bool allowMeshUpdate=true)
        {
            if (from == null || to == null)
            {
                return;
            }

           // m_ShouldActivateNextUpdateFieldInfo.SetValue(from, false);
           // m_ShouldActivateNextUpdateFieldInfo.SetValue(to, false);
            
            // m_PreventCallbackFieldInfo.SetValue(from, false);
            // m_PreventCallbackFieldInfo.SetValue(to,true);
            //
            // _stringPositionInternalFieldInfo.SetValue(to, from.selectionStringAnchorPosition);
            // m_StringSelectPositionFieldInfo.SetValue(to, m_StringSelectPositionFieldInfo.GetValue(from));
            // m_CaretSelectPositionFieldInfo.SetValue(to, m_CaretSelectPositionFieldInfo.GetValue(from));

         
                
            if (allowMeshUpdate && (to.caretPosition != from.caretPosition || force))
            {
             
                to.caretPosition = from.caretPosition;
                // if (!to.HasSelection())
                // {
                //  
                //     to.caretBlinkRate = .85f;
                //     from.caretBlinkRate = .85f;
                // }
                // else
                // {
                //     DoHighlight(from, to);
                // }

                if (!from.HasSelection())
                {
                    UpdateMesh(from);
                    UpdateMesh(to);
                    // to.caretBlinkRate = .85f;
                    // from.caretBlinkRate = .85f;
                }
                else
                {
                    DoHighlight(from, to);
                    DoHighlight(from, from);
                }
                if (to.selectionStringAnchorPosition != from.selectionStringFocusPosition)
                {
                 //   DoHighlight(from, to);
                    
                }
                else
                {
                  
                   // GenerateCaret(to);
               
                }
                // m_IsCaretPositionDirtyFieldInfo.SetValue(from, true);
                // m_IsCaretPositionDirtyFieldInfo.SetValue(to, true);
              
              //  GenerateCaret(to)
                //DoHighlight(from, from);
                // UpdateMesh(from);
                //
            }

            to.stringPosition = from.stringPosition;
            to.selectionStringAnchorPosition = from.selectionStringAnchorPosition;
            to.selectionStringFocusPosition = from.selectionStringFocusPosition;
            to.selectionFocusPosition = from.selectionFocusPosition;
            if (updateAnchorPosition)
                to.selectionAnchorPosition = from.selectionAnchorPosition;

            // _stringPositionInternalFieldInfo.SetValue(to, from.selectionStringAnchorPosition);
            // m_StringSelectPositionFieldInfo.SetValue(to, m_StringSelectPositionFieldInfo.GetValue(from));
            
           //  to.stringPosition = from.stringPosition;
           //  to.selectionStringAnchorPosition = from.selectionStringAnchorPosition;
           //  to.selectionStringFocusPosition = from.selectionStringFocusPosition;
           // to.selectionFocusPosition = from.selectionFocusPosition;
           //  if(updateAnchorPosition)
           //  to.selectionAnchorPosition = from.selectionAnchorPosition;
        
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
                tmpInput.Rebuild(CanvasUpdate.LatePreRender);
                object mesh = meshPropertyInfo.GetValue(tmpInput);
                if (mesh!= null)
                {
                   fillVBOMethodInfo.Invoke(tmpInput, new[] { mesh });
                
                
                }
            }

        }
        private void GenerateCaret(TMP_InputField tmpInput)
        {
            var m_CaretVisible=(bool) m_CaretVisibleFieldInfo.GetValue(tmpInput);
            var m_TextComponent = (TMP_Text)m_TextComponentFieldInfo.GetValue(tmpInput);
            var caretPositionInternal=(int)caretPositionInternalPropertyInfo.GetValue(tmpInput);
            var caretColor = tmpInput.caretColor;
            var m_ShouldUpdateIMEWindowPosition = (bool)m_ShouldUpdateIMEWindowPositionFieldInfo.GetValue(tmpInput);
            var m_PreviousIMEInsertionLine = (int)m_PreviousIMEInsertionLineFieldInfo.GetValue(tmpInput);
            var m_CachedInputRenderer = (CanvasRenderer)m_CachedInputRendererFieldInfo.GetValue(tmpInput);
            
            if (m_CaretVisible == false || m_TextComponent.canvas == null || tmpInput.readOnly)
                return;
            CanvasRenderer cr = (CanvasRenderer)m_CachedInputRendererFieldInfo.GetValue(tmpInput);
            object mesh = meshPropertyInfo.GetValue(tmpInput);
            using (var vbo = new VertexHelper())
            {
                UIVertex[] m_CursorVerts = (UIVertex[])m_CursorVertsFieldInfo.GetValue(tmpInput);
                if (m_CursorVerts == null)
                {
                    m_CursorVerts = new UIVertex[4];

                    for (int i = 0; i < m_CursorVerts.Length; i++)
                    {
                        m_CursorVerts[i] = UIVertex.simpleVert;
                        m_CursorVerts[i].uv0 = Vector2.zero;
                    }

                    m_CursorVertsFieldInfo.SetValue(tmpInput, m_CursorVerts);
                }

                float width = tmpInput.caretWidth;

                // TODO: Optimize to only update the caret position when needed.

                Vector2 startPosition = Vector2.zero;
                float height = 0;
                TMP_CharacterInfo currentCharacter;

                // Make sure caret position does not exceed characterInfo array size.
                if (caretPositionInternal >= m_TextComponent.textInfo.characterInfo.Length)
                    return;

                int currentLine = m_TextComponent.textInfo.characterInfo[caretPositionInternal].lineNumber;

                // Caret is positioned at the origin for the first character of each lines and at the advance for subsequent characters.
                if (caretPositionInternal == m_TextComponent.textInfo.lineInfo[currentLine].firstCharacterIndex)
                {
                    currentCharacter = m_TextComponent.textInfo.characterInfo[caretPositionInternal];
                    height = currentCharacter.ascender - currentCharacter.descender;

                    if (m_TextComponent.verticalAlignment == VerticalAlignmentOptions.Geometry)
                        startPosition = new Vector2(currentCharacter.origin, 0 - height / 2);
                    else
                        startPosition = new Vector2(currentCharacter.origin, currentCharacter.descender);
                }
                else
                {
                    currentCharacter = m_TextComponent.textInfo.characterInfo[caretPositionInternal - 1];
                    height = currentCharacter.ascender - currentCharacter.descender;

                    if (m_TextComponent.verticalAlignment == VerticalAlignmentOptions.Geometry)
                        startPosition = new Vector2(currentCharacter.xAdvance, 0 - height / 2);
                    else
                        startPosition = new Vector2(currentCharacter.xAdvance, currentCharacter.descender);

                }

                adjustRectTransformRelativeToViewportMethodInfo.Invoke(tmpInput, new object[] { startPosition, height, currentCharacter.isVisible });

                // Clamp Caret height
                float top = startPosition.y + height;
                float bottom = top - height;

                // Minor tweak to address caret potentially being too thin based on canvas scaler values.
                float scale = m_TextComponent.canvas.scaleFactor;

                m_CursorVerts[0].position = new Vector3(startPosition.x, bottom, 0.0f);
                m_CursorVerts[1].position = new Vector3(startPosition.x, top, 0.0f);
                m_CursorVerts[2].position = new Vector3(startPosition.x + width, top, 0.0f);
                m_CursorVerts[3].position = new Vector3(startPosition.x + width, bottom, 0.0f);

                // Set Vertex Color for the caret color.
                m_CursorVerts[0].color = caretColor;
                m_CursorVerts[1].color = caretColor;
                m_CursorVerts[2].color = caretColor;
                m_CursorVerts[3].color = caretColor;

                vbo.AddUIVertexQuad(m_CursorVerts);


                // Update position of IME window when necessary.
                if (m_ShouldUpdateIMEWindowPosition || currentLine != m_PreviousIMEInsertionLine)
                {
                    m_ShouldUpdateIMEWindowPositionFieldInfo.SetValue(tmpInput, false);
                    m_PreviousIMEInsertionLineFieldInfo.SetValue(tmpInput, currentLine);


                    // Calculate position of IME Window in screen space.
                    Camera cameraRef;
                    if (m_TextComponent.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        cameraRef = null;
                    else
                    {
                        cameraRef = m_TextComponent.canvas.worldCamera;

                        if (cameraRef == null)
                            cameraRef = Camera.current;
                    }

                    Vector3 cursorPosition = m_CachedInputRenderer.gameObject.transform.TransformPoint(m_CursorVerts[0].position);
                    Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(cameraRef, cursorPosition);
                    screenPosition.y = Screen.height - screenPosition.y;

                    if (inputSystem != null)
                        inputSystem.compositionCursorPos = screenPosition;

                }

                vbo.FillMesh((Mesh)mesh);
                cr.SetMesh((Mesh)mesh);
            }
        }
        private void DoHighlight(TMP_InputField from, TMP_InputField to)
        {
            setCaretActiveMethodInfo.Invoke(to, null);
            CanvasRenderer cr = (CanvasRenderer)m_CachedInputRendererFieldInfo.GetValue(to);
            object mesh = meshPropertyInfo.GetValue(to);
            if (mesh == null)
            {
                return;
            }

            using (var vbo = new VertexHelper())
            {
                var caretPosition = (int)GetCaretPositionFromStringIndexMethodInfo.Invoke(to, new object[] { from.selectionStringAnchorPosition });
                var caretSelectPosition = (int)GetCaretPositionFromStringIndexMethodInfo.Invoke(to, new object[] { from.selectionStringFocusPosition });

                int startChar = Mathf.Max(0, caretPosition);
                int endChar = Mathf.Max(0, caretSelectPosition);

               
                if (startChar > endChar)
                {
                    (startChar, endChar) = (endChar, startChar);
                }

                endChar -= 1;


                var text = (TMP_Text)m_TextComponentFieldInfo.GetValue(to);
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

                vbo.FillMesh((Mesh)mesh);
                cr.SetMesh((Mesh)mesh);
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
            UpdateInfo();
        
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
                        Debug.LogWarning("DESELECT");
                    }

                    //Show Selection
                    if (_select)
                    {
                       //  UpdateMesh(_targetInputField);
                       //  UpdateMesh(_previewInputField);
                       // _targetInputField.ForceLabelUpdate();
                       //  _previewInputField.ForceLabelUpdate();
                        _select = false;
                    }

            }

        }

    }
}
