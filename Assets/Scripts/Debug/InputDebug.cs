using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
// #if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
// #endif

namespace PolySpatial.Samples
{
    public class InputDebug : MonoBehaviour
    {
        [SerializeField] private ApplicationReferences appRef;
        [SerializeField] private GameObject rightSpawnPrefab;
        [SerializeField] private GameObject leftSpawnPrefab;
        [SerializeField] private Transform polyspatialCamTransform;
        [SerializeField] private Material highlightMaterial;


// #if UNITY_INCLUDE_XR_HANDS
        private XRHandSubsystem handSubsystem;
        private XRHandJoint rightIndexTipJoint;
        private XRHandJoint rightThumbTipJoint;
        private XRHandJoint leftIndexTipJoint;
        private XRHandJoint leftThumbTipJoint;
        private bool activeRightPinch;
        private bool activeLeftPinch;
        private float scaledThreshold;

        private const float k_PinchThreshold = 0.02f;

        private bool isPressed;
        void OnEnable()
        {
            // EnhancedTouchSupport.Enable();
            GetHandSubsystem();
            scaledThreshold = k_PinchThreshold / polyspatialCamTransform.localScale.x;
        }

        void Update()
        {
            GameObject tappedObj = UpdateIndexThumb();
            if (tappedObj != null && tappedObj.CompareTag("Navigable"))
            {
                Debug.Log("[INPUT DEBUG] Tapped Object: " + tappedObj.name);
                tappedObj.GetComponent<MeshRenderer>().material = highlightMaterial;
                // add to navigableObjects
                appRef.navManager.AddNavigable(tappedObj);
            }
        }

        void GetHandSubsystem()
        {
            var xrGeneralSettings = XRGeneralSettings.Instance;
            if (xrGeneralSettings == null)
            {
                Debug.LogError("XR general settings not set");
            }

            var manager = xrGeneralSettings.Manager;
            if (manager != null)
            {
                var loader = manager.activeLoader;
                if (loader != null)
                {
                    handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                    if (!CheckHandSubsystem()) return;
                    handSubsystem.Start();
                }
            }
        }

        private bool CheckHandSubsystem()
        {
            if (handSubsystem == null)
            {
#if !UNITY_EDITOR
            Debug.LogError("Could not find Hand Subsystem");
#endif
                enabled = false;
                return false;
            }

            return true;
        }

        GameObject DetectPinch(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
        {
            var spawnObject = right ? rightSpawnPrefab : leftSpawnPrefab;

            if (index.trackingState != XRHandJointTrackingState.None &&
                thumb.trackingState != XRHandJointTrackingState.None)
            {
                Vector3 indexPos = Vector3.zero;
                Vector3 thumbPos = Vector3.zero;

                if (index.TryGetPose(out Pose indexPose))
                {
                    // adjust transform relative to the PolySpatial Camera transform
                    indexPos = polyspatialCamTransform.InverseTransformPoint(indexPose.position);
                }

                if (thumb.TryGetPose(out Pose thumbPose))
                {
                    thumbPos = polyspatialCamTransform.InverseTransformPoint(thumbPose.position);
                }

                var pinchDistance = Vector3.Distance(indexPos, thumbPos);
                GameObject output = null;
                if (pinchDistance <= scaledThreshold)
                {
                    if (!activeFlag)
                    {
                        if (right)
                        {
                            // Instantiate(spawnObject, indexPos, Quaternion.identity);
                            output = GetObjectFromFingerCast(indexPose);
                            activeFlag = true;
                        }
                        else
                        {
                            GameObject destNode = SetNavDest(indexPos);
                        }
                    }
                }
                else activeFlag = false;

                return output;
            }
            return null;
        }

        private GameObject GetObjectFromFingerCast(Pose indexPos)
        {
            indexPos.position = new Vector3(indexPos.position.x, indexPos.position.y, indexPos.position.z);
            Debug.Log("[INPUT DEBUG] Pinch Location: " + indexPos.position);
            // shoot a raycast in the direction indexPos is pointing with a layer mask for checking only objects on layer 29
            RaycastHit hit;
            if (Physics.Raycast(indexPos.position, polyspatialCamTransform.forward, out hit, 100f, 1<<29))
            {
                Debug.Log("[INPUT DEBUG] Pinch Object: " + hit.transform.name);
                return hit.transform.gameObject;
            }

            return null;
        }

        private GameObject NativeTouchInput()
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
                    Debug.Log("[INPUT DEBUG] Pinch Location: " + interactionPosition);
                    Debug.Log("[INPUT DEBUG] Pinch Object: " + objectBeingInteractedWith.name);
                    return objectBeingInteractedWith;
                }
            }
            else if (activeTouches.Count == 0) isPressed = false;
            return null;
        }

        private GameObject UpdateIndexThumb()
        {
            if (!CheckHandSubsystem()) return null;

            var updateSuccessFlags = handSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                // assign joint values
                rightIndexTipJoint = handSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
                rightThumbTipJoint = handSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

                return DetectPinch(rightIndexTipJoint, rightThumbTipJoint, ref activeRightPinch, true);
            }

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            {
                // assign joint values
                leftIndexTipJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
                leftThumbTipJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

                return DetectPinch(leftIndexTipJoint, leftThumbTipJoint, ref activeLeftPinch, false);
            }

            return null;
        }

        private GameObject SetNavDest(Vector3 position)
        {
            return Instantiate(leftSpawnPrefab, position, Quaternion.identity);
        }
// #endif
    }
}