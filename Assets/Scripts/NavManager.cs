using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// #if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// #endif

public class NavManager : MonoBehaviour
{
    [SerializeField] private TMP_Text debugFloorText;
// #if UNITY_INCLUDE_ARFOUNDATION
    public void Update()
    {
        int floorCount = 0;
        ARPlane[] planes = FindObjectsOfType<ARPlane>();
        foreach (var p in planes)
        {
            if (p.classification == PlaneClassification.Floor)
            {
                floorCount++;
                // Debug.Log("Floor found: " + floorCount);
            }
        }
        if (debugFloorText != null) debugFloorText.text = "Floor Count: " + floorCount;
    }
// #endif
}
