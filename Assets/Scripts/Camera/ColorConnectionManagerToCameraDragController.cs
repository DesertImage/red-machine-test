using Connection;
using UnityEngine;

namespace Camera
{
    public class ColorConnectionManagerToCameraDragController : MonoBehaviour
    {
        [SerializeField] private ColorConnectionManager colorConnectionManager;
        private CameraDragController _cameraDragController;

        private void Awake()
        {
            _cameraDragController = CameraDragController.Instance;

            colorConnectionManager.StartConnectingEvent += StartConnectingEvent;
            colorConnectionManager.StopConnectingEvent += StopConnectingEvent;
        }

        private void OnDestroy()
        {
            colorConnectionManager.StartConnectingEvent -= StartConnectingEvent;
            colorConnectionManager.StopConnectingEvent -= StopConnectingEvent;
        }

        private void StartConnectingEvent(ColorNode obj)
        {
            _cameraDragController.Disable();
        }

        private void StopConnectingEvent(ColorNode obj)
        {
            _cameraDragController.Enable();
        }
    }
}