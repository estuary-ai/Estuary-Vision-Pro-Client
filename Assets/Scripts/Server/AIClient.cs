using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;

// TODO: get SocketIOClient package from here: https://github.com/itisnajim/SocketIOUnity


public class AIClient : MonoBehaviour
{
    private const string DEBUG_PREFIX = "AIClient.cs :: ";
    // Supported Events and Requests defined at the bottom
    public bool isDebugging = false;
    private SocketIOUnity socket;
    private bool isAwake = false;
    private BotResponseHandler botResponseHandler;
    private MicController micController;
    private BotVoice botVoice;

    [field:SerializeField]
    public string api = "ws://localhost:4000";

    /// <summary>
    /// Singleton access
    /// </summary>
    private static AIClient _instance;
    public static AIClient Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception ("AIClient could not find the AIClient object instance.");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null) {
            _instance = this;
			DontDestroyOnLoad(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        botResponseHandler = BotResponseHandler.Instance;
        micController = MicController.Instance;
        // camController = CamController.Instance;
        botVoice = BotVoice.Instance;

        micController.OnAudioFrameCaptured += MicController_OnAudioPacketCaptured;
        // camController.OnVideoFrameCaptured += CamController_OnVideoFrameCaptured;

        if (isDebugging)
        {
            // UnityThread.executeInLateUpdate(() =>
            // {
                Debug.Log("DigitalAssistant is in DEBUG mode!");
                StartClient(api);
            // });
        }
    }

    public void RequestBotToRead(DataReadingInfo dataReadingInfo)
    {
        Debug.Log("Request to Read Data " + dataReadingInfo.ToString());
        // lock(socket) {
        socket.Emit(REQUESTS.TTS_READ, dataReadingInfo);
        // }
    }

    public void RequestBotToRead(string stringToRead)
    {
        Debug.Log("Request to Read String " + stringToRead);
        // lock(socket) {
        socket.Emit(REQUESTS.TTS_READ, stringToRead);
        // }
    }



    public void StartClient(String retrieved_api)
    {
        bool firstTime = true;
        Debug.Log("Start Client @ " + retrieved_api);
        if (socket != null) {
            DisconnectSocket();
            firstTime = false;
        }

        api = retrieved_api;
        socket = new SocketIOUnity(api, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", "UNITY" },
                {"AutoUpgrade", "true"}
            },
            EIO=4,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.JsonSerializer = new NewtonsoftJsonSerializer();
        SetupOnSocketEvents();
        if (firstTime) {
            Debug.Log("Connecting for the first time...");
        } else {
            Debug.Log("Connecting after kill...");
        }
        socket.Connect();
        DontDestroyOnLoad(transform.root.gameObject);
    }

    IEnumerator StartMicDelay()
    {
        Debug.Log("Waiting for bot voice to finish speaking or socket to connect");
        while(botVoice.IsSpeaking() || !socket.Connected)
        {
            yield return new WaitForSeconds(0.5f);
            // if (botVoice.IsSpeaking()) {
            //     Debug.Log("Bot voice is still speaking...");
            // }
            // if (!socket.Connected) {
            //     Debug.Log("Socket is not connected...");
            // }
        }
        Debug.Log("Starting Mic from AIClient.cs");
        micController.AllowStream();
    }

    private void SetupOnSocketEvents()
    {
        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected");
            socket.Emit("trial", "test");
            UnityThread.executeInLateUpdate(
                () => StartCoroutine(StartMicDelay())
            );
        };
        socket.OnPing += (sender, e) => { Debug.Log("Ping"); };
        socket.OnPong += (sender, e) => { Debug.Log("Pong: " + e.TotalMilliseconds); };
        socket.OnDisconnected += (sender, e) => {
            Debug.Log("Disconnected.. Trying reconnecting..");
            micController.BlockStream();
            UnityThread.executeInLateUpdate(() => socket.Connect());
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Attempting to reconnect -> {e} with {api}");
        };
        socket.OnReconnectFailed += (sender, e) =>
        {
            Debug.Log($"Event Reconnect Failed with {api} due to {e} from {sender}");
            if (isAwake) {
                StopCommandTransmission();
            }
        };
        socket.OnReconnectError += (sender, e) =>
        {
            Debug.Log($"Event Reconnect Error with {api} due to {e} from {sender}");
            if (isAwake) {
                StopCommandTransmission();
            }
        };
        /////////////////////

        socket.On(EVENTS.WAKE_UP, (result) =>
        {
            Debug.Log($"WakeUp: {result}");
            StartCommandTransmission();
        });
        socket.On(EVENTS.STT_RES, (result) =>
        {
            Debug.Log($"stt-response: {result}");
            StopCommandTransmission();
        });
        socket.On(EVENTS.BOT_RES, (result) =>
        {
            Debug.Log($"bot-response: {result}");
            botResponseHandler.Handle(result.GetValue<BotResponse>());
        });
        socket.On(EVENTS.BOT_VOICE, (audioBytesObject) =>
        {
            Debug.Log(
                $"{EVENTS.BOT_VOICE} -- Playing SENVA voice.. " +
                $"recieved {audioBytesObject.InComingBytes.ToArray().Length} lists of bytes"
            );
            // TODO verify and debug audioBytes / test FixedUpdate
            UnityThread.executeInUpdate(() =>
            {
                botVoice.PlayAudioBytes(audioBytesObject.InComingBytes[0]);
            });
        });
    }

    void StartCommandTransmission()
    {
        Debug.Log("Start Transmission");
        UnityThread.executeInUpdate(() => {
            botVoice.PlayActivationSound();
            // micController.RefreshTheMic();
        });
        isAwake = true;
        Debug.Log("set isAwake");

    }

    void StopCommandTransmission()
    {
        Debug.Log("Stop Transmission -- Unset is Awake");
        UnityThread.executeInUpdate(() => {
            // socket.Emit(REQUESTS.RESET_AUDIO_STREAM);
            botVoice.PlayTerminationSound();
        });
        isAwake = false;
    }

    void OnDisable()
    {
        Debug.Log("On Disable");
        micController.OnAudioFrameCaptured -= MicController_OnAudioPacketCaptured;
    }

    public void DisconnectSocket()
    {
        if (socket != null)
        {
            Debug.Log("Destroying socket");
            try {
                socket.Disconnect();
                socket.Dispose();
                // await socket.DisconnectAsync();
                // socket = null;
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
            isAwake = false;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("On Application Quit");
        DisconnectSocket();
    }

    public bool IsConnected()
    {
        return socket != null && socket.Connected;
    }

    // void OnApplicationFocus(bool paused)
    // {
    //     handleSocketReceiving = !paused;
    // }
    // void OnApplicationPause(bool paused)
    // {
    //     handleSocketReceiving = !paused;
    // }

    private void MicController_OnAudioPacketCaptured(AudioPacket audioPacket)
    {
        if (socket == null) {
            Debug.Log("Socket is null. Can't send audio packet");
            return;
        }

        if (socket.Connected)
        {
            // Debug.Log("Sending Audio Packet now!");
            Thread thread = new Thread(() => {
                socket.Emit(REQUESTS.AUDIO_STREAM, audioPacket);
            });
            thread.Start();
        }
    }

    // private void CamController_OnVideoFrameCaptured(VideoFrame videoFrame)
    // {
    //     if(socket.Connected)
    //     {
    //         Thread thread = new Thread(() => {
    //             socket.Emit(REQUESTS.VIDEO_STREAM, videoFrame);
    //         });
    //         thread.Start();
    //     }
    // }

    public void TextInitiator_OnTextCommmandIntiated(string commandString)
    {
        if(socket.Connected)
        {
            Debug.Log($"Debug onTextCommand: {commandString}");
            commandString = commandString.Replace("\u200B", "");
            Thread thread = new Thread(() => {
                socket.Emit(REQUESTS.TEXT_STREAM, commandString);
            });
            thread.Start();
        }
    }

    public void Debugger_OnTrialMsg(string debugStatement)
    {
        if(socket.Connected)
        {
            debugStatement = debugStatement.Replace("\u200B", "");
            Thread thread = new Thread(() => {
                socket.Emit("trial", debugStatement);
            });
            thread.Start();
        }
    }

    public void IssueTaggingSample()
    {
        if (socket.Connected)
            socket.Emit(REQUESTS.TEXT_STREAM, "tag a sample");
    }

    class EVENTS {
        public static readonly string BOT_RES = "bot_response";
        public static readonly string BOT_VOICE = "bot_voice";
        public static readonly string STT_RES = "stt_response";
        public static readonly string WAKE_UP = "wake_up";
    }

    class REQUESTS {
        public static readonly string AUDIO_STREAM = "stream_audio";
        public static readonly string TTS_READ = "read_tts";
        public static readonly string TEXT_STREAM = "stream_text";
        public static readonly string VIDEO_STREAM = "stream_video";
        public static readonly string TELEMETRY_STREAM = "update_world_state";
    }
}