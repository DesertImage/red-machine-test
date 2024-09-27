using System;
using Connection;
using UnityEngine;

namespace Camera
{
    public class AutoCameraBounds : MonoBehaviour
    {
        [SerializeField] private ColorConnectionManager colorConnectionManager;
        private CameraDragController _cameraDragController;

        private void Awake()
        {
            _cameraDragController = CameraDragController.Instance;
        }

        private void Start()
        {
            RecalculateBounds();
        }

        private void RecalculateBounds()
        {
            var nodes = colorConnectionManager.Nodes;

            var minXNodePosition = Vector3.zero;
            var maxXNodePosition = Vector3.zero;
            var minYNodePosition = Vector3.zero;
            var maxYNodePosition = Vector3.zero;

            var centerPosition = Vector3.zero;

            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var nodePosition = node.transform.position;

                if (nodePosition.x < minXNodePosition.x)
                {
                    minXNodePosition = nodePosition;
                }

                if (nodePosition.x > maxXNodePosition.x)
                {
                    maxXNodePosition = nodePosition;
                }

                if (nodePosition.y < minYNodePosition.y)
                {
                    minYNodePosition = nodePosition;
                }

                if (nodePosition.y > maxYNodePosition.y)
                {
                    maxYNodePosition = nodePosition;
                }
            }

            // centerPosition = minXNodePosition + maxXNodePosition + minYNodePosition + maxYNodePosition;

            // var width = Math.Abs(maxXNodePosition.x) + Mathf.Abs(minXNodePosition.x);
            // var height = Math.Abs(maxYNodePosition.y) + Math.Abs(minYNodePosition.y);

            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            
            bounds.Encapsulate(minXNodePosition);
            bounds.Encapsulate(maxXNodePosition);
            bounds.Encapsulate(minYNodePosition);
            bounds.Encapsulate(maxYNodePosition);
            
            _cameraDragController.SetBounds
            (
                bounds
            );
        }

        private void OnValidate()
        {
            if (colorConnectionManager)
                return;

            colorConnectionManager = FindObjectOfType<ColorConnectionManager>();
        }
    }
}