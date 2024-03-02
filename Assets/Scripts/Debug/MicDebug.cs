using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;

public class MicDebug : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private SpatialUIButton m_Button;
    [SerializeField] private MicController micController;
    void OnEnable()
    {
        m_Button.WasPressed += WasPressed;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RequestMicAccess());
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
}
