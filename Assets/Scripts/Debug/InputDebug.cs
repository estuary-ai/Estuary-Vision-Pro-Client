using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputDebug : MonoBehaviour
{
	public GameObject testPrefab;
    private bool isPressed;
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        var activeTouches = Touch.activeTouches;
        // You can determine the number of active inputs by checking the count of activeTouches
        if (activeTouches.Count > 0)
        {
            if (!isPressed)
            {
                isPressed = true;
                // For getting access to PolySpatial (visionOS) specific data you can pass an active touch into the EnhancedSpatialPointerSupport()
                SpatialPointerState primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);

                SpatialPointerKind interactionKind = primaryTouchData.Kind;
                GameObject objectBeingInteractedWith = primaryTouchData.targetObject;
                Vector3 interactionPosition = primaryTouchData.inputDevicePosition;
                Debug.Log("Pinch Location: " + interactionPosition);
                Instantiate(testPrefab, interactionPosition, Quaternion.identity);
            }
        }
        else if (activeTouches.Count == 0) isPressed = false;
    }
}