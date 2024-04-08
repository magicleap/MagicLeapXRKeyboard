
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

#if MAGICLEAP
using UnityEngine.XR.MagicLeap;
#endif


namespace MagicLeap.XRKeyboard.Component
{

	/// <summary>
	/// Links UI Input Events to the Magic Leap Controller Haptics.
	/// </summary>
	public class KeyHaptics : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
	{
#if MAGICLEAP
		private InputSubsystem.Extensions.Haptics.PreDefined _selectBuzz;
		private InputSubsystem.Extensions.Haptics.PreDefined _pressBuzz;
		private InputSubsystem.Extensions.Haptics.PreDefined _longPressBuzz;
#endif
		
		[SerializeField,Tooltip("Will Keyboard Key Up/Down events if assigned.")]
		private KeyboardKey _optionalKeyBoardKey;

		private bool _keyAssigned;
		private void Start()
		{
#if MAGICLEAP
			_selectBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.C);
			_pressBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.B);
			_longPressBuzz = InputSubsystem.Extensions.Haptics.PreDefined.Create(InputSubsystem.Extensions.Haptics.PreDefined.Type.A);
#endif

			
			_keyAssigned = _optionalKeyBoardKey != null;
			if(_keyAssigned)
			{
				_optionalKeyBoardKey.OnPress += OnPress;
				_optionalKeyBoardKey.OnLongPress += OnLongPress;
			}
		}

		private void OnLongPress()
		{
#if MAGICLEAP
			_longPressBuzz.StartHaptics();
#endif
		}
		private void OnPress()
		{
#if MAGICLEAP
			_pressBuzz.StartHaptics();
#endif
		}

		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
#if MAGICLEAP
			if (!_keyAssigned)
			{
				_pressBuzz.StartHaptics();
			}
#endif
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
#if MAGICLEAP
			if (!_keyAssigned)
			{
				_pressBuzz.StartHaptics();
			}
			else
			{
				_selectBuzz.StartHaptics();
			}
#endif
		}

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
#if MAGICLEAP
			_selectBuzz.StartHaptics();
#endif
		}

		/// <inheritdoc />
		public void OnPointerExit(PointerEventData eventData) { }



		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData) { }
	}
}