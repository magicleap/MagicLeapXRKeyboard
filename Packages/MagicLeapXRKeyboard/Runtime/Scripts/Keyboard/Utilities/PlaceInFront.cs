using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicLeap.XRKeyboard
{
    public class PlaceInFront :MonoBehaviour
    {
        public enum LookDirection
        {
            None = 0,
            LookAtCamera = 1,
            LookAwayFromCamera = 2
        }

        [SerializeField, Tooltip("The distance from the camera through its forward vector.")]
        private float _distance = 0.5f;

        [SerializeField, Tooltip("The offset of the object")]
        private Vector2 Offset = new Vector2(0, 0);
       
  

        [SerializeField, Tooltip("The approximate time it will take to reach the current position."), Range(.01f, 10.0f)]
        private float _positionSmoothTime = .1f;
        private Vector3 _positionVelocity = Vector3.zero;

        [SerializeField, Tooltip("The approximate time it will take to reach the current rotation."), Range(.01f, 10.0f)]
        private float _rotationSmoothTime = .1f;

        [SerializeField, Tooltip("The direction the transform should face.")]
        private LookDirection _lookDirection = LookDirection.LookAwayFromCamera;

        [SerializeField, Tooltip("Toggle to set position on awake.")]
        private bool _placeOnAwake = false;

        [SerializeField, Tooltip("Toggle to set position on enable.")]
        private bool _placeOnEnable = false;
        
        [SerializeField, Tooltip("Toggle to set position on update.")]
        private bool _placeOnUpdate = false;

        [SerializeField, Tooltip("Prevents the object from rotating around the Z axis.")]
        private bool _lockZRotation = false;

        [SerializeField, Tooltip("Prevents the object from rotating around the X axis.")]
        private bool _lockXRotation = false;

        [SerializeField, Tooltip("Places the object in front of and at the same Y position as the camera.")]
        private bool _lockToXZPlane = false;

        private bool _snapToTarget = false;
        
        [SerializeField, Tooltip("The (x, y) euler limits will force an update.")]
        private Vector2 _movementThreshold = new Vector2(10f, 5f);

        [SerializeField, Tooltip("Distance from the camera will force an update.")]
        private float _distanceThreshold = 0.15f;

        [SerializeField]
        private Transform _mainCamera = null;

        private Vector3 _movePosition = Vector3.zero;
        private Vector3 _lastCameraEuler = Vector3.zero;
        private Vector3 _lastCameraPosition = Vector3.zero;



        private void Reset()
        {
            if (_mainCamera == null&& Camera.main)
            {
                _mainCamera = Camera.main.transform;
            }
        
        }

        /// <summary>
        /// Set the transform from latest position if flag is checked.
        /// </summary>
        void Awake()
        {
            if (_mainCamera == null && Camera.main)
            {
                _mainCamera = Camera.main.transform;
            }
            if (_placeOnAwake)
            {
               
                _snapToTarget = true;
                SnapToTarget();
                StartCoroutine(UpdateTransformEndOfFrame());
            }
        }

        void Start()
        {
            if (_placeOnAwake)
            {

                _snapToTarget = true;
                SnapToTarget();
                StartCoroutine(UpdateTransformEndOfFrame());
            }
        }

        private void OnEnable()
        {
            if (_placeOnEnable)
            {
                _snapToTarget = true;
                StartCoroutine(UpdateTransformEndOfFrame());
            }
        }

        public void SnapToTarget()
        {
            if (_mainCamera == null && Camera.main)
            {
                _mainCamera = Camera.main.transform;
            }
            _snapToTarget = true;
            UpdateTransform(_mainCamera);
        }

        void Update()
        {
            if (!_snapToTarget && _placeOnUpdate)
            {
                UpdateTransform(_mainCamera);
            }
        }

        /// <summary>
        /// Reset position and rotation to match current camera values, after the end of frame.
        /// </summary>
        private IEnumerator UpdateTransformEndOfFrame()
        {
            if (_mainCamera== null && Camera.main)
            {
                _mainCamera = Camera.main.transform;
            }
            // Wait until the camera has finished the current frame.
            yield return new WaitForEndOfFrame();
          
            UpdateTransform(_mainCamera);
        }

        /// <summary>
        /// Reset position and rotation to match current camera values.
        /// </summary>
        private void UpdateTransform(Transform cameraTransform)
        {
            // Move the object in front of the camera with specified offsets.
            if (cameraTransform == null)
            {
                return;
            }
  
        
            var forwardVec = _lockToXZPlane ? new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized : cameraTransform.forward;
            var targetPosition = cameraTransform.position + (cameraTransform.up * Offset.y + cameraTransform.right * Offset.x) +  (forwardVec * _distance);

            if (Vector3.Distance(transform.position, _movePosition) > 0.01f ||
                Quaternion.Angle(Quaternion.Euler(cameraTransform.eulerAngles.x, 0, 0), Quaternion.Euler(_lastCameraEuler.x, 0, 0)) > _movementThreshold.y ||
                Quaternion.Angle(Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0), Quaternion.Euler(0, _lastCameraEuler.y, 0)) > _movementThreshold.x ||
                Vector3.Distance(cameraTransform.position, _lastCameraPosition) > _distanceThreshold)
            {

                _movePosition = targetPosition;

                _lastCameraEuler = cameraTransform.eulerAngles;
                _lastCameraPosition = cameraTransform.position;
            }

           
            transform.position = _snapToTarget ? targetPosition : Vector3.SmoothDamp(transform.position, _movePosition, ref _positionVelocity, _positionSmoothTime);
           

            Quaternion targetRotation = Quaternion.identity;

            // Rotate the object to face the camera.
            if (_lookDirection == LookDirection.LookAwayFromCamera)
            {
                targetRotation = Quaternion.LookRotation(transform.position - cameraTransform.position, cameraTransform.up);
            }
            else if (_lookDirection == LookDirection.LookAtCamera)
            {
                targetRotation = Quaternion.LookRotation(cameraTransform.position - transform.position, cameraTransform.up);
            }

            transform.rotation = _snapToTarget ? targetRotation : Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / _rotationSmoothTime);

            if (_snapToTarget)
            {
                _snapToTarget = false;
            }

            if (_lockZRotation)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            }

            if (_lockXRotation)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
        }
    }
}
