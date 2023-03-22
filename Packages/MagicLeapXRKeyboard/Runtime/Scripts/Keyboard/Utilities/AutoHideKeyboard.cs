using System;
using MagicLeap.XRKeyboard.Component;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MagicLeap.XRKeyboard
{
	/// <summary>
	/// Dismisses the keyboard if another UI element is pressed
	/// </summary>
	public class AutoHideKeyboard : MonoBehaviour
	{
		public enum SearchType
		{
			LayerMask,
			Component
		}
		[SerializeField] private Keyboard _keyboard;
		[SerializeField] private LayerMask _keyboardLayerMask;
		[SerializeField] private SearchType _searchType = SearchType.Component;
		private GameObject _currentObject;
		private void Reset()
		{
			_keyboard = GetComponentInChildren<Keyboard>(true);
		}

		private void Update()
		{
			switch (_searchType)
			{
				case SearchType.LayerMask:
					SearchLayer();
					break;
				case SearchType.Component:
					SearchComponents();
					break;
			
			}

		}

		private void SearchComponents()
		{
			//Handle dismissing the keyboard if another UI element is pressed
			var currentSelectedObject = EventSystem.current.currentSelectedGameObject;

			if (currentSelectedObject != null && _currentObject != currentSelectedObject)
			{

		
				if (!currentSelectedObject.GetComponent<InputField>() && !currentSelectedObject.GetComponent<TMP_InputField>() && !currentSelectedObject.GetComponent<KeyboardKey>() && !currentSelectedObject.GetComponentInParent<Keyboard>())
				{

					if (_keyboard.IsEditing())
					{
						_keyboard.EndEdit();
						_currentObject = null;
					}

					_currentObject = null;


				}

				_currentObject = currentSelectedObject;
			}
		}

		private void SearchLayer()
		{
			//Handle dismissing the keyboard if another UI element is pressed
			var currentSelectedObject = EventSystem.current.currentSelectedGameObject;

			if (currentSelectedObject != null && _currentObject != currentSelectedObject)
			{

				var onKeyboardLayer = ((_keyboardLayerMask & (1 << currentSelectedObject.layer)) != 0);
				if (!onKeyboardLayer && !currentSelectedObject.GetComponent<InputField>() && !currentSelectedObject.GetComponent<TMP_InputField>())
				{

					if (_keyboard.IsEditing())
					{
						_keyboard.EndEdit();
						_currentObject = null;
					}

					_currentObject = null;


				}

				_currentObject = currentSelectedObject;
			}
		}
	}
}
