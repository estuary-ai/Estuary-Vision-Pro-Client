using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Mangrove
{
    [RequireComponent(typeof(AudioSource))]
    public class BotVoice : MonoBehaviour
    {
        // public AudioClip activateClip;
        // public AudioClip terminateClip;
        private Queue<IncomingAudioPacket> _incomingAudioPacketQueue;
        [SerializeField] private AudioSource audioSource;
        public bool _isSpeaking;
        private int _offset;
        private int clipPlayedTimestamp;

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
                    throw new Exception("BotVoice could not find the BotVoice object.");
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!audioSource) audioSource = GetComponent<AudioSource>();
            _incomingAudioPacketQueue = new Queue<IncomingAudioPacket>();
        }



        void Update()
        {
            if (audioSource.isPlaying == false)
            {
                lock (_incomingAudioPacketQueue)
                {
                    if (_incomingAudioPacketQueue.Count > 0)
                    {
                        IncomingAudioPacket packet = _incomingAudioPacketQueue.Dequeue();
                        Debug.Log("Packet timestamp: " + packet.timestamp + " _offset: " + _offset + " audioSource.time: " + audioSource.time);
                        if (packet.timestamp < _offset + audioSource.time)
                        {
                            Debug.Log("Packet skipping!");
                            audioSource.Stop();
                            return;
                        }
                        AudioClip convertedClip = CreateAudioClipFromAudioPacket(packet);
                        if(convertedClip == null) return;
                        _isSpeaking = true;
                        clipPlayedTimestamp = packet.timestamp;
                        audioSource.PlayOneShot(convertedClip);
                    }
                    else
                    {
                        _isSpeaking = false;
                    }
                }
            }
            else
            {
                if (clipPlayedTimestamp < _offset)
                {
                    audioSource.Stop();
                    _isSpeaking = false;
                }
            }
        }


        public bool IsSpeaking()
        {
            return _isSpeaking;
        }

        public void Interrupt(int timestamp)
        {
            _offset = timestamp;
            // Debug.Log("Setting _offset to " + _offset);
        }

        public AudioClip CreateAudioClipFromAudioPacket(IncomingAudioPacket packet)
        {
            Debug.Log($"Playing SENVA voice.. recieved {packet.ToString()}");

            float[] samples;
            if (packet.sampleWidth == 2)
            {
                samples = BotVoice.Convert16BitByteArrayToAudioClipData(packet.bytes);
            }
            else
            {
                if (packet.sampleWidth != 4)
                {
                    Debug.LogError($"Unsupported sample width: {packet.sampleWidth}. Expected 2 or 4.");
                    return null;
                }

                samples = new float[packet.bytes.Length / 4]; //size of a float is 4 bytes
                Buffer.BlockCopy(packet.bytes, 0, samples, 0, packet.bytes.Length);
            }

            AudioClip clip = AudioClip.Create("ClipName", samples.Length, packet.numChannels, packet.sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // public void PlayActivationSound()
        // {
        //     Log("Playing activation sound");
        //     // lock(_incomingAudioPacketQueue) {
        //     // _incomingAudioPacketQueue.Enqueue(activateClip);
        //     // }
        //     if(audioSource.isPlaying) audioSource.Stop();
        //     audioSource.PlayOneShot(activateClip);
        // }

        // public void PlayTerminationSound()
        // {
        //     Log("Playing termination sound");
        //     // lock(_incomingAudioPacketQueue) {
        //     // Log("Queueing termination sound 12");
        //     // _incomingAudioPacketQueue.Enqueue(terminateClip);
        //     // }
        //     if(audioSource.isPlaying) audioSource.Stop();
        //     audioSource.PlayOneShot(terminateClip);
        // }


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


        //     // lock(_incomingAudioPacketQueue) {
        //         _incomingAudioPacketQueue.Enqueue(clip);
        //     // }
        // }


        // public static AudioClip ConvertBytesToAudioClip(byte[] source, string NAME = "bot-voice")
        // {
        //     string filepath = Application.persistentDataPath + "/__tmp__.wav";
        //     Debug.Log("Saving audio to be played to " + filepath);
        //     File.WriteAllBytes(filepath, source);
        //     WWW www = new WWW("file://" + filepath);
        //     while (!www.isDone)
        //     {
        //     }
        //
        //     AudioClip audioClip = www.GetAudioClip(false, false, AudioType.WAV);
        //     // delete the file
        //     // File.Delete(filepath);
        //     audioClip.name = NAME;
        //     return audioClip;
        // }

        public void EnqueueAudioPacket(IncomingAudioPacket packet)
        {
            _incomingAudioPacketQueue.Enqueue(packet);
            // StartCoroutine(playAfterSilence(clip));
        }

        // IEnumerator playAfterSilence(AudioClip clip)
        // {
        //     while (audioSource.isPlaying)
        //     {
        //         yield return null;
        //     }
        //     audioSource.PlayOneShot(clip);
        // }

        // public void PlayAudioBytes(byte[] audioBytes, string audioName="bot-voice") {
        //     AudioClip clip = ConvertBytesToAudioClip(audioBytes, audioName);
        //
        //     // lock(_incomingAudioPacketQueue) {
        //         _incomingAudioPacketQueue.Enqueue(clip);
        //     // }
        // }

        private static void Log(string message)
        {
            Debug.Log("BotVoice.cs :: " + message);
        }

    }
}