using System.Collections;
using UnityEngine;

class SpeakingIdle : MonoBehaviour {
    private Mangrove.BotVoice botVoice;
    [SerializeField] private Animator agentAnimator;
    private static readonly int IsSpeaking = Animator.StringToHash("isSpeaking");

    public void Start() {
        botVoice = Mangrove.BotVoice.Instance;
    }

    public void Update() {
        // Check if BotVoice is speaking
        if (botVoice.IsSpeaking()) {
            // Only change if not already speaking
            // if (agentAnimator.GetBool(IsSpeaking) == false) {
                Debug.Log("Starting speaking animation");
                agentAnimator.SetBool(IsSpeaking, true);
            // }
        } else {
            // Only change if not already silent
            // if (agentAnimator.GetBool(IsSpeaking) == true) {
                Debug.Log("Stopping speaking animation");
                agentAnimator.SetBool(IsSpeaking, false);
            // }
        }
    }
}