using System.Collections.Generic;
using System.IO;
using System.Linq;
using MagicLeap.XRKeyboard.Component;
using UnityEngine;
using UnityEngine.UI;
using MagicLeap.XRKeyboard.Extensions;
using MagicLeap.XRKeyboard.Model;

namespace MagicLeap.XRKeyboard
{
 
    public class KeyboardBuilder : MonoBehaviour
    {
      
        
        [Header("Prefabs")]
        public KeyboardKey keyPrefab;

        public KeyboardRow keyboardRowPrefab;

        [Header("Grouped Layout Settings")]
        public RectTransform keyboardContainer;

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
         
            while (keyboardContainer.childCount > 0)
            {
               DestroyImmediate(keyboardContainer.transform.GetChild(0).gameObject);
            }

            Keys.Clear();
            KeyRows.Clear();



            var rows = keyboardLayoutData.GetKeyboardRows();
           
            var rect = keyboardContainer.rect;
            Vector2 rowSize = new Vector2(rect.size.x, rect.size.y / rows.Count);
            float rowCenterX = keyboardContainer.anchoredPosition.x / 2;
            float rowTopY  = keyboardContainer.rect.size.y/2;

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var anchoredPosition = new Vector2(rowCenterX, rowTopY - (rowSize.y * i));
 

                var keyRow = Instantiate(keyboardRowPrefab, keyboardContainer.transform);
                keyRow.Initialize($"Row {i}", keyboardContainer, rowSize, anchoredPosition, row.Size, row.Spacing,row.VerticalGap);
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
                        var newKey = Instantiate(keyPrefab, keyRow.transform);
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

            keyboardContainer.MarkAsDirty($"Created Keyboard: {keyboardContainer.name}");
            LayoutRebuilder.ForceRebuildLayoutImmediate(keyboardContainer);
            Canvas.ForceUpdateCanvases();
        }





     

        [EasyButtons.Button]
        public void LoadFromJSON()
        {
            if (File.Exists(_saveAndLoadPath))
            {
                string json = File.ReadAllText(_saveAndLoadPath);
                JsonUtility.FromJsonOverwrite(json, keyboardLayoutData);
            }
            else
            {
                Debug.LogError("Path does not exist for keymap file.");
            }


            keyboardLayoutData.ValidateKeyMap();
        }

        [EasyButtons.Button]
        public void WriteNewJSON()
        {
            if (keyboardLayoutData.Rows.Count == 0 || keyboardLayoutData.Rows[0].Keys.Count == 0)
            {
                Debug.LogWarning("No map to write");
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

            
            var directory = Path.Combine(Application.streamingAssetsPath, "XRKeyboard/AndroidKeyMaps").Replace("\\", "/");
            var filePath = Path.Combine(directory, keyboardLayoutData.Description + ".json").Replace("\\", "/");
            _saveAndLoadPath = filePath;
            string jsonMap = JsonUtility.ToJson(keyboardLayoutData, true);


            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, jsonMap);
            Debug.Log($"Wrote KeyMap to [{filePath}]\njson output:\n{jsonMap.ToString()}");
        }

    }
}
