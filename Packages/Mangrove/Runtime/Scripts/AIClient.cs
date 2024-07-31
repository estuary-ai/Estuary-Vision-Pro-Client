using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;

namespace Mangrove
{
    public class AIClient : MonoBehaviour
    {
        // Supported Events and Requests defined at the bottom
        public bool isDebugging = false;

        private SocketIOUnity socket;

        // private bool isAwake = false;
        private BotResponseHandler botResponseHandler;
        private MicController micController;
        private BotVoice botVoice;

        [field: SerializeField] public string api = "ws://localhost:4000";

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
                    throw new Exception("AIClient could not find the AIClient object instance.");
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
            {
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

            micController.Init();
        }

        // public void RequestBotToRead(DataReadingInfo dataReadingInfo)
        // {
        //     Debug.Log("Request to Read Data " + dataReadingInfo.ToString());
        //     Emit(REQUESTS.TTS_READ, dataReadingInfo);
        // }
        //
        // public void RequestBotToRead(string stringToRead)
        // {
        //     Debug.Log("Request to Read String " + stringToRead);
        //     Emit(REQUESTS.TTS_READ, stringToRead);
        // }

        public void StartClient(String retrieved_api)
        {
            bool firstTime = true;
            Debug.Log("Start Client @ " + retrieved_api);
            if (socket != null)
            {
                DisconnectSocket();
                firstTime = false;
            }

            api = retrieved_api;
            socket = new SocketIOUnity(api, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    { "token", "UNITY" },
                    { "AutoUpgrade", "true" }
                },
                EIO = 4,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            socket.JsonSerializer = new NewtonsoftJsonSerializer();
            SetupOnSocketEvents();
            if (firstTime)
            {
                Debug.Log("Connecting for the first time...");
            }
            else
            {
                Debug.Log("Connecting after kill...");
            }

            socket.Connect();
            DontDestroyOnLoad(transform.root.gameObject);
        }

        IEnumerator StartMicDelay()
        {
            Debug.Log("Waiting for bot voice to finish speaking or socket to connect");
            while (botVoice.IsSpeaking() || !socket.Connected)
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
                Emit("trial", "test");
                UnityThread.executeInLateUpdate(
                    () => StartCoroutine(StartMicDelay())
                );
            };
            socket.OnPing += (sender, e) => { Debug.Log("Ping"); };
            socket.OnPong += (sender, e) => { Debug.Log("Pong: " + e.TotalMilliseconds); };
            socket.OnDisconnected += (sender, e) =>
            {
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
                // if (isAwake) {
                //     StopCommandTransmission();
                // }
            };
            socket.OnReconnectError += (sender, e) =>
            {
                Debug.Log($"Event Reconnect Error with {api} due to {e} from {sender}");
                // if (isAwake) {
                //     StopCommandTransmission();
                // }
            };
            /////////////////////

            // socket.On(EVENTS.WAKE_UP, (result) =>
            // {
            //     Debug.Log("WAKE UP EVENT RECEIVED");
            //     // result.GetValue<IncomingDataPacket<short>>();
            //     // Debug.Log($"WakeUp: {result}");
            //     if(appRef && appRef.navManager) appRef.navManager.CallPuppy();
            //     Debug.Log("before StartCommandTransmission");
            //     StartCommandTransmission();
            //     Debug.Log("after StartCommandTransmission");
            // });

            socket.On(EVENTS.STT_RES, (result) =>
            {
                Debug.Log($"stt-response: {result}");
                // StopCommandTransmission();
            });
            socket.On(EVENTS.BOT_RES, (result) =>
            {
                Debug.Log($"bot-response: {result}");
                botResponseHandler.Handle(result.GetValue<BotResponse>());
            });
            socket.On(EVENTS.BOT_VOICE, (result) =>
            {
                Debug.Log($"bot-voice: {result}");
                IncomingAudioPacket packet = result.GetValue<IncomingAudioPacket>();

                // TODO verify and debug audioBytes / test FixedUpdate
                UnityThread.executeInUpdate(() =>
                {
                    // botVoice.PlayAudioBytes(audioPacket.InComingBytes[0]);
                    botVoice.ProcessAndPlayAudioBytes(packet);
                });

            });
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
                try
                {
                    socket.Disconnect();
                    socket.Dispose();
                    // await socket.DisconnectAsync();
                    // socket = null;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                // isAwake = false;
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
            if (socket == null)
            {
                Debug.Log("Socket is null. Can't send audio packet");
                return;
            }

            Emit(REQUESTS.AUDIO_STREAM, audioPacket);
        }

        private void Emit(string eventName, object data)
        {
            if (socket.Connected)
            {
                Thread thread = new Thread(() => { socket.Emit(eventName, data); });
                thread.Start();
            }
        }

        // private void CamController_OnVideoFrameCaptured(VideoFrame videoFrame)
        // {
        //     Emit(REQUESTS.VIDEO_STREAM, videoFrame);
        // }

        class EVENTS
        {
            public static readonly string BOT_RES = "bot_response";
            public static readonly string BOT_VOICE = "bot_voice";
            public static readonly string STT_RES = "stt_response";
        }

        class REQUESTS
        {
            public static readonly string AUDIO_STREAM = "stream_audio";
            public static readonly string VIDEO_STREAM = "stream_video";
        }
    }
}