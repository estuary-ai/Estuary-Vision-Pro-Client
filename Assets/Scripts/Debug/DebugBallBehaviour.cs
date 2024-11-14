using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBallBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private Rigidbody rb;

    private Transform camTrans;
    // Start is called before the first frame update
    void Start()
    {
        camTrans = GameObject.Find("Main Camera").transform;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = camTrans.forward * speed;
    }
}
