using System;
using Camera;
using UnityEngine;
using Utils.Singleton;


namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;

        public event Action<Vector3> PointerDownEvent;
        public event Action<Vector3> ClickEvent;
        public event Action<Vector3> PointerUpEvent;
        public event Action<Vector3> DragStartEvent;
        public event Action<Vector3> DragEvent;
        public event Action<Vector3> DragEndEvent;

        private Vector3 _pointerDownPosition;

        private Vector3 _pointerDragPosition;

        private Vector3 _startMousePos;
        
        private bool _isClick;
        private bool _isDrag;
        private float _clickHoldDuration;


        private void Update()
        {
            var worldMousePosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                _isClick = true;
                _clickHoldDuration = .0f;

                _pointerDownPosition = worldMousePosition;

                PointerDownEvent?.Invoke(_pointerDownPosition);

                _pointerDownPosition = new Vector3(_pointerDownPosition.x, _pointerDownPosition.y, .0f);
                _pointerDragPosition = _pointerDownPosition;
            }
            else if (Input.GetMouseButton(0))
            {
                if (_pointerDragPosition != worldMousePosition)
                {
                    _pointerDragPosition = worldMousePosition;
                    DragEvent?.Invoke(_pointerDragPosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var pointerUpPosition = worldMousePosition;

                if (_isDrag)
                {
                    DragEndEvent?.Invoke(pointerUpPosition);

                    _isDrag = false;
                }
                else
                {
                    ClickEvent?.Invoke(pointerUpPosition);
                }

                PointerUpEvent?.Invoke(pointerUpPosition);

                _isClick = false;
            }
        }

        private void LateUpdate()
        {
            if (_isClick)
            {
                _clickHoldDuration += Time.deltaTime;
                if (_clickHoldDuration >= clickToDragDuration)
                {
                    DragStartEvent?.Invoke(_pointerDownPosition);

                    _isClick = false;
                    _isDrag = true;
                }
            } else if (_isDrag)
            {
            }
        }

        public void SetDragEventHandlers(Action<Vector3> dragStartEvent, Action<Vector3> dragEndEvent)
        {
            ClearEvents();

            DragStartEvent = dragStartEvent;
            DragEndEvent = dragEndEvent;
        }

        public void ClearEvents()
        {
            DragStartEvent = null;
            DragEvent = null;
            DragEndEvent = null;
        }
    }
}