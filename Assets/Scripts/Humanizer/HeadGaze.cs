using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGaze : MonoBehaviour
{
    public Camera head;
    // Start is called before the first frame update
    void Start()
    {
        head = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // visualize the raycast
        Debug.DrawRay(head.transform.position, head.transform.forward * 1000, Color.red);
        
        // Raycast from the camera to the center of the screen
        Ray ray = head.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("I'm looking at " + hit.transform.name);
            // Do something with the object that was hit by the raycast.
        }
    }
}
