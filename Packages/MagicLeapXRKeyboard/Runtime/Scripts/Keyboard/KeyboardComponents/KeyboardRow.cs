using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicLeap.XRKeyboard.Extensions;

namespace MagicLeap.XRKeyboard.Component
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(LayoutElement))]
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class KeyboardRow : MonoBehaviour
	{
		
		public RectTransform KeyRowRectTransform => _rectTransform;
		[SerializeField]
		private RectTransform _rectTransform;
		

		[SerializeField]
		private LayoutElement _layoutElement;

		[SerializeField]
		private HorizontalLayoutGroup _horizontalLayoutGroup;

		public List<KeyboardKey> Keys;
		private void Reset()
		{
			_rectTransform = transform.GetCachedComponent(ref _rectTransform);
			_layoutElement = transform.GetCachedComponent(ref _layoutElement);
			_horizontalLayoutGroup = transform.GetCachedComponent(ref _horizontalLayoutGroup);
		}

		public void Initialize(string rowName, Transform parent, Vector2 sizeDelta, Vector2 anchoredPosition, float flexSize, float spacing, int verticalGap)
		{
			gameObject.name = rowName;
			transform.SetParent(parent.transform);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			
			_rectTransform = transform.GetCachedComponent(ref _rectTransform);
			_layoutElement = transform.GetCachedComponent(ref _layoutElement);
			_horizontalLayoutGroup = transform.GetCachedComponent(ref _horizontalLayoutGroup);
			_horizontalLayoutGroup.padding.bottom = verticalGap;
			_horizontalLayoutGroup.spacing = spacing;
			_rectTransform.sizeDelta = sizeDelta;
			_rectTransform.pivot = new Vector2(0.5f, 1);
			_rectTransform.anchoredPosition = anchoredPosition;

			_layoutElement.flexibleHeight = flexSize;
			_horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
			_horizontalLayoutGroup.childControlHeight = true;
			_horizontalLayoutGroup.childControlWidth = true;
			_horizontalLayoutGroup.childScaleWidth = false;
			_horizontalLayoutGroup.childScaleHeight = false;
			_horizontalLayoutGroup.childForceExpandWidth = false;
			_horizontalLayoutGroup.childForceExpandHeight = false;
		}
	}
}
