using System;
using Unity.PolySpatial;
using UnityEngine;

namespace PolySpatial.Samples
{
    // This simple script listens to window events and prints it out.
    // content).
    public class VolumeCameraListener : MonoBehaviour
    {
        Vector3 m_OriginalOutputDimensions;

        void OnEnable()
        {
            var volumeCamera = GetComponent<VolumeCamera>();

            // Save the original volume camera dimensions so we know what we're starting with.
            // Note: in case you want to change these dimensions dynamically, you'll need to
            // adapt this script to take that into account
            m_OriginalOutputDimensions = volumeCamera.OutputDimensions;

            volumeCamera.WindowStateChanged.AddListener(VolumeWindowResized);
            volumeCamera.ViewpointChanged.AddListener(VolumeViewpointChanged);
        }

        void OnDisable()
        {
            var volumeCamera = GetComponent<VolumeCamera>();
            volumeCamera.WindowStateChanged.RemoveListener(VolumeWindowResized);
            volumeCamera.ViewpointChanged.RemoveListener(VolumeViewpointChanged);
        }

        void VolumeViewpointChanged(VolumeCamera.Viewpoint viewpoint)
        {
            Debug.Log($"User is to the {viewpoint.ToString()} of the bounded volume linked to this volume camera \"{gameObject.name}\".");
        }

        void VolumeWindowResized(VolumeCamera volumeCamera, VolumeCamera.WindowState windowState)
        {
            switch (windowState.WindowEvent)
            {
                case VolumeCamera.WindowEvent.Resized:
                {
                    Debug.Log($"Volume window size has been resized to {windowState.OutputDimensions}, and the content has been rescaled to {windowState.ContentDimensions}.");
                    break;
                }
                case VolumeCamera.WindowEvent.Opened:
                {
                    Debug.Log($"Volume window has been opened, with a mode of {windowState.Mode}, and with a window size of {windowState.OutputDimensions}. " +
                              $"Content has been rescaled to {windowState.ContentDimensions}.");
                    break;
                }
                case VolumeCamera.WindowEvent.Closed:
                {
                    Debug.Log($"Volume window has been closed.");
                    break;
                }
                case VolumeCamera.WindowEvent.Backgrounded:
                {
                    Debug.Log($"Volume window has been backgrounded.");
                    break;
                }
                case VolumeCamera.WindowEvent.Focused:
                {
                    Debug.Log(windowState.IsFocused ? $"Volume window has come into focus." : $"Volume window has lost focus.");
                    break;
                }
            }
        }
    }
}
