using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

// #if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// #endif


public class NavManager : MonoBehaviour
{
    // Dictionary of the GameObject meshes and their NavMeshSurfaces that are attached as components
    public Dictionary<GameObject, NavMeshSurface> navigableObjects = new();
    private bool isUpdating = false;
    private string DEBUG_TAG = "[NAV MANAGER]: ";
    [SerializeField] private Transform groundDetectorOrigin;
    [SerializeField] public NavMeshAgent navMeshAgent;
    [SerializeField] private Animator agentAnimator;
    public GameObject navNode;
    private static readonly int IsWalking = Animator.StringToHash("isRunning");

    public void FixedUpdate()
    {
        // // find all game objects that use the Navigable tag
        //     // mesh will be generated from prefab with "Navigable" tag
        // navigableObjects = GameObject.FindGameObjectsWithTag("Navigable");

        // perform a raycast from groundDectorOrigin to the ground and add navigable
        RaycastHit hit;
        if (Physics.Raycast(groundDetectorOrigin.position, Vector3.down, out hit, 10f))
        {
            if (hit.collider.gameObject.CompareTag("Navigable"))
            {
                Debug.Log(DEBUG_TAG + "Navigable object found: " + hit.collider.gameObject.name);
                AddNavigable(hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log(DEBUG_TAG + "No navigable object found");
        }

        if (navigableObjects.Count <= 0)
        {
            Debug.Log(DEBUG_TAG + "No navigable objects have been added.");
        }
        
        foreach (KeyValuePair<GameObject, NavMeshSurface> pair in navigableObjects)
        {
            if (isUpdating == false)
            {
                StartCoroutine(UpdateNavMesh(pair.Value));
            }
        }

        if (navNode != null) {
            // if the char has reached the destination node
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                    if (navMeshAgent.gameObject.GetComponent<Animation>() != null) {
                        navMeshAgent.gameObject.GetComponent<Animation>().Play("Idle");
                    }
                }
                agentAnimator.SetBool(IsWalking, false);
            }
            else
            {
                agentAnimator.SetBool(IsWalking, true);
            }
        }

    }

    /**
     * Add a navigable object to the list of navigable objects
     * Used in InputDebug.cs for adding meshes that are tapped (user selects the floor meshes)
     */
    public void AddNavigable(GameObject n)
    {
        if (navigableObjects.ContainsKey(n) == false)
        {
            Debug.Log(DEBUG_TAG + "Adding navigable object: " + n.ToString());
            NavMeshSurface navMeshSurface = n.AddComponent<NavMeshSurface>();
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navigableObjects.Add(n, navMeshSurface);
        } else {
            Debug.Log(DEBUG_TAG + "Navigable object already exists: " + n.ToString());
        }
    }

    private IEnumerator UpdateNavMesh(NavMeshSurface navMeshSurface)
    {
        if (navMeshSurface != null)
        {
            isUpdating = true;
            Debug.Log(DEBUG_TAG + "Updating NavMeshSurface");
            navMeshSurface.BuildNavMesh();
            yield return new WaitForSeconds(1f);
            isUpdating = false;
        } else {
            Debug.Log(DEBUG_TAG + "NavMeshSurface is null");
        }
    }

    public void MakeNavigate(GameObject destNode) {

        if (destNode != null)
        {
            Debug.Log(DEBUG_TAG + "Navigating to: " + destNode.transform.position);
            // move the user to the selected position
            navMeshAgent.SetDestination(destNode.transform.position);
            navNode = destNode;

            // animations
            agentAnimator.SetBool(IsWalking, true);

            if (navMeshAgent.gameObject.GetComponent<Animation>() != null) {
                navMeshAgent.gameObject.GetComponent<Animation>().Play("Walking");
            }
        }
        else
        {
            Debug.Log(DEBUG_TAG + "No navigation position found");
        }
    }
}
