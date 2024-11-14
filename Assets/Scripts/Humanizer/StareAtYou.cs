using System;
using System.Collections;
using UnityEngine;

class StareAtYou : MonoBehaviour {

    [SerializeField] private Animator agentAnimator;
    private static readonly int IsSpeaking = Animator.StringToHash("isSpeaking");
    private GameObject stareTransformer;
    private GameObject nodTransformer;
    private Transform target;
    [SerializeField] private float nodSpeed = 9.0f;
    [SerializeField] private float nodAmplitude = 5.0f;

    public void Start() {
        stareTransformer = GameObject.Find("StareTransformer").gameObject;
        nodTransformer = GameObject.Find("NodTransformer").gameObject;
        target = Camera.main.transform;
        // target = GameObject.Find("debug_box").transform;
    }

    public void LateUpdate() {
        if (agentAnimator.GetBool(IsSpeaking)) {
            float stepSize = 1.7f * Time.deltaTime;
            float bodyTurnSpeed = 0.1f;
            
            // turn head to face the camera
            // if not already facing the camera
            Vector3 targetStareDir = target.position - stareTransformer.transform.position;
            if (Vector3.Angle(stareTransformer.transform.rotation.eulerAngles, targetStareDir) > 5.0f) {
                Debug.Log("Not facing camera! Turning head to face the camera");
                
                Vector3 newStareDir = Vector3.RotateTowards(stareTransformer.transform.forward, targetStareDir, stepSize, 0.0f);
                Vector3 newBillboardDir = Vector3.RotateTowards(transform.forward, targetStareDir, bodyTurnSpeed, 0.0f);
                transform.rotation = Quaternion.LookRotation(newBillboardDir);
                // set x and z of transform rotation to 0
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                stareTransformer.transform.rotation = Quaternion.LookRotation(newStareDir);
            }

            float nodAmount = Mathf.Sin(Time.time * nodSpeed) * nodAmplitude;
            nodTransformer.transform.localRotation = Quaternion.Euler(nodAmount, 0, 0);
        }
        else
        {
            // reset nodding
            Debug.Log("Reset nod animation");
            stareTransformer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

}