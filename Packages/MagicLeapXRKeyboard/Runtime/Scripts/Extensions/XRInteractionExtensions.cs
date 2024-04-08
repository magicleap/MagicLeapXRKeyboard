using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MagicLeap.Common
{
	public static class XRInteractionExtensions
	{
		/// <summary>
		/// Adds colliders to the interactable.
		/// </summary>
		/// <param name="xrInteractable"></param>
		/// <param name="colliders"></param>
		/// <param name="clearCurrentColliders"></param>
		public static void AddColliders(this XRBaseInteractable xrInteractable, Collider[] colliders, bool clearCurrentColliders = false)
		{
			if (clearCurrentColliders)
			{
				xrInteractable.colliders.Clear();
			}

			foreach (Collider handleCollider in colliders)
			{

				if (!xrInteractable.colliders.Contains(handleCollider))
				{

					xrInteractable.colliders.Add(handleCollider);
				}
			}

			xrInteractable.ReRegisterInteractable();
		}

		public static void ReRegisterInteractable(this XRBaseInteractable xrInteractable)
		{
			xrInteractable.StartCoroutine(ReRegisterInteractableCoroutine(xrInteractable));
		}

		private static IEnumerator ReRegisterInteractableCoroutine(this XRBaseInteractable xrInteractable)
		{
			yield return new WaitForEndOfFrame();
			xrInteractable.interactionManager.UnregisterInteractable(xrInteractable as IXRInteractable);

			yield return new WaitForEndOfFrame();
			xrInteractable.interactionManager.RegisterInteractable(xrInteractable as IXRInteractable);

			yield return null;
		}
	}
}
