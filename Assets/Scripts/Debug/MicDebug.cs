using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;
using UnityEngine.UI;

public class MicDebug : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private SpatialUIButton m_Button;
    [SerializeField] private MicController micController;
    // autoStart mic init after 5 seconds
    [SerializeField] private bool autoStart = false;
    void OnEnable()
    {
        m_Button.WasPressed += WasPressed;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RequestMicAccess());
        if (autoStart)
        {
            StartCoroutine(AutoStart());
        }

    }

    private IEnumerator RequestMicAccess()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    void WasPressed(string buttonText, MeshRenderer meshrenderer)
    {
        Debug.Log("Mic Debug Button Pressed");
        micController.Init();
    }

    private IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(5);
        micController.Init();
    }
}
