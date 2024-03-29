using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isSittingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");
        isSittingHash = Animator.StringToHash("isSitting");
    }

    // Update is called once per frame
    void Update()
    {   
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSitting = animator.GetBool(isSittingHash);

        //Start & Stop Walking Trigger
        bool WalkingTrigger = Input.GetKey("w");

        //Start & Stop Sitting Trigger
        bool SittingTrigger = Input.GetKey("left shift");

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
    }
}
