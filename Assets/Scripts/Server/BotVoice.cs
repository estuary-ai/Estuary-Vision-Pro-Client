using System;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(AudioSource))];
public class BotVoice : MonoBehaviour
{
    public AudioClip activateClip;
    public AudioClip terminateClip;
    private Queue<AudioClip> clipQueue;
    private AudioSource audioSource; 
    private String activationFilepath = $"{Application.streamingAssetsPath}/DigitalAssistant/assistant_initiate.wav";
    private String terminationFilepath = $"{Application.streamingAssetsPath}/DigitalAssistant/assistant_terminate.wav";
    private bool isSpeaking;

    /// <summary>
    /// Singleton access
    /// </summary>
    private static BotVoice _instance;
    public static BotVoice Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception ("BotVoice could not find the BotVoice object.");
            }
            return _instance;
        }
    }

    void Awake() {
        if (_instance == null)
        {
            _instance = this;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clipQueue = new Queue<AudioClip>();
        // activateClip = UnityWebRequestMultimedia.GetAudioClip(activationFilepath, AudioType.WAV).;
        // terminateClip = new WWW(terminationFilepath).GetAudioClip();
    }

    void Update()
    {
        if (audioSource.isPlaying == false) {
            if (clipQueue.Count > 0) {
                lock(clipQueue) {
                    audioSource.clip = clipQueue.Dequeue();
                }
                Log("Playing clip");
                audioSource.Play();
                isSpeaking = true;
            } else {
                isSpeaking = false;
            }
        }
    }

    public bool IsSpeaking()
    {
        return isSpeaking;
    }

    public void PlayActivationSound() {
        Log("Queueing activation sound");
        lock(clipQueue) {
            clipQueue.Enqueue(activateClip);
        }
    }

    public void PlayTerminationSound() {
        // Log("Queueing termination sound 11");
        lock(clipQueue) {
            // Log("Queueing termination sound 12");
            clipQueue.Enqueue(terminateClip);
        }
    }

    public static float[] Convert16BitByteArrayToAudioClipData(byte[] source)
    {
        int x = sizeof(Int16); 
        int convertedSize = source.Length / x;
        float[] data = new float[convertedSize];
        Int16 maxValue = Int16.MaxValue;

        for (int i = 0; i < convertedSize; i++)
        {
            int offset = i * x;
            data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
        }
        return data;
    }

    public void PlayAudioBytes(byte[] audioBytes, string audioName="bot-voice") {
        lock(clipQueue) {
            // Its a 16 bit audio file, so there will be two bytes (8 bits * 2 = 16 bit)
            // per float, so we need half the floats as there are bits
            float[] audioFloat = Convert16BitByteArrayToAudioClipData(audioBytes);
            AudioClip clip = AudioClip.Create(
                audioName, 
                audioBytes.Length, 
                1, // 1 channel
                26000, // 22.05 is 352 kbps at 16 bit
                false
            );
            clip.SetData(audioFloat, 0);
            clipQueue.Enqueue(clip);
        }
    }

    private void Log(string message)
    {
        Debug.Log("BotVoice.cs :: " + message);
    }
}