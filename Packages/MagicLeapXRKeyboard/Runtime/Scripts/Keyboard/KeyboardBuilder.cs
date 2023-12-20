using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MagicLeap.XRKeyboard.Component;
using UnityEngine;
using UnityEngine.UI;
using MagicLeap.XRKeyboard.Extensions;
using MagicLeap.XRKeyboard.Model;
using UnityEngine.Serialization;

namespace MagicLeap.XRKeyboard
{
    /// <summary>
    /// Consumes JSON data to create the <see cref="KeyboardLayout"/> objects. The script requires that you assign the <see cref="KeyboardKey"/> and <see cref="KeyboardRow"/> prefab. 
    /// </summary>
    public class KeyboardBuilder : MonoBehaviour
    {
      
        
        [FormerlySerializedAs("keyPrefab")]
        [Header("Prefabs")]
        public KeyboardKey KeyPrefab;

        [FormerlySerializedAs("keyboardRowPrefab")]
        public KeyboardRow KeyboardRowPrefab;

        [FormerlySerializedAs("keyboardContainer")]
        [Header("Grouped Layout Settings")]
        public RectTransform KeyboardContainer;

        [Header("Data")]
        public KeyboardLayoutData keyboardLayoutData;

        [Header("GameObjects")]
        [HideInInspector]
        public List<KeyboardKey> Keys;
        [HideInInspector]
        public List<KeyboardRow> KeyRows;

        [Header("Save and Load Path")]
        [SerializeField,Tooltip("Provide a path to a JSON keymap file.")]
        private string _saveAndLoadPath;

        public string LayoutId
        {
            get
            {
                return keyboardLayoutData.LayoutId;
            }
        }


        // Start is called before the first frame update
        void Awake()
        {
          
            // If the keyboard is empty of keys then generate a new one
            if (GetComponentsInChildren<KeyboardKey>().Length == 0)
            {
                RegenerateKeyboard();
            }
        }


     


        public RectTransform AddHorizontalGap(Transform parent, Vector2 sizeDelta, Vector2 anchoredPosition, float flexSize)
        {
            var newKey = new GameObject($"Horizontal Gap")
                         {
                             transform =
                             {
                                 parent = parent,
                                 localPosition = Vector3.zero,
                                 localScale = Vector3.one,
                                 localRotation = Quaternion.identity
                             }
                         };
            var newKeyRectTransform = newKey.AddComponent<RectTransform>();
            newKeyRectTransform.sizeDelta = sizeDelta;
            newKeyRectTransform.anchoredPosition = anchoredPosition;
            var layoutElement = newKey.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = flexSize;
            return newKeyRectTransform;
        }

     


        [EasyButtons.Button]
        public void RegenerateKeyboard()
        {
         
            while (KeyboardContainer.childCount > 0)
            {
               DestroyImmediate(KeyboardContainer.transform.GetChild(0).gameObject);
            }

            Keys.Clear();
            KeyRows.Clear();



            var rows = keyboardLayoutData.GetKeyboardRows();
           
            var rect = KeyboardContainer.rect;
            Vector2 rowSize = new Vector2(rect.size.x, rect.size.y / rows.Count);
            float rowCenterX = KeyboardContainer.anchoredPosition.x / 2;
            float rowTopY  = KeyboardContainer.rect.size.y/2;

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var anchoredPosition = new Vector2(rowCenterX, rowTopY - (rowSize.y * i));
 

                var keyRow = Instantiate(KeyboardRowPrefab, KeyboardContainer.transform);
                keyRow.Initialize($"Row {i}", KeyboardContainer, rowSize, anchoredPosition, row.Size, row.Spacing,row.VerticalGap);
                Vector2 keySize = new Vector2(rowSize.x / row.Keys.Count, rowSize.y);
          
             
                for (var j = 0; j < row.Keys.Count; j++)
                {
                    var key = row.Keys[j];
                    if (key.KeyCode == "GAP")
                    {
                      var gap=  AddHorizontalGap(keyRow.transform, keySize, new Vector2(keySize.x * j, 0), key.WidthScale);
                    }
                    else
                    {
                        var sizeDelta = new Vector2(keySize.x * key.WidthScale, keySize.y);
                        var anchorPosition = new Vector2(keySize.x * j, 0);
                        var newKey = Instantiate(KeyPrefab, keyRow.transform);
                        newKey.Initialize(key, sizeDelta, anchorPosition, new Vector2(row.PreferredKeySize.x * key.WidthScale, row.PreferredKeySize.y) , row.ShowAccentHint);
                        Keys.Add(newKey);
                        keyRow.Keys.Add(newKey);
                    }

                  
                }

                KeyRows.Add(keyRow);
                keyRow.KeyRowRectTransform.MarkAsDirty($"Created Row: {keyRow.name}");
                LayoutRebuilder.ForceRebuildLayoutImmediate(keyRow.KeyRowRectTransform);
                Canvas.ForceUpdateCanvases();
            }

            KeyboardContainer.MarkAsDirty($"Created Keyboard: {KeyboardContainer.name}");
            LayoutRebuilder.ForceRebuildLayoutImmediate(KeyboardContainer);
            Canvas.ForceUpdateCanvases();
        }





     

        [EasyButtons.Button]
        // ReSharper disable once UnusedMember.Global
        public void LoadFromJSON()
        {
            var loadPath = _saveAndLoadPath;
            if (!File.Exists(loadPath))
            {
             
                var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, loadPath);
                if (File.Exists(streamingAssetsPath))
                {
                    loadPath = Path.GetFullPath(streamingAssetsPath).Replace("\\", "/");
                }
                else 
                {
                    loadPath = Path.GetFullPath(Path.Combine(Application.dataPath, loadPath)).Replace("\\","/");
                    
                }
            }

            if (!File.Exists(loadPath))
            {
                Debug.LogError($"File does not exist. {loadPath}");
            }
            Debug.Log($"loaded data from file: {loadPath}");
            var json = File.ReadAllText(loadPath);
            keyboardLayoutData = JsonUtility.FromJson<KeyboardLayoutData>(json);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            RegenerateKeyboard();
        }
#if UNITY_EDITOR
        [EasyButtons.Button]
        // ReSharper disable once UnusedMember.Local
        private void WriteNewJSON()
        {
            if (keyboardLayoutData.Rows.Count == 0 || keyboardLayoutData.Rows[0].Keys.Count == 0)
            {
                Debug.LogError("No map to write.");
                return;
            }
            
            for (var i = 0; i < keyboardLayoutData.Rows.Count; i++)
            {
                var row = keyboardLayoutData.Rows[i];
                for (var j = 0; j < row.Keys.Count; j++)
                {
                    var key = keyboardLayoutData.Rows[i].Keys[j];
                    if (string.IsNullOrWhiteSpace(key.KeyLabel))
                    {
                        key.KeyLabel = key.KeyCode;
                    }
                    
                }
            }

            string tempPath = _saveAndLoadPath;
            
            if (string.IsNullOrWhiteSpace(_saveAndLoadPath))
            {
                _saveAndLoadPath = $"Keymaps/{keyboardLayoutData.Description}.json";
                tempPath = Path.Combine(Application.dataPath, _saveAndLoadPath).Replace("\\", "/");
            }
          

            

            tempPath = tempPath.Replace("\\", "/");
            var dataPath = Application.dataPath.Replace("\\", "/");
            string jsonMap = JsonUtility.ToJson(keyboardLayoutData, true);

            if (!tempPath.Contains(".json"))
            {
                var fixAndContinue = UnityEditor.EditorUtility.DisplayDialog($"Incorrect file format",
                                                                        $"The path \"{tempPath}\"does not contain a '.json' extension. "
                                                                    + $"Do you want to correct the name to \"{tempPath}/{keyboardLayoutData.Description}.json\"?"
                                                                    , "Yes", "Cancel");
                if (!fixAndContinue)
                {
                    return;
                }
            }


    
            var savePath = tempPath;
            if (!Path.IsPathRooted(savePath))
            {
                savePath = Path.GetFullPath(Path.Combine(dataPath, savePath)).Replace("\\", "/");
           
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
            }
            
            if (Path.IsPathRooted(savePath) && !savePath.Contains(dataPath))
            {

       
                var writeFile = UnityEditor.EditorUtility.DisplayDialog($"File path outside of project folder",
                                                                            $"The path \"{savePath}\"is outside of the project folder. Do you still want to write to it?"
                                                                            , "Yes", "Cancel");
                if (!writeFile)
                {
                    return;
                }
            }


            if (File.Exists(savePath))
            {
               
               
                var choice = UnityEditor.EditorUtility.DisplayDialogComplex($"\"{Path.GetFileName(savePath)}\"already exists. Do you want to replace it?",
                                                                            $"A file with the same name already exists in directory: \"{Path.GetDirectoryName(savePath)}\". "
                                                                        + $"Replacing it will overwrite its current contents. This action cannot be undone", "Replace", "Cancel", "Keep Both");
                if (choice == 0)
                {
                    //Replace

                    File.WriteAllText(savePath, jsonMap);
                }

                else if (choice == 1)
                {
                    //Cancel
                    return;
                }
                else if (choice == 2)
                {
                    //Keep Both
                    var newPath = PathUtility.GetUniqueEnumeratedFileName(Path.GetDirectoryName(savePath), ".json", Path.GetFileNameWithoutExtension(savePath));
                    File.WriteAllText(savePath, newPath);
                 
                }

                return;
            }

        
            
            File.WriteAllText(savePath, jsonMap);
            Debug.Log($"Wrote KeyMap to [{savePath}]\njson output:\n{jsonMap.ToString()}");
            _saveAndLoadPath = _saveAndLoadPath.Replace(dataPath + "/", "");


        }
#endif


    }
}
