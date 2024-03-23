using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BotVoice : MonoBehaviour
{
    public AudioClip activateClip;
    public AudioClip terminateClip;
    private Queue<AudioClip> _clipQueue;
    private AudioSource _audioSource;
    private bool _isSpeaking;

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
        _audioSource = GetComponent<AudioSource>();
        _clipQueue = new Queue<AudioClip>();
    }

    void Update()
    {
        if (_audioSource.isPlaying == false) {
            lock(_clipQueue) {
                if (_clipQueue.Count > 0) {
                    _audioSource.clip = _clipQueue.Dequeue();
                    _isSpeaking = true;
                    _audioSource.Play();
                }
                else
                {
                    _isSpeaking = false;
                }
            }
        } else {
            Debug.Log("Audio source is playing!!!");
        }
    }

    public bool IsSpeaking()
    {
        return _isSpeaking;
    }

    public void PlayActivationSound() {
        Log("Queueing activation sound");
        // lock(_clipQueue) {
            _clipQueue.Enqueue(activateClip);
        // }
    }

    public void PlayTerminationSound() {
        // Log("Queueing termination sound 11");
        // lock(_clipQueue) {
            // Log("Queueing termination sound 12");
            _clipQueue.Enqueue(terminateClip);
        // }
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

    // public void PlayAudioBytes(byte[] audioBytes, string audioName="bot-voice") {
    //     // Its a 16 bit audio file, so there will be two bytes (8 bits * 2 = 16 bit)
    //     // per float, so we need half the floats as there are bits
    //     float[] audioFloat = Convert16BitByteArrayToAudioClipData(audioBytes);
    //     AudioClip clip = AudioClip.Create(
    //         audioName,
    //         audioBytes.Length,
    //         1, // 1 channel
    //         26000, // 22.05 is 352 kbps at 16 bit
    //         false
    //     );
    //     clip.SetData(audioFloat, 0);


    //     // lock(_clipQueue) {
    //         _clipQueue.Enqueue(clip);
    //     // }
    // }

    
    public static AudioClip ConvertBytesToAudioClip(byte[] source, string NAME="bot-voice")
    {
        string filepath = Application.persistentDataPath + "/__tmp__.mp3";
        Debug.Log("Saving audio to be played to " + filepath);
        File.WriteAllBytes(filepath, source);
        WWW www = new WWW("file://" + filepath);
        while (!www.isDone) { }
        return www.GetAudioClip(false, false, AudioType.MPEG);
    }
        
        

    public void PlayAudioBytes(byte[] audioBytes, string audioName="bot-voice") {
        AudioClip clip = ConvertBytesToAudioClip(audioBytes, audioName);

        // lock(_clipQueue) {
            _clipQueue.Enqueue(clip);
        // }
    }

    private static void Log(string message)
    {
        Debug.Log("BotVoice.cs :: " + message);
    }
}