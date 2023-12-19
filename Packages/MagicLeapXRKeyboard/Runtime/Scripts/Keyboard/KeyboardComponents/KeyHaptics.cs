
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.XRKeyboard.Component
{

	/// <summary>
	/// Links UI Input Events to the Magic Leap Controller Haptics.
	/// </summary>
	public class KeyHaptics : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
	{

		private InputSubsystem.Extensions.Haptics.PreDefined _selectBuzz;
		private InputSubsystem.Extensions.Haptics.PreDefined _pressBuzz;
		private InputSubsystem.Extensions.Haptics.PreDefined _longPressBuzz;
		[SerializeField,Tooltip("Will Keyboard Key Up/Down events if assigned.")]
		private KeyboardKey _optionalKeyBoardKey;

		private bool _keyAssigned;
		private void Start()
		{
			_selectBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.C);
			_pressBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.B);
			_longPressBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.A);
			
			_keyAssigned = _optionalKeyBoardKey != null;
			if(_keyAssigned)
			{
				_optionalKeyBoardKey.OnPress += OnPress;
				_optionalKeyBoardKey.OnLongPress += OnLongPress;
			}
		}

		private void OnLongPress()
		{
			_longPressBuzz.StartHaptics();
		}
		private void OnPress()
		{
			_pressBuzz.StartHaptics();
		}

		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!_keyAssigned)
			{
				_pressBuzz.StartHaptics();
			}
			
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			if (!_keyAssigned)
			{
				_pressBuzz.StartHaptics();
			}
			else
			{
				_selectBuzz.StartHaptics();
			}
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{

			_selectBuzz.StartHaptics();
		
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData) { }



		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData) { }
	}
}