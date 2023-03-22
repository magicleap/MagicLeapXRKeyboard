﻿using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
	[CustomPropertyDrawer(typeof(LabelByChildAttribute))]
    public class LabelByChildAttributeDrawer : PropertyDrawer
    {
        private GUIContent GetLabelByValue(SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                    label.text = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    label.text = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    label.text = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    label.text = property.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    label.text = property.colorValue.ToString();
                    break;
                case SerializedPropertyType.ObjectReference:
                    label.text = property.objectReferenceValue ? property.objectReferenceValue.name : "null";
                    break;
                case SerializedPropertyType.LayerMask:
                    switch (property.intValue)
                    {
                        case 0:
                            label.text = "Nothing";
                            break;
                        case ~0:
                            label.text = "Everything";
                            break;
                        default:
                            label.text = LayerMask.LayerToName((int)Mathf.Log(property.intValue, 2));
                            break;
                    }
                    break;
                case SerializedPropertyType.Enum:
                    label.text = property.enumNames[property.enumValueIndex];
                    break;
                case SerializedPropertyType.Vector2:
                    label.text = property.vector2Value.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    label.text = property.vector3Value.ToString();
                    break;
                case SerializedPropertyType.Vector4:
                    label.text = property.vector4Value.ToString();
                    break;
                case SerializedPropertyType.Rect:
                    label.text = property.rectValue.ToString();
                    break;
                case SerializedPropertyType.Character:
                    label.text = ((char)property.intValue).ToString();
                    break;
                case SerializedPropertyType.Bounds:
                    label.text = property.boundsValue.ToString();
                    break;
                case SerializedPropertyType.Quaternion:
                    label.text = property.quaternionValue.ToString();
                    break;
                default:
                    break;
            }

            return label;
        }

        /// <summary>
        /// Safe equivalent of the <see cref="GetPropertyHeight"/> method.
        /// Provided property is previously validated by the <see cref="IsPropertyValid(SerializedProperty)"/> method.
        /// </summary>

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected  void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyName = Attribute.ChildName;
            var childProperty = property.FindPropertyRelative(propertyName);
            //validate availability of the child property
            if (childProperty != null)
            {
                //set new label if found (unknown types will be ignored)
                label = GetLabelByValue(childProperty, label);
            }
            else
            {
                var warningContent = new GUIContent(property.displayName + "does not exists.");
                warningContent.image = EditorGUIUtility.IconContent("console.warnicon").image;
                EditorGUI.LabelField(position, label);
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        public  bool IsPropertyValid(SerializedProperty property)
        {
            return property.hasChildren;
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
        private LabelByChildAttribute Attribute => attribute as LabelByChildAttribute;
    }
}
