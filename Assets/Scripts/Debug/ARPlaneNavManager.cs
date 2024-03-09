// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using Unity.AI.Navigation;
// using UnityEngine;
// using UnityEngine.AI;

// // #if UNITY_INCLUDE_ARFOUNDATION
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;

// // #endif

// [Serializable]
// public class ARPlaneNavigationSurface
// {
//     public ARPlane plane;
//     public NavMeshSurface surface;
//     public bool isUpdating = false;
// }

// [Serializable]
// public class NavigationSurface
// {
//     public GameObject mesh;
//     public NavMeshSurface surface;
//     public bool isUpdating = false;

//     public NavigationSurface(GameObject m, NavMeshSurface s)
//     {
//         mesh = m;
//         surface = s;
//     }
// }

// public class ARPlaneNavManager : MonoBehaviour
// {
//     [SerializeField] private TMP_Text debugFloorText;
//     public Dictionary<ARPlane, NavigationSurface> navMeshSurfaces = new();

// // #if UNITY_INCLUDE_ARFOUNDATION
//     public void FixedUpdate()
//     {
//         int floorCount = 0;
//         ARPlane[] planes = FindObjectsOfType<ARPlane>();
//         NavMeshSurface navMeshSurface = null;
//         // delete all elements in navMeshSurfaces that are not in planes
//         foreach (var n in planes)
//         {
//             if(!navMeshSurfaces.ContainsKey(n))
//             {
//                 navMeshSurfaces.Remove(n);
//             }
//         }
//         foreach (var p in planes)
//         {
// #if UNITY_EDITOR
//             if (p.alignment == PlaneAlignment.HorizontalUp)
//             {
//                 Debug.Log("Plane found: " + p.alignment);
//                 // make a NavMesh Surface if doesn't exist
//                 if(p.GetComponent<NavMeshSurface>() == null)
//                 {
//                     navMeshSurface = p.gameObject.AddComponent<NavMeshSurface>();
//                     navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
//                     // navMeshSurface.BuildNavMesh();
//                 }
//                 else
//                 {
//                     navMeshSurface = p.gameObject.GetComponent<NavMeshSurface>();
//                 }
//                 // update navMesh dictionary
//                 if(navMeshSurfaces.ContainsKey(p) == false)
//                 {
//                     NavigationSurface navigationSurface = new NavigationSurface();
//                     navigationSurface.plane = p;
//                     navigationSurface.surface = navMeshSurface;
//                     navMeshSurfaces.Add(p, navigationSurface);
//                 }
//             }
// #endif
//             if (p.classification == PlaneClassification.Floor)
//             {
//                 floorCount++;
//                 // Debug.Log("Floor found: " + floorCount);
//             }
//         }
//         // update the navMeshSurfaces
//         foreach (var n in navMeshSurfaces)
//         {
//             if (n.Value.isUpdating == false && n.Value.surface != null)
//             {
//                 StartCoroutine(UpdateNavMesh(n.Value));
//             }
//         }
//         if (debugFloorText != null) debugFloorText.text = "Floor Count: " + floorCount;
//     }

//     private IEnumerator UpdateNavMesh(NavigationSurface navSurface)
//     {
//         if (navSurface != null)
//         {
//             navSurface.isUpdating = true;
//             Debug.Log("Updating NavMeshSurface");
//             navSurface.surface.BuildNavMesh();
//             yield return new WaitForSeconds(1.5f);
//             navSurface.isUpdating = false;
//         }
//     }
// // #endif
// }
