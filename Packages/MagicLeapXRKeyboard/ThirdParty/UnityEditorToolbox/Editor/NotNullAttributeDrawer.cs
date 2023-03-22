
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
  

    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : PropertyDrawer
    {
        private NotNullAttribute Attribute => attribute as NotNullAttribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.objectReferenceValue
                ? EditorGUI.GetPropertyHeight(property, label)
                : EditorGUI.GetPropertyHeight(property, label) + Style.boxHeight + Style.spacing * 2;
        }

        protected  void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUI.GetPropertyHeight(property);

            if (property.objectReferenceValue)
            {
                //draw property in the default way
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
            }
            else
            {
                //create respective HelpBox information
                var helpBoxRect = new Rect(position.x,
                                           position.y,
                                           position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, MessageType.Error);
                position.y += Style.boxHeight + Style.spacing * 2;

               
                var oldBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = Style.errorBackgroundColor;
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                GUI.backgroundColor = oldBackgroundColor;
            
            }
        }


        public bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        /// <summary>
        /// Native call to draw the provided property.
        /// </summary>
        public override sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsPropertyValid(property))
            {
                OnGUISafe(position, property, label);
                return;
            }

          
            //create additional warning log to the console window
            Debug.LogWarning($"The Attribute [NotNullAttribute] on {property.name} cannot be used on this property.");
            
            //create additional warning label based on the property name
            var warningContent = new GUIContent(property.displayName + " has invalid property drawer");
            warningContent.image = EditorGUIUtility.IconContent("console.warnicon").image;
            EditorGUI.LabelField(position, label);
        }



        private static class Style
        {

            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;

            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly Color errorBackgroundColor = Color.red;
        }
    }
}