using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testzuko : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // upon key press
        if (Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Animator>().SetBool("isWalking", !GetComponent<Animator>().GetBool("isWalking"));
        } 
        if (GetComponent<Animator>().GetBool("isWalking")) {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            // set new condition in animator
            GetComponent<Animator>().SetBool("isSitting", !GetComponent<Animator>().GetBool("isWalking"));
        }
    }
}
