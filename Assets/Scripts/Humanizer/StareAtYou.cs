using System;
using System.Collections;
using UnityEngine;

class StareAtYou : MonoBehaviour {

    [SerializeField] private Animator agentAnimator;
    private static readonly int IsSpeaking = Animator.StringToHash("isSpeaking");
    private GameObject head;
    private Transform target;

    public void Start() {
        head = this.gameObject.transform.Find("body_head").gameObject;
        // target = Camera.main.transform;
        target = GameObject.Find("debug_box").transform;
    }

    public void LateUpdate() {
        if (agentAnimator.GetBool(IsSpeaking) == true) {
            // // turn head to face the camera
            // Debug.Log("Turning head to stare at you");
            // head.transform.LookAt(target);
            agentAnimator.SetLookAtPosition(target.position);
        }
    }
}