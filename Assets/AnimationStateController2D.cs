using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController2D : MonoBehaviour
{
    Animator animator;
    int isSittingHash;
    int isGreetingHash;
    public float acceleration = 2.0f;
    public float deceleration = 1.0f;
    float motionZ = 0.0f;
    float motionX = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        isSittingHash = Animator.StringToHash("isSitting");
        isGreetingHash = Animator.StringToHash("isGreeting");
    }

    // Update is called once per frame
    void Update()
    {
        bool isSitting = animator.GetBool(isSittingHash);
        bool isGreeting = animator.GetBool(isGreetingHash);

        //Start & Stop Walking Trigger
        //walk directions triggers
        bool WalkingForwardTrigger = Input.GetKey("w");
        bool WalkingLeftTrigger = Input.GetKey("a");
        bool WalkingRightTrigger = Input.GetKey("d");

        //Start & Stop Sitting Trigger
        bool SittingTrigger = Input.GetKey("left shift");

        //Start & Stop Greeting Trigger
        bool GreetingTrigger = Input.GetKey("e");

        //Walking Forward Condition Trigger
        if (WalkingForwardTrigger && motionZ < 0.5f)
        {
            motionZ += Time.deltaTime*acceleration;
        }
        //Walking Right Condition Trigger
        if (WalkingRightTrigger && motionX < 0.5f)
        {
            motionX += Time.deltaTime*acceleration;
        }
        //Walking Left Condition Trigger
        if (WalkingLeftTrigger && motionX > -0.5f)
        {
            motionX -= Time.deltaTime * acceleration;
        }
        //Slowing down
        if (!WalkingForwardTrigger && motionZ > 0.0f)
        {
            motionZ -= Time.deltaTime * deceleration;
        }
        if (!WalkingForwardTrigger && motionZ < 0.0f)
        {
            motionZ = 0.0f;
        }
        if (!WalkingLeftTrigger && motionX < 0.0f)
        {
            motionX += Time.deltaTime * deceleration;
        }
        if (!WalkingRightTrigger && motionX > 0.0f)
        {
            motionX -= Time.deltaTime * deceleration;
        }
        if (!WalkingLeftTrigger && !WalkingRightTrigger && motionX != 0.0f && (motionX > -0.05f && motionX < 0.05f))
        {
            motionX = 0.0f;
        }

        //Start Sitting Condition Trigger
        if (!isSitting && SittingTrigger)
        {
            animator.SetBool(isSittingHash, true);
        }
        //Stop Sitting Condition Trigger
        if (isSitting && !SittingTrigger)
        {
            animator.SetBool(isSittingHash, false);
        }
        //Start Greeting Condition Trigger
        if (!isGreeting && GreetingTrigger)
        {
            animator.SetBool(isGreetingHash, true);
        }
        //Stop Greeting Condition Trigger
        if (isGreeting && !SittingTrigger)
        {
            animator.SetBool(isGreetingHash, false);
        }
        animator.SetFloat("MotionZ", motionZ);
        animator.SetFloat("MotionX", motionX);
    }
}
