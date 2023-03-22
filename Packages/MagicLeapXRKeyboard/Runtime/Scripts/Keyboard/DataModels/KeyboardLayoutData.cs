using System.Collections.Generic;
using Toolbox;
using UnityEngine;

namespace MagicLeap.XRKeyboard.Model
{
    [System.Serializable]
    public class KeyboardLayoutData 
    {
        [System.Serializable]
        public class Key
        {

            public string KeyIconPath;
            public string KeyLabel;
            public string KeyCode;
            public float WidthScale = 1;
            public List<string> Accents;
            public Color BackgroundColor = Color.white;

      
            /// <summary>
            /// Left, Top, Right, Bottom
            /// </summary>
            [Tooltip("Left,Top,Right,Bottom")]
            public Vector4 Margins = new Vector4(50, 50, 50, 98.2f);
  

            public Key()
            {
                Accents = new List<string>();
            }
            
            
        }

        [System.Serializable]
        public class Row
        {
            [Tooltip("% size")]
            public float Size = 1;

            [Tooltip("horizontal gap between keys in this row.")]
            public float Spacing = 0;

            [Tooltip("Vertical gap following this row.")]
            public int VerticalGap = 0;
            [Tooltip("Key size that should be used.")]
            public Vector2 PreferredKeySize = new Vector2(300,300);
            [Tooltip("Shows the first accent under the main character.")]
            public bool ShowAccentHint = true;

            [LabelByChild("KeyLabel")]
            public List<Key> Keys;
        }

        public string LayoutId;
        public string Description;

        /// <Summary>
        /// Each dictionary entry contains a reference to the transform
        /// for a row of keys and a list of keys to be present in the row
        ///</Summary>
        public List<Row> Rows;

        public KeyboardLayoutData()
        {
            Rows = new List<Row>();
        }

        public virtual List<Row> GetKeyboardRows()
        {
            return Rows;
        }

        public bool ValidateKeyMap()
        {


            return true;
        }
    }
}
