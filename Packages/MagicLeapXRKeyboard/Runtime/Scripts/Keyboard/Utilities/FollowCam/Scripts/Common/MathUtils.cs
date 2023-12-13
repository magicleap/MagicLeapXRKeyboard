using UnityEngine;

namespace MagicLeap.XRKeyboard.Utilities.Common
{
	public static class MathUtils
	{
		public static bool Vector3EqualEpsilon(Vector3 x, Vector3 y, float eps)
		{
			float sqrMagnitude = (x - y).sqrMagnitude;

			return sqrMagnitude > eps;
		}

		public static float SimplifyAngle(float angle)
		{
			while (angle > Mathf.PI)
			{
				angle -= 2 * Mathf.PI;
			}

			while (angle < -Mathf.PI)
			{
				angle += 2 * Mathf.PI;
			}

			return angle;
		}

		/// <summary>
		/// Projects from and to on to the plane with given normal and gets the
		/// angle between these projected vectors.
		/// </summary>
		/// <returns>Angle between project from and to in degrees</returns>
		public static float AngleBetweenOnPlane(Vector3 from, Vector3 to, Vector3 normal)
		{

			from.Normalize();
			to.Normalize();
			normal.Normalize();

			Vector3 right = Vector3.Cross(normal, from);
			Vector3 forward = Vector3.Cross(right, normal);

			float angle = Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, forward));

			return SimplifyAngle(angle) * Mathf.Rad2Deg;

		}
	}
}
