using MagicLeap.XRKeyboard.Utilities.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace MagicLeap.XRKeyboard.Utilities
{
    /// <summary>
    ///  A simplified version of the MRTK's <a href="https://github.com/microsoft/MixedRealityToolkit-Unity/blob/main/Assets/MRTK/SDK/Features/Utilities/Solvers/Follow.cs">follow script</a>.
    /// </summary>
    /// <remarks>
    /// Follow solver positions an element in front of the of the tracked target (relative to its local forward axis).
    /// The element can be loosely constrained (a.k.a. tag-along) so that it doesn't follow until the tracked target moves
    /// beyond user defined bounds.
    /// </remarks>
    public class FollowTarget : MonoBehaviour
    {

        public enum SolverOrientationType
        {

            /// <summary>
            /// Face toward the tracked object
            /// </summary>
            FaceTrackedObject = 0,

            /// <summary>
            /// Orient towards SolverHandler's tracked object or TargetTransform
            /// </summary>
            YawOnly = 1,

        }

        [SerializeField]
        [Tooltip("The desired orientation of this object")]
        private SolverOrientationType _orientationType = SolverOrientationType.FaceTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        public SolverOrientationType OrientationType
        {
            get => _orientationType;
            set => _orientationType = value;
        }


        [SerializeField]
        [Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
        private float _minDistance = 0.3f;

        /// <summary>
        /// Min distance from eye to position element around, i.e. the sphere radius.
        /// </summary>
        public float MinDistance
        {
            get => _minDistance;
            set => _minDistance = value;
        }

        [SerializeField]
        [Tooltip("If 0, the position will update immediately.  Otherwise, the greater this attribute the slower the position updates")]
        private float _moveLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("If true, updates are smoothed to the target. Otherwise, they are snapped to the target")]
        private bool _smoothing = true;

        [SerializeField]
        [Tooltip("Max distance from eye to element")]
        private float _maxDistance = 0.9f;

        /// <summary>
        /// Max distance from eye to element.
        /// </summary>
        public float MaxDistance
        {
            get => _maxDistance;
            set => _maxDistance = value;
        }

        [SerializeField]
        [Tooltip("Default distance from eye to position element around, i.e. the sphere radius")]
        private float _defaultDistance = 0.7f;

        /// <summary>
        /// Initial placement distance. Should be between min and max.
        /// </summary>
        public float DefaultDistance
        {
            get => _defaultDistance;
            set => _defaultDistance = value;
        }

        [SerializeField]
        [Tooltip("The horizontal angle from the tracked target forward axis to this object will not exceed this value")]
        private float _maxViewHorizontalDegrees = 30f;

        /// <summary>
        /// The horizontal angle from the tracked target forward axis to this object will not exceed this value.
        /// </summary>
        public float MaxViewHorizontalDegrees
        {
            get => _maxViewHorizontalDegrees;
            set => _maxViewHorizontalDegrees = value;
        }

        [SerializeField]
        [Tooltip("If 0, the rotation will update immediately.  Otherwise, the greater this attribute the slower the rotation updates")]
        private float _rotateLerpTime = 0.1f;

        [SerializeField]
        [Tooltip("The vertical angle from the tracked target forward axis to this object will not exceed this value")]
        private float _maxViewVerticalDegrees = 20f;

        /// <summary>
        /// The vertical angle from the tracked target forward axis to this object will not exceed this value.
        /// </summary>
        public float MaxViewVerticalDegrees
        {
            get => _maxViewVerticalDegrees;
            set => _maxViewVerticalDegrees = value;
        }

        [SerializeField]
        [Tooltip("The element will only reorient when the object is outside of the distance/direction bounds defined in this component.")]
        private bool _reorientWhenOutsideParameters = true;

        /// <summary>
        /// The element will only reorient when the object is outside of the distance/direction bounds above.
        /// </summary>
        public bool ReorientWhenOutsideParameters
        {
            get => _reorientWhenOutsideParameters;
            set => _reorientWhenOutsideParameters = value;
        }


        [SerializeField]
        [Tooltip("The element will not reorient until the angle between the forward vector and vector to the controller is greater then this value")]
        private float _orientToCameraDeadzoneDegrees = 30f;

        /// <summary>
        /// The element will not reorient until the angle between the forward vector and vector to the controller is greater then this value.
        /// </summary>
        public float OrientToCameraDeadzoneDegrees
        {
            get => _orientToCameraDeadzoneDegrees;
            set => _orientToCameraDeadzoneDegrees = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore angle clamping")]
        private bool _ignoreAngleClamp = false;

        /// <summary>
        /// Option to ignore angle clamping.
        /// </summary>
        public bool IgnoreAngleClamp
        {
            get => _ignoreAngleClamp;
            set => _ignoreAngleClamp = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore distance clamping")]
        private bool _ignoreDistanceClamp = false;

        /// <summary>
        /// Option to ignore distance clamping.
        /// </summary>
        public bool IgnoreDistanceClamp
        {
            get => _ignoreDistanceClamp;
            set => _ignoreDistanceClamp = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore the pitch and roll of the reference target")]
        private bool _ignoreReferencePitchAndRoll = false;

        /// <summary>
        /// Option to ignore the pitch and roll of the reference target
        /// </summary>
        public bool IgnoreReferencePitchAndRoll
        {
            get => _ignoreReferencePitchAndRoll;
            set => _ignoreReferencePitchAndRoll = value;
        }

        [SerializeField]
        [Tooltip("Pitch offset from reference element (relative to Max Distance)")]
        private float _pitchOffset = 0;

        /// <summary>
        /// Pitch offset from reference element (relative to MaxDistance).
        /// </summary>
        public float PitchOffset
        {
            get => _pitchOffset;
            set => _pitchOffset = value;
        }

        private Transform _transformTarget;
        private Vector3 _referencePosition => _transformTarget != null ? _transformTarget.position : Vector3.zero;
        private Quaternion _referenceRotation => _transformTarget != null ? _transformTarget.rotation : Quaternion.identity;
        private Quaternion _previousGoalRotation = Quaternion.identity;
        private bool _recenterNextUpdate = true;
        private Vector3 GoalPosition;
        private Quaternion GoalRotation;

        /// <summary>
        /// Re-centers the target in the next update.
        /// </summary>
        public void Recenter()
        {
            _recenterNextUpdate = true;
        }

        protected void OnEnable()
        {
            _transformTarget = Camera.main.transform;
            SnapGoalTo(GoalPosition, GoalRotation);
        }

        private void Start()
        {
            _transformTarget = Camera.main.transform;
        }

        public void SnapGoalTo(Vector3 position, Quaternion rotation)
        {
            GoalPosition = position;
            GoalRotation = rotation;
        }

        public void LateUpdate()
        {
            SolverUpdate();
        }

        private void SnapTo(Vector3 position, Quaternion rotation)
        {
            SnapGoalTo(position, rotation);


        }

        public void SolverUpdate()
        {


            GetReferenceInfo(out Vector3 refPosition, out Quaternion refRotation, out Vector3 refForward);

            // Determine the current position of the element
            Vector3 currentPosition = transform.position;
            if (_recenterNextUpdate)
            {
                currentPosition = refPosition + refForward * DefaultDistance;
            }

            bool wasClamped = false;

            // Angular clamp to determine goal direction to place the element
            Vector3 goalDirection = refForward;
            if (!_ignoreAngleClamp && !_recenterNextUpdate)
            {
                wasClamped |= AngularClamp(refPosition, refRotation, currentPosition, ref goalDirection);
            }

            // Distance clamp to determine goal position to place the element
            Vector3 goalPosition = currentPosition;
            if (!_ignoreDistanceClamp && !_recenterNextUpdate)
            {
                wasClamped |= DistanceClamp(currentPosition, refPosition, goalDirection, out goalPosition);
            }

            // Figure out goal rotation of the element based on orientation setting
            Quaternion goalRotation = Quaternion.identity;
            ComputeOrientation(goalPosition, wasClamped || _recenterNextUpdate, ref goalRotation);

            if (_recenterNextUpdate)
            {
                var cachedSmoothing = _smoothing;
                _smoothing = false;
                _previousGoalRotation = goalRotation;
                SnapTo(goalPosition, goalRotation);
                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
                _recenterNextUpdate = false;
                _smoothing = cachedSmoothing;
            }
            else
            {
                // Avoid drift by not updating the goal position when not clamped
                if (wasClamped)
                {
                    GoalPosition = goalPosition;
                }

                GoalRotation = goalRotation;
                _previousGoalRotation = goalRotation;
                UpdateWorkingPositionToGoal();
                UpdateWorkingRotationToGoal();
            }

        }

        /// <summary>
        /// Updates only the working position to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingPositionToGoal()
        {
            transform.position = _smoothing ? SmoothTo(transform.position, GoalPosition, Time.deltaTime, _moveLerpTime) : GoalPosition;
        }

        /// <summary>
        /// Updates only the working rotation to goal with smoothing, if enabled
        /// </summary>
        public void UpdateWorkingRotationToGoal()
        {
            transform.rotation = _smoothing ? SmoothTo(transform.rotation, GoalRotation, Time.deltaTime, _rotateLerpTime) : GoalRotation;
        }



        /// <summary>
        /// Lerps Vector3 source to goal.
        /// </summary>
        /// <remarks>
        /// Handles lerpTime of 0.
        /// </remarks>
        public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
        {
            return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }


        /// <summary>
        /// Calculates the angle between vec and a plane described by normal. The angle returned
        /// is signed.
        /// </summary>
        /// <returns>Signed angle between vec and the plane described by normal</returns>
        private float AngleBetweenVectorAndPlane(Vector3 vec, Vector3 normal)
        {
            return 90 - (Mathf.Acos(Vector3.Dot(vec, normal)) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// This method ensures that the refForward vector remains within the bounds set by the
        /// leashing parameters. To do this, it determines the angles between toTarget and the reference
        /// local xz and yz planes. If these angles fall within the leashing bounds, then we don't have
        /// to modify refForward. Otherwise, we apply a correction rotation to bring it within bounds.
        /// </summary>
        private bool AngularClamp(Vector3 refPosition, Quaternion refRotation, Vector3 currentPosition, ref Vector3 refForward)
        {

            Vector3 toTarget = currentPosition - refPosition;
            float currentDistance = toTarget.magnitude;
            if (currentDistance <= 0)
            {
                // No need to clamp
                return false;
            }

            toTarget.Normalize();

            // Start off with a rotation towards the target. If it's within leashing bounds, we can leave it alone.
            Quaternion rotation = Quaternion.LookRotation(toTarget, Vector3.up);

            Vector3 currentRefForward = refRotation * Vector3.forward;
            Vector3 refRight = refRotation * Vector3.right;

            bool angularClamped = false;

            // X-axis leashing
            // Leashing around the reference's X axis only makes sense if the reference isn't gravity aligned.
            if (IgnoreReferencePitchAndRoll)
            {
                float angle = MathUtils.AngleBetweenOnPlane(toTarget, currentRefForward, refRight);
                rotation = Quaternion.AngleAxis(angle, refRight) * rotation;
            }
            else
            {
                float angle = -MathUtils.AngleBetweenOnPlane(toTarget, currentRefForward, refRight);
                var minMaxAngle = MaxViewVerticalDegrees * 0.5f;


                if (angle < -minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(-minMaxAngle - angle, refRight) * rotation;
                    angularClamped = true;
                }
                else if (angle > minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(minMaxAngle - angle, refRight) * rotation;
                    angularClamped = true;
                }
            }

            float yAngle = AngleBetweenVectorAndPlane(toTarget, refRight);
            float yMinMaxAngle = MaxViewHorizontalDegrees * 0.5f;

            if (yAngle < -yMinMaxAngle)
            {
                rotation = Quaternion.AngleAxis(-yMinMaxAngle - yAngle, Vector3.up) * rotation;
                angularClamped = true;
            }
            else if (yAngle > yMinMaxAngle)
            {
                rotation = Quaternion.AngleAxis(yMinMaxAngle - yAngle, Vector3.up) * rotation;
                angularClamped = true;
            }

            refForward = rotation * Vector3.forward;
            return angularClamped;

        }


        /// <summary>
        /// This method ensures that the distance from clampedPosition to the tracked target remains within
        /// the bounds set by the leashing parameters. To do this, it clamps the current distance to these
        /// bounds and then uses this clamped distance with refForward to calculate the new position. If
        /// IgnoreReferencePitchAndRoll is true and we have a PitchOffset, we only apply these calculations
        /// for xz.
        /// </summary>
        private bool DistanceClamp(Vector3 currentPosition, Vector3 refPosition, Vector3 refForward, out Vector3 clampedPosition)
        {
            float clampedDistance;
            float currentDistance = Vector3.Distance(currentPosition, refPosition);
            Vector3 direction = refForward;
            if (IgnoreReferencePitchAndRoll && PitchOffset != 0)
            {
                // If we don't account for pitch offset, the casted object will float up/down as the reference
                // gets closer to it because we will still be casting in the direction of the pitched offset.
                // To fix this, only modify the XZ position of the object.

                Vector3 directionXZ = refForward;
                directionXZ.y = 0;
                directionXZ.Normalize();

                Vector3 refToElementXZ = currentPosition - refPosition;
                refToElementXZ.y = 0;
                float desiredDistanceXZ = refToElementXZ.magnitude;

                Vector3 minDistanceXZVector = refForward * MinDistance;
                minDistanceXZVector.y = 0;
                float minDistanceXZ = minDistanceXZVector.magnitude;

                Vector3 maxDistanceXZVector = refForward * MaxDistance;
                maxDistanceXZVector.y = 0;
                float maxDistanceXZ = maxDistanceXZVector.magnitude;

                desiredDistanceXZ = Mathf.Clamp(desiredDistanceXZ, minDistanceXZ, maxDistanceXZ);

                Vector3 desiredPosition = refPosition + directionXZ * desiredDistanceXZ;
                float desiredHeight = refPosition.y + refForward.y * MaxDistance;
                desiredPosition.y = desiredHeight;

                direction = desiredPosition - refPosition;
                clampedDistance = direction.magnitude;
                direction /= clampedDistance;

                clampedDistance = Mathf.Max(MinDistance, clampedDistance);
            }
            else
            {
                clampedDistance = Mathf.Clamp(currentDistance, MinDistance, MaxDistance);
            }

            clampedPosition = refPosition + direction * clampedDistance;



            return MathUtils.Vector3EqualEpsilon(clampedPosition, currentPosition, 0.0001f);
        }

        private void ComputeOrientation(Vector3 goalPosition, bool needsRefresh, ref Quaternion orientation)
        {
            SolverOrientationType defaultOrientationType = OrientationType;

            if (!needsRefresh && _reorientWhenOutsideParameters)
            {
                Vector3 nodeToCamera = goalPosition - _referencePosition;
                float angle = Mathf.Abs(MathUtils.AngleBetweenOnPlane(transform.forward, nodeToCamera, Vector3.up));
                if (angle < OrientToCameraDeadzoneDegrees)
                {
                    orientation = _previousGoalRotation;
                    return;
                }
            }



            switch (defaultOrientationType)
            {

                case SolverOrientationType.YawOnly:
                    var lookAt = Quaternion.LookRotation(goalPosition - _referencePosition).eulerAngles;
                    orientation = _transformTarget != null ? Quaternion.Euler(0f, lookAt.y, 0f) : Quaternion.identity;
                    break;
                case SolverOrientationType.FaceTrackedObject:
                    orientation = _transformTarget != null ? Quaternion.LookRotation(goalPosition - _referencePosition) : Quaternion.identity;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }
        }

        private void GetReferenceInfo(out Vector3 refPosition, out Quaternion refRotation, out Vector3 refForward)
        {
            refPosition = _referencePosition;
            refRotation = _referenceRotation;
            if (IgnoreReferencePitchAndRoll)
            {
                Vector3 forward = _referenceRotation * Vector3.forward;
                forward.y = 0;
                refRotation = Quaternion.LookRotation(forward);
                if (PitchOffset != 0)
                {
                    Vector3 right = refRotation * Vector3.right;
                    forward = Quaternion.AngleAxis(PitchOffset, right) * forward;
                    refRotation = Quaternion.LookRotation(forward);
                }
            }

            refForward = refRotation * Vector3.forward;
        }

    }

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