using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBallBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * speed;
    }
}
