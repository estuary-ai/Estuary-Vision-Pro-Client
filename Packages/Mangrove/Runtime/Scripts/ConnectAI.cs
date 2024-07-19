using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Uses the Unity plugin
// Connection utilities for the AI server connection panel!

// ws:// ___URI_HERE___ :4000
// saves to json in Application.persistentDataPath "AIConnectionIPs.json"

namespace Mangrove
{
    public class ConnectAI : MonoBehaviour
    {

        public bool manualConnect;

        public string manualURI;

        // [SerializeField] private SENVAConnScript connManager;
        private AIClient habibi;

        private TouchScreenKeyboard keyboard;


        // [SerializeField] private UIManagerDropdown dropdown;
        [SerializeField] private TMP_Dropdown dropdown;

        // [SerializeField] private MRTKTMPInputField inputField;
        // [SerializeField] private UIManagerInputField inputField;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text connection_status;

        // [SerializeField] private TMP_Text suits_panel_connection_status;

        // [SerializeField] private ApplicationReferences appRef;

        [Header("Loading Indicator")]
        // [SerializeField] private ProgressIndicatorOrbsRotator loadingIndicator;
        [SerializeField]
        private GameObject loadingOrbs;

        private bool _isConnecting;

        private bool firstConnect;

        private string inputURI;
        private string fullInputURI;
        private string path_URIJson;

        // array of URIs for reading/writing URIs to json
        private string[] uris;

        // list of URIs for modifying list of URIs
        private List<string> list_URIs;

        // list of URIs that have been input this session
        private List<string> current_session_URIs;

        private bool fileExists;

        // whether json needs update
        private bool needUpdate;

        // whether or not inputField is being edited
        private bool editing;


        // Start is called before the first frame update
        void Start()
        {
            habibi = AIClient.Instance;
            TouchScreenKeyboard.hideInput = false;

            if (manualConnect)
            {
                // do nothing
                DoConnect();
            }
            else
            {
                manualURI = "--manual connect is disabled--";

                connection_status.text = "Not Connected";

                path_URIJson = Path.Combine(Application.persistentDataPath, "AIConnectionIPs" + ".json");

                current_session_URIs = new List<string>();

                // populate dropdown with JSON URIs
                dropdown.ClearOptions();

                uris = Array.Empty<string>();
                fileExists = File.Exists(path_URIJson);
                needUpdate = true;
                list_URIs = new List<string>();
                if (fileExists)
                {
                    // retrieve the current list of URIs from the JSON
                    string[] linesURIs = File.ReadAllLines(path_URIJson);
                    // turn into single string to convert to array of objects
                    string fullLines = "";
                    foreach (string line in linesURIs)
                    {
                        fullLines += line;
                    }

                    uris = JsonHelper.FromJson<string>(fullLines);

                    // change from array to List
                    if (uris != null)
                    {
                        foreach (string uri in uris)
                        {
                            list_URIs.Add(uri);
                        }
                    }

                    // // for easily erasing inputfield
                    // list_URIs.Add("");

                    // get most recent first
                    list_URIs.Reverse();
                    dropdown.AddOptions(list_URIs);

                    // set default keyboard input to previous IP address that was stored
                    if (list_URIs.Count > 0)
                    {
                        inputField.text = list_URIs[0];
                    }
                }

                // TODO: here is attempt at fixing invisible caret issue:
                StartCoroutine(ReloadInputField());
            }



        }

        // Update is called once per frame
        void Update()
        {
            if (habibi.IsConnected())
            {
                DisableLoadingIndicator();

                // TODO: Uncomment below after merging with unity branch
                if (firstConnect == false)
                {
                    // appRef.notificationManager.HabibiNotif(1);
                }

                firstConnect = true;
                // TODO: Uncomment above after merging with unity branch
                // connection_status.text = "Connected";
                // suits_panel_connection_status.text = "AI Connected";
                // suits_panel_connection_status.color = new Color32(255,255,255, 255);

                // add to json
                // var exists = current_session_URIs.Contains(inputURI);
                // if(exists == false)
                // {
                //     current_session_URIs.Add(inputURI);
                //     dropdown.AddOptions(list_URIs);
                //     UpdateJson();
                // }
            }
            else
            {
                // TODO: Uncomment below after merging with unity branch
                firstConnect = false;
                // TODO: Uncomment above after merging with unity branch
                // if(!_isConnecting) connection_status.text = "Not Connected";
                // suits_panel_connection_status.text = "AI Not Connected";
                // suits_panel_connection_status.color = new Color32(255,0,0,255);

            }

            if (editing)
            {
                inputField.caretPosition = inputField.text.Length;
                editing = false;
            }
        }

        // call when click connect/disconnect button
        public void ButtonPressed()
        {
            Debug.Log("button pressed");
            if (habibi.IsConnected())
            {
                // already connected
                // pressing button again should disconnect
                Debug.Log("Disconnecting AI websocket...");
                DoDisconnect();
            }
            else
            {
                // not connected
                // pressing button should connect
                Debug.Log("Connecting AI websocket...");
                if (inputField.text.Contains(" "))
                {
                    inputField.text = inputField.text.Replace(" ", "");
                }

                DoConnect();
            }
        }

        public void DoConnect()
        {
            // start loading animation
            EnableLoadingIndicator();
            StartCoroutine(ConnectionTimeout());


            // TODO: Uncomment below after merging with unity branch
            // appRef.notificationManager.HabibiNotif(0);
            // TODO: Uncomment above after merging with unity branch
            if (manualConnect)
            {
                // get the constant input uri
                if (manualURI == "" || manualURI == null)
                {
                    Debug.LogError("Manual URI is empty or null");
                }

                inputURI = manualURI;
            }
            else
            {
                inputURI = inputField.text;
            }


            // TODO: delete?
            // var exists = current_session_URIs.Contains(inputURI);
            // if (exists == false)
            // {
            //     current_session_URIs.Add(inputURI);
            //     UpdateJson();
            // }

            // Call SENVAConnScript.cs Connect()
            // format "ws://" + inputURI + ":4000"
            fullInputURI = "ws://";
            fullInputURI += inputURI;
            fullInputURI += ":4000";
            habibi.StartClient(fullInputURI);
        }

        public void DoDisconnect()
        {
            Debug.Log("Attempting to call habibi disconnect...");
            habibi.DisconnectSocket();
        }


        // call for dropdown
        public void ChooseOption()
        {
            // change the input text
            inputURI = dropdown.captionText.text;
            inputField.text = inputURI;
            Debug.Log("Chose option: " + inputURI);
        }

        public void RemoveOption()
        {
            // caveat: can't remove option while connected to the uri
            // because Update() will continuously add it

            // also remove from json....
            list_URIs.RemoveAll(p => p == dropdown.options[dropdown.value].text);
            current_session_URIs.RemoveAll(p => p == dropdown.options[dropdown.value].text);

            dropdown.ClearOptions();

            dropdown.AddOptions(list_URIs);

            // change from List to array
            string[] newURIs = list_URIs.ToArray();

            // write updated json to file
            string json = JsonHelper.ToJson<string>(newURIs);
            System.IO.File.WriteAllText(path_URIJson, json);
        }


        public void UpdateJson()
        {
            inputURI = inputField.text;

            if (uris != null)
            {
                // check if current URI exists in json already
                for (int i = 0; i < uris.Length; i++)
                {
                    if (uris[i].Contains(inputURI))
                    {
                        needUpdate = false;
                    }
                }

            }

            if (needUpdate)
            {
                Debug.Log("updating json for IP address:" + inputURI);
                // also adding to dropdown immediately
                List<string> tempList = new List<string>();
                tempList.Add(inputURI);
                dropdown.AddOptions(tempList);

                // save IP address in json for later
                list_URIs.Add(inputURI);

                // change from List to array
                string[] newURIs = list_URIs.ToArray();

                // write updated json to file
                string json = JsonHelper.ToJson<string>(newURIs);
                System.IO.File.WriteAllText(path_URIJson, json);
            }
        }

        private void EnableLoadingIndicator()
        {
            // loadingIndicator.OpenAsync();
            // loadingOrbs.SetActive(true);
            _isConnecting = true;
            if (manualConnect == false)
            {
                connection_status.text = "Connecting...";
            }

        }

        private void DisableLoadingIndicator()
        {
            // loadingIndicator.CloseAsync();
            // loadingOrbs.SetActive(false);
            _isConnecting = false;
        }

        // called by inputfield
        public void StartEditText()
        {
            editing = true;
        }


        // wait one frame, then reload input field
        IEnumerator ReloadInputField()
        {
            yield return 0;

            inputField.selectionColor = Color.gray;
            inputField.enabled = false;
            inputField.enabled = true;
        }

        IEnumerator ConnectionTimeout()
        {
            yield return new WaitForSeconds(3f);
            DisableLoadingIndicator();
        }
    }
}