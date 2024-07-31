using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isSittingHash;
    int isGreetingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");
        isSittingHash = Animator.StringToHash("isSitting");
        isGreetingHash = Animator.StringToHash("isGreeting");
    }

    // Update is called once per frame
    void Update()
    {   
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSitting = animator.GetBool(isSittingHash);
        bool isGreeting = animator.GetBool(isGreetingHash);

        //Start & Stop Walking Trigger
        bool WalkingTrigger = Input.GetKey("w");

        //Start & Stop Sitting Trigger
        bool SittingTrigger = Input.GetKey("left shift");

        //Start & Stop Greeting Trigger
        bool GreetingTrigger = Input.GetKey("e");

        //Start Walking Condition Trigger
        if (!isWalking && WalkingTrigger)
        {
            animator.SetBool(isWalkingHash, true);
        }
        //Stop Walking Condition Trigger
        if (isWalking && !WalkingTrigger)
        {
            animator.SetBool(isWalkingHash, false);
        }
        //Start Sitting Condition Trigger
        if (!isSitting && !isWalking && SittingTrigger)
        {
            animator.SetBool(isSittingHash, true);
        }
        //Stop Sitting Condition Trigger
        if (isSitting && !isWalking && !SittingTrigger)
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

    }
}
