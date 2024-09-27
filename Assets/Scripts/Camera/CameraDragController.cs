using Player.ActionHandlers;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraDragController : MonoBehaviourSingleton<CameraDragController>
    {
        private const float InertiaStep = .3f;
        private const float InertiaMultiplier = 2f;
        private const float InertiaDampingMultiplier = 10f;

        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private float sensitivity = 1f;

        [Space] [Header("Inertia")] [SerializeField]
        private bool inertiaEnabled = true;

        [Range(0f, 5f)] [SerializeField] private float inertiaDamping = 1.5f;

        [Space] [Header("Bounds")] [SerializeField]
        private bool boundsEnabled = true;

        [SerializeField] private Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 5f);

        private ClickHandler _clickHandler;

        private Vector3 _velocity;

        private bool _isDragging;
        private Vector3 _lastMousePosition;

        private Vector3 _lastInertiaPosition;
        private float _inertialElapsedTime;

        private bool _isEnabled = true;


        protected override void Init()
        {
            _clickHandler = ClickHandler.Instance;

            _clickHandler.DragStartEvent += OnDragStart;
            _clickHandler.DragEvent += OnDrag;
            _clickHandler.DragEndEvent += OnDragEnd;
        }

        private void OnDestroy()
        {
            _clickHandler.DragStartEvent -= OnDragStart;
            _clickHandler.DragEvent -= OnDrag;
            _clickHandler.DragEndEvent -= OnDragEnd;
        }

        private void LateUpdate()
        {
            if (!_isEnabled)
                return;

            var deltaTime = Time.deltaTime;

            if (_isDragging)
            {
                if (inertiaEnabled)
                {
                    _inertialElapsedTime += deltaTime;

                    if (_inertialElapsedTime >= InertiaStep)
                    {
                        _inertialElapsedTime = 0f;
                        _lastInertiaPosition = _lastMousePosition;
                    }
                }

                return;
            }

            SetCameraPosition(mainCamera.transform.position + _velocity * deltaTime);
            _velocity -= _velocity.normalized * (InertiaDampingMultiplier * inertiaDamping * deltaTime);
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void SetBounds(Bounds bounds)
        {
            this.bounds = bounds;

            if (!boundsEnabled)
                return;

            ClampPositionToBounds(bounds);
        }

        private void SetCameraPosition(Vector3 newPosition)
        {
            mainCamera.transform.position = newPosition;

            if (!boundsEnabled)
                return;

            ClampPositionToBounds(bounds);
        }

        private void ClampPositionToBounds(Bounds bounds)
        {
            var cameraTransform = mainCamera.transform;
            var position = cameraTransform.position;

            position = new Vector3
            (
                Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
                Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
                position.z
            );

            cameraTransform.position = position;
        }

        private void OnDragStart(Vector3 worldPos)
        {
            _isDragging = true;

            _velocity = Vector3.zero;

            if (!_isEnabled)
                return;

            _lastMousePosition = mainCamera.WorldToScreenPoint(worldPos);
            _lastInertiaPosition = _lastMousePosition;
        }

        private void OnDrag(Vector3 worldPos)
        {
            if (!_isEnabled)
                return;

            worldPos.z = 0f;

            var deltaPos = worldPos - mainCamera.ScreenToWorldPoint(_lastMousePosition);

            _lastMousePosition = mainCamera.WorldToScreenPoint(worldPos);

            var newPos = mainCamera.transform.position + -deltaPos * sensitivity;
            SetCameraPosition(newPos);
        }

        private void OnDragEnd(Vector3 worldPos)
        {
            _isDragging = false;

            if (!_isEnabled || !inertiaEnabled)
                return;

            worldPos.z = 0f;

            var delta = worldPos - mainCamera.ScreenToWorldPoint(_lastInertiaPosition);

            _velocity = -delta * InertiaMultiplier;
        }
    }
}