using System.Reflection;
using UnityEngine;

namespace MagicLeap.XRKeyboard.Extensions
{
	using TMPro;

	public static class TextMeshProInputFieldExtensions
	{

		//Reflections to show the Caret even when text UI is not selected.
		//By storing them as static readonly variables, we significantly increase the speed of each call
		private static readonly FieldInfo _stringPositionInternalFieldInfo = typeof(TMP_InputField).GetField("m_StringPosition", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly FieldInfo _stringSelectPositionFieldInfo = typeof(TMP_InputField).GetField("m_StringSelectPosition", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly FieldInfo _cachedInputRendererFieldInfo = typeof(TMP_InputField).GetField("m_CachedInputRenderer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly FieldInfo _textComponentFieldInfo = typeof(TMP_InputField).GetField("m_TextComponent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly FieldInfo _allowInputFieldInfo = typeof(TMP_InputField).GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly PropertyInfo _meshPropertyInfo = typeof(TMP_InputField).GetProperty("mesh", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly MethodInfo _fillVBOMethodInfo = typeof(TMP_InputField).GetMethod("OnFillVBO", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly MethodInfo _setCaretActiveMethodInfo = typeof(TMP_InputField).GetMethod("SetCaretActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
		private static readonly MethodInfo _getCaretPositionFromStringIndexMethodInfo = typeof(TMP_InputField).GetMethod("GetCaretPositionFromStringIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

		public static int GetCaretPositionFromStringIndex(this TMP_InputField tmpInputField, int selectionStringAnchorPosition)
		{
			return (int) _getCaretPositionFromStringIndexMethodInfo.Invoke(tmpInputField, new object[] { selectionStringAnchorPosition });
		}

		public static void SetCaretActive(this TMP_InputField tmpInputField)
		{
			_setCaretActiveMethodInfo.Invoke(tmpInputField, null);
		}

		public static void TryFillVBO(this TMP_InputField tmpInputField)
		{
			var mesh = _meshPropertyInfo.GetValue(tmpInputField);
			if(mesh!= null)
			{
				_fillVBOMethodInfo.Invoke(tmpInputField, new object[] { mesh });
			}
		}

		public static void FillVBO(this TMP_InputField tmpInputField, Mesh value)
		{
			_fillVBOMethodInfo.Invoke(tmpInputField, new object[]{ value });
		}

		public static void SetMesh(this TMP_InputField tmpInputField, Mesh value)
		{
			_meshPropertyInfo.SetValue(tmpInputField, value);
		}

		public static TMP_Text GetTextComponent(this TMP_InputField tmpInputField)
		{
			return (TMP_Text)_textComponentFieldInfo.GetValue(tmpInputField);
		}

		

		public static CanvasRenderer GetCachedInputRenderer(this TMP_InputField tmpInputField)
		{
			return (CanvasRenderer)_cachedInputRendererFieldInfo.GetValue(tmpInputField);
		}

		public static Mesh GetMesh(this TMP_InputField tmpInputField)
		{
			return (Mesh)_meshPropertyInfo.GetValue(tmpInputField);
		}

		public static int GetStringPPositionInternal(this TMP_InputField tmpInputField)
		{
			return (int)_stringPositionInternalFieldInfo.GetValue(tmpInputField);
		}
		
		public static void SetStringPositionInternal(this TMP_InputField tmpInputField, int value)
		{
			_stringPositionInternalFieldInfo.SetValue(tmpInputField, value);
		}

		public static void SetStringSelectPosition(this TMP_InputField tmpInputField,int value)
		{
			_stringSelectPositionFieldInfo.SetValue(tmpInputField, value);
		}
		public static int GetStringSelectPosition(this TMP_InputField tmpInputField)
		{
			return (int)_stringSelectPositionFieldInfo.GetValue(tmpInputField);
		}
		
		
		public static bool GetAllowInput(this TMP_InputField tmpInputField)
		{
			return (bool)_allowInputFieldInfo.GetValue(tmpInputField);
		}

		public static void SetAllowInput(this TMP_InputField tmpInputField, bool value)
		{
			_allowInputFieldInfo.SetValue(tmpInputField, value);
		}


		public static bool HasSelection(this TMP_InputField tmpInputField)
		{
			return tmpInputField.selectionStringAnchorPosition != tmpInputField.stringPosition;

		}

	}
}
