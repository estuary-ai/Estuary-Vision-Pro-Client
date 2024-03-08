using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// #if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// #endif

[Serializable]
public class NavigationSurface
{
    public ARPlane plane;
    public NavMeshSurface surface;
    public bool isUpdating = false;
}

public class NavManager : MonoBehaviour
{
    public void FixedUpdate()
    {

    }
}
