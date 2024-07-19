using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Mangrove
{
    public class BotResponseHandler : MonoBehaviour
    {
        public bool Disable = false;

//     [SerializeField]
//     private ApplicationReferences applicationReferences;
//     private UIManager uiManager;
//     private MapInteraction mapInteraction;
//     private SENVAConnScript senvaConnScript;

//     private Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.EyeTrackerInteractionManager eyeTrackerInteractionManager;

//     private AnomalyPopup anomalyPopup;

//     private GameObject _camera;
//     private GameObject miniMap;
//     private GameObject photoCapturing;
//     private bool isSampleTagging = false;
//     private string sampleTaggingId;
//     private bool isRetried = false;
        private Queue<BotResponse> responsesQueue;
        private AIClient clientController;

//     private delegate void HandleDelegate(string action, string[] additionalInfo);
//     private Dictionary<string, HandleDelegate> handleDict;

//     private List<string> possibleTargets;


        /// <summary>
        /// Singleton access
        /// </summary>
        private static BotResponseHandler _instance;

        public static BotResponseHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("BotResponseHandler could not find the BotResponseHandler object.");
                }

                return _instance;
            }
        }

        public BotResponseHandler()
        {

            // initHandleDict();
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            responsesQueue = new Queue<BotResponse>();
            if (Disable) return;
            // uiManager = applicationReferences.uiManager;
            // mapInteraction = applicationReferences.mapInteraction;
            // senvaConnScript = applicationReferences.SENVAConnScript;
            // eyeTrackerInteractionManager = applicationReferences.eyeTrackerInteractionManager;
            // if (applicationReferences.anomalyPopup != null) {
            //     anomalyPopup = applicationReferences.anomalyPopup.GetComponent<AnomalyPopup>();
            // }
        }

        void Start()
        {
            if (Disable) return;
            // _camera = GameObject.Find("Main Camera");
            // miniMap = GameObject.Find("Minimap");
            clientController = AIClient.Instance;
            // isSampleTagging = false;
            // uiManager = applicationReferences.uiManager;
            // mapInteraction = applicationReferences.mapInteraction;
        }

//     private void DispatchCommand(string key, Action callback)
//     {
//         UnityThread.executeInLateUpdate(() =>
//         {
//             Debug.Log("Executing " + key);
//             Debug.Log("This is executed from the main thread on Late Update: running " + key);
//             callback();
//         });
//     }

//     void FixedUpdate() {
//         if(responsesQueue.Count > 0) {
//             lock(responsesQueue) {
//                 Debug.Log("Testing");
//                 _Handle(responsesQueue.Dequeue());
//             }
//         }
//     }

//     void initHandleDict() {
//         handleDict = new Dictionary<string, HandleDelegate>();
//         handleDict.Add("error", new HandleDelegate(handleError));
//         handleDict.Add("panel", new HandleDelegate(handlePanel));
//         // handleDict.Add("spectrometry", new HandleDelegate(handleSpectrometry));
//         handleDict.Add("vitals", new HandleDelegate(handleVitals));
//         handleDict.Add("suit", new HandleDelegate(handleSuit));
//         handleDict.Add("anomaly", new HandleDelegate(handleAnomalies));
//         // handleDict.Add("warnings", new HandleDelegate(handleWarnings));
//         // handleDict.Add("cautions", new HandleDelegate(handleCautions));
//         handleDict.Add("longrangenavigation", new HandleDelegate(handleLongNaviation));
//         handleDict.Add("waypoint", new HandleDelegate(handleWaypoint));
//         handleDict.Add("time", new HandleDelegate(handleTime));
//         handleDict.Add("shortrangenavigation", new HandleDelegate(handleShortNavigation));
//         handleDict.Add("breadcrumb", new HandleDelegate(handleBreadCrumb));
//         handleDict.Add("route", new HandleDelegate(handleRoute));
//         handleDict.Add("rover", new HandleDelegate(handleRover));
//         handleDict.Add("egress", new HandleDelegate(handleEgress));
//         handleDict.Add("tss", new HandleDelegate(handleTSS));
//         handleDict.Add("ip", new HandleDelegate(handleIP));
//         handleDict.Add("menumanager", new HandleDelegate(handleMenuManager));
//         handleDict.Add("aimanager", new HandleDelegate(handleAIManager));
//         handleDict.Add("uia", new HandleDelegate(handleUIA));
//         possibleTargets = new List<String>(handleDict.Keys);
//     }

//     // Source: https://stackoverflow.com/questions/6944056/compare-string-similarity/6944095
//     public static int GetDamerauLevenshteinDistance(string s, string t)
//     {
//         float threshold = 0.4f;
//         if (string.IsNullOrEmpty(s))
//         {
//             throw new ArgumentNullException(s, "String Cannot Be Null Or Empty");
//         }

//         if (string.IsNullOrEmpty(t))
//         {
//             throw new ArgumentNullException(t, "String Cannot Be Null Or Empty");
//         }

//         int n = s.Length; // length of s
//         int m = t.Length; // length of t

//         if (n == 0)
//         {
//             return m;
//         }

//         if (m == 0)
//         {
//             return n;
//         }

//         int[] p = new int[n + 1]; //'previous' cost array, horizontally
//         int[] d = new int[n + 1]; // cost array, horizontally

//         // indexes into strings s and t
//         int i; // iterates through s
//         int j; // iterates through t

//         for (i = 0; i <= n; i++)
//         {
//             p[i] = i;
//         }

//         for (j = 1; j <= m; j++)
//         {
//             char tJ = t[j - 1]; // jth character of t
//             d[0] = j;

//             for (i = 1; i <= n; i++)
//             {
//                 int cost = s[i - 1] == tJ ? 0 : 1; // cost
//                 // minimum of cell to the left+1, to the top+1, diagonally left and up +cost
//                 d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
//             }

//             // copy current distance counts to 'previous row' distance counts
//             int[] dPlaceholder = p; //placeholder to assist in swapping p and d
//             p = d;
//             d = dPlaceholder;
//         }

//         // our last action in the above loop was to switch d and p, so p now
//         // actually has the most recent cost counts
//         int finalDistance = p[n];
//         if (finalDistance / s.Length > threshold) {
//             return int.MaxValue;
//         }
//         return finalDistance;
//     }

//     public void handleError(string action, string[] additionalInfo) {
//         clientController.RequestBotToRead(additionalInfo[0]);
//     }

        public void Handle(BotResponse botResponse)
        {
            lock (responsesQueue)
            {
                Debug.Log("Enqueuing a botResponse");
                Debug.Log(botResponse);
                // BotResponse botResponse = JsonConvert.DeserializeObject<BotResponse>(botResponseJson);
                Debug.Log($"object BotResponse {botResponse.ToString()}");
                responsesQueue.Enqueue(botResponse);
            }
        }

//     // public void Handle(BotResponse botResponse) {
//     //     _Handle(botResponse);
//     // }

//     private void _Handle(BotResponse botResponse)
//     {
//         // if (isSampleTagging)
//         // {
//         //     if(botResponse.text.Length > 0)
//         //     {
//         //         // notesPanelOutput.text =  botResponse.text[0];
//         //     }
//         // }
//         // Debug.Log("_Handele being called***************************");
//         // Debug.Log(botResponse);
//         // Debug.Log(handleDict);

//         foreach (Command command in botResponse.commands)
//         {
//             List<string> strList = new List<string>();
//             foreach (string s in command.additionalInfo) {
//                 strList.Add(s.ToLower());
//             }
//             string target = command.target.ToLower();
//             // Debug.Log(target);
//             if (target == "") {
//                 continue;
//             } else if (!handleDict.ContainsKey(target)) {
//                 Debug.Log("NOt in dict");
//                 target = possibleTargets.OrderByDescending(obj => GetDamerauLevenshteinDistance(obj, command.target.ToLower())).First();
//                 if (GetDamerauLevenshteinDistance(target, command.target.ToLower()) == int.MaxValue) {
//                     Debug.Log("command target not found, target being " + target);
//                     clientController.RequestBotToRead("Command not found.");
//                     return;
//                 }
//             }
//             // Debug.Log("target is " + target + " action is " + command.action.ToLower());
//             handleDict[target](command.action.ToLower(), strList.ToArray());
//         }
//     }

//     private void handlePanel(string action, string[] additionalInfo) {
//         string panelName = additionalInfo[0];
//         // Debug.Log("The input panelName is " + panelName);
//         List<string> possibleList = new List<string>{"vitals", "suit", "spectrometry", "tss", "ai", "eye", "all"};
//         panelName = possibleList.Contains(panelName)? panelName : possibleList.OrderByDescending(obj => GetDamerauLevenshteinDistance(obj, additionalInfo[0])).Last();
//         // Debug.Log("The output panelName is " + panelName);
//         // Debug.Log("The distance is " + GetDamerauLevenshteinDistance(panelName, additionalInfo[0].ToLower()));
//         if (GetDamerauLevenshteinDistance(panelName, additionalInfo[0].ToLower()) == int.MaxValue) {
//             clientController.RequestBotToRead("Command not found.");
//             return;
//         }
//         switch(panelName) {
//             case "vitals":
//                 panelName = "DisplayVitals";
//                 break;
//             case "suit":
//                 panelName = "DisplaySuitData";
//                 break;
//             case "spectrometry":
//                 panelName = "DisplaySpectrometry";
//                 break;
//             case "tss":
//                 panelName = "TSSConnectionPanel";
//                 break;
//             case "ai":
//                 panelName = "AIConnectionPanel";
//                 break;
//             case "eye":
//                 handleEyePanel(action);
//                 return;
//             case "all":
//                 closeAllPanels();
//                 return;
//             default:
//                 break;
//         }
//         Debug.Log(panelName);
//         // Debug.Log(applicationReferences);
//         // Debug.Log(uiManager);
//         switch(action) {
//             case "open":
//                 // openPanel(panelName);
//                 uiManager.OpenPanel(panelName);
//                 break;
//             case "close":
//                 uiManager.ClosePanel(panelName);
//                 // closePanel(panelName);
//                 break;
//             case "follow":
//                 break;
//             case "unfollow":
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void closeAllPanels() {
//         uiManager.ClosePanel("DisplayVitals");
//         uiManager.ClosePanel("DisplaySuitData");
//         uiManager.ClosePanel("spectrometryPanel");
//         uiManager.ClosePanel("TSSConnectionPanel");
//         uiManager.ClosePanel("AIConnectionPanel");
//         anomalyPopup.hidePopup();
//     }

//     private void handleEyePanel(string action) {
//         Debug.Log("action " + action + "is called on eye panel");
//         switch(action) {
//             case "follow":
//                 eyeTrackerInteractionManager.ToggleLookedAtPanelFollow(true);
//                 break;
//             case "unfollow":
//                 eyeTrackerInteractionManager.ToggleLookedAtPanelFollow(false);
//                 break;
//             case "close":
//                 eyeTrackerInteractionManager.CloseLookedAtPanel();
//                 Debug.Log("Close eye target panel");
//                 // clientController.RequestBotToRead("eye target panel close");
//                 break;
//             default:
//                 break;
//         }
//     }


//     // private void handleSpectrometry(string action, string[] additionalInfo) {
//     //     switch(action) {
//     //         case "next_graph":
//     //             //TODO
//     //             break;
//     //         default:
//     //             break;
//     //     }
//     // }

//     private void handleVitals(string action, string[] additionalInfo) {
//         switch(action) {
//             case "read":
//                 string value = "";
//                 string unit = "";
//                 string feature = additionalInfo[0];
//                 Debug.Log(feature);
//                 switch(feature) {
//                     case "battery":
//                         value = senvaConnScript.curMsg.simulationStates.battery_capacity.ToString();
//                         unit = "percent";
//                         break;
//                     case "battery_time":
//                         value = convertSecToTime(Int32.Parse(senvaConnScript.curMsg.simulationStates.battery_time_left));
//                         break;
//                     case "primary_o2":
//                         value = senvaConnScript.curMsg.simulationStates.primary_oxygen.ToString();
//                         unit = "percent";
//                         break;
//                     case "secondary_o2":
//                         value = senvaConnScript.curMsg.simulationStates.secondary_oxygen.ToString();
//                         unit = "percent";
//                         break;
//                     case "o2_pressure":
//                         value = senvaConnScript.curMsg.simulationStates.o2_pressure.ToString();
//                         unit = "Pounds per square inch";
//                         break;
//                     case "o2_time":
//                         value = convertSecToTime(Int32.Parse(senvaConnScript.curMsg.simulationStates.o2_time_left));
//                         break;
//                     case "heart_rate":
//                         value = senvaConnScript.curMsg.simulationStates.heart_rate.ToString();
//                         unit = "beats per minute";
//                         break;
//                     case "suit_pressure":
//                         value = senvaConnScript.curMsg.simulationStates.suit_pressure.ToString();
//                         unit = "Pounds per square inch";
//                         break;
//                     default:
//                         feature = "data is not avaliable";
//                         break;
//                 }
//                 Debug.Log("The feature is " + feature + " the value is " + value);
//                 clientController.RequestBotToRead(feature.Replace("_", " ") + " is " + value + " " + unit);
//                 // if (unit == "" ){
//                 //     Debug.Log("Is calling to read");
//                 //     TransferRequestBotToRead(feature, new string[]{value.ToString()}, new string[]{});
//                 // } else {
//                     // TransferRequestBotToRead(feature, new string[]{value.ToString()}, new string[]{unit.ToString()});
//                 // }
//                 break;
//             default:
//                 break;
//         }
//     }

//     private string convertSecToTime(int sec) {
//         int hours = sec / 3600;
//         sec %= 3600;
//         int minutes = sec / 60;
//         sec %= 60;
//         if (hours != 0) {
//             return hours + " hours " + minutes + " minutes " + sec + " seconds";
//         } else if (minutes != 0) {
//             return minutes + " minutes " + sec + " seconds";
//         }
//         return sec + " seconds";
//     }

//     private void handleSuit(string action, string[] additionalInfo) {
//         switch(action) {
//             case "read":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleAnomalies(string action, string[] additionalInfo) {
//         switch(action) {
//             case "clear":
//                 anomalyPopup.dismissPopup();
//                 break;
//             case "show":
//                 anomalyPopup.expandPopup();
//                 break;
//             case "hide":
//                 anomalyPopup.hidePopup();
//                 break;
//             default:
//                 break;
//         }
//     }

//     // private void handleWarnings(string action, string[] additionalInfo) {
//     //     switch(action) {
//     //         case "clear":
//     //             //TODO
//     //             break;
//     //         case "show":
//     //             //TODO
//     //             break;
//     //         case "hide":
//     //             //TODO
//     //             break;
//     //         default:
//     //             break;
//     //     }
//     // }

//     // private void handleCautions(string action, string[] additionalInfo) {
//     //     switch(action) {
//     //         case "clear":
//     //             //TODO
//     //             break;
//     //         case "show":
//     //             //TODO
//     //             break;
//     //         case "hide":
//     //             //TODO
//     //             break;
//     //         default:
//     //             break;
//     //     }
//     // }

//     private void handleLongNaviation(string action, string[] additionalInfo) {
//         switch(action) {
//             case "open":
//                 uiManager.OpenMap();
//                 break;
//             case "close":
//                 uiManager.CloseMap();
//                 break;
//             case "clear":
//                 //TODO
//                 break;
//             case "show":
//                 uiManager.OpenMap();
//                 break;
//             case "hide":
//                 uiManager.CloseMap();
//                 break;
//             case "mode":
//                 string mode = translateMode(additionalInfo[0]);
//                 mapInteraction.SetMode(mode);
//                 break;
//             case "north":
//                 mapInteraction.ManualCompassCalibration();
//                 break;
//             case "compass":
//                 mapInteraction.CompassCalibration();
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleWaypoint(string action, string[] additionalInfo) {
//         string targetWaypoint = "";
//         string mode = "";
//         if (additionalInfo.Length >= 1) {
//             targetWaypoint = additionalInfo[0];
//         }
//         if (additionalInfo.Length == 2) {
//             mode = translateMode(additionalInfo[1]);
//         }
//         switch(action) {
//             case "remove":
//                 switch(targetWaypoint) {
//                     case "eye":
//                         eyeTrackerInteractionManager.RemoveLookedAtWaypoint();
//                         break;
//                     case "last":
//                         mapInteraction.UndoWaypoint();
//                         break;
//                     case "all":
//                         mapInteraction.ClearAllWaypoints();
//                         break;
//                     default:
//                         break;
//                 }
//                 break;
//             case "show":
//                 //TODO
//                 mapInteraction.ShowWaypoint();
//                 break;
//             case "hide":
//                 //TODO
//                 mapInteraction.HideWaypoint();
//                 break;
//             case "add":
//                 switch(targetWaypoint) {
//                     case "poi":
//                         putWaypoint("eye", "Area of Interest");
//                         break;
//                     case "hazard":
//                         putWaypoint("eye", "Hazard");
//                         break;
//                     default:
//                         putWaypoint(targetWaypoint, mode);
//                         break;
//                 }
//                 break;
//             default:
//                 break;
//         }

//     }

//     private string translateMode(string mode) {
//         switch(mode) {
//             case "move":
//                 return "Move";
//             case "nav":
//                 return "Pathfind";
//             case "hazard":
//                 return "Hazard";
//             case "rover":
//                 return "Dispatch Rover";
//             case "poi":
//                 return "Area of Interest";
//             default:
//                 return mode;
//         }
//     }

//     private void putWaypoint(string target, string mode) {
//         switch(target) {
//             case "eye":
//                 mapInteraction.SetMode(mode);
//                 if (mode != "move") {
//                     eyeTrackerInteractionManager.PlaceWaypointGivenGazePoint();
//                 }
//                 break;
//             case "input":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void clearWaypoint(string target) {
//         switch(target) {
//             case "eye":
//                 eyeTrackerInteractionManager.RemoveLookedAtWaypoint();
//                 break;
//             case "all":
//                 mapInteraction.ClearAllWaypoints();
//                 break;
//             default:
//                 break;
//         }
//     }


//     private void handleTime(string action, string[] additionalInfo) {
//         switch(action) {
//             case "open":
//                 //TODO
//                 putWaypoint("eye", "Pathfind");
//                 // string value = convertSecToTime((int) senvaConnScript.curMsg.simulationStates.time);
//                 // string value = senvaConnScript.curMsg.simulationStates.time.ToString();
//                 // clientController.RequestBotToRead("Time is " + value);
//                 break;
//             default:
//                 break;
//         }
//     }



//     private void handleShortNavigation(string action, string[] additionalInfo) {
//         switch(action) {
//             // case "open":
//             //     //TODO
//             //     uiManager.OpenPanel("SettingsPanel");
//             //     break;
//             // case "close":
//             //     //TODO
//             //     uiManager.ClosePanel("SettingsPanel");
//             //     break;
//             case "enable":
//                 //TODO
//                 uiManager.TurnShortRangeNavigationOn();
//                 break;
//             case "disable":
//                 //TODO
//                 uiManager.TurnShortRangeNavigationOff();
//                 break;
//             default:
//                 break;
//         }
//     }


//     private void handleBreadCrumb(string action, string[] additionalInfo) {
//         // [home, cloesest, A~J]
//         string targetLocation = "";
//         if (additionalInfo.Length >= 1) {
//             targetLocation = additionalInfo[0];
//         }

//         switch(action) {
//             case "navigate":
//                 //TODO
//                 senvaConnScript.roverComm.voiceRover(targetLocation.ToUpper().ToCharArray()[0]);
//                 break;
//             case "show":
//                 mapInteraction.ShowBreadcrumbTrailUCS();
//                 break;
//             case "hide":
//                 mapInteraction.HideBreadcrumbTrailUCS();
//                 break;
//             case "clear":
//                 mapInteraction.ClearBreadcrumbTrail();
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleRoute(string action, string[] additionalInfo) {

//         string targetLocation = "";
//         if (additionalInfo.Length >= 1) {
//             targetLocation = additionalInfo[0];
//         }
//         switch(action) {
//             case "show":
//                 mapInteraction.ShowBreadcrumbTrailUCSMCS();
//                 break;
//             case "hide":
//                 mapInteraction.HideBreadcrumbTrailUCSMCS();
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleRover(string action, string[] additionalInfo) {
//         // [A~J]
//         string targetLocation = "";
//         if (additionalInfo.Length >= 1) {
//             targetLocation = additionalInfo[0];
//         }
//         switch(action) {
//             case "move":
//                 //TODO
//                 switch(targetLocation) {
//                     default:
//                         break;
//                 }
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleEgress(string action, string[] additionalInfo) {
//         string targetInformation = "";
//         if (additionalInfo.Length >= 1) {
//             targetInformation = additionalInfo[0];
//         }
//         switch(action) {
//             case "open":
//                 //TODO
//                 break;
//             case "close":
//                 //TODO
//                 break;
//             case "read":
//                 //TODO
//                 switch (targetInformation) {
//                     case "step number":
//                         break;
//                     case "step":
//                         break;
//                     case "next step":
//                         break;
//                     case "complete":
//                         break;
//                     default:
//                         break;
//                 }
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleTSS(string action, string[] additionalInfo) {
//         switch(action) {
//             case "open":
//                 //TODO
//                 break;
//             case "read":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleAI(string action, string[] additionalInfo) {
//         switch(action) {
//             case "open":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleIP(string action, string[] additionalInfo) {
//         switch(action) {
//             case "select":
//                 //TODO
//                 break;
//             case "create":
//                 //TODO
//                 break;
//             case "connect":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleMenuManager(string action, string[] additionalInfo) {
//         switch(action) {
//             case "swap hand":
//                 //TODO
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleAIManager(string action, string[] additionalInfo) {
//         string targetFeature = "";
//         if (additionalInfo.Length >= 1) {
//             targetFeature = additionalInfo[0];
//         }
//         switch(action) {
//             case "increase":
//                 //TODO
//                 switch(targetFeature) {
//                     case "brightness":
//                         break;
//                     default:
//                         break;
//                 }
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void handleUIA(string action, string[] additionalInfo) {
//         //TODO
//         string stepID = "";
//         string target = "";
//         if (additionalInfo.Length >= 1) {
//             stepID = additionalInfo[0];
//         } else if (additionalInfo.Length >= 2) {
//             target = additionalInfo[1];
//         }
//         switch(action) {
//             case "open":
//             case "start":
//                 // clientController.TextInitiator_OnTextCommmandIntiated("Next step");
//                 // StartCoroutine(StartEgressInstructions());
//                 break;
//             case "current_step_number":
//             {
//                 //TODO

//                 // string stepID = additionalInfo[0];
//                 break;
//             }
//             case "current_step":
//             {
//                 //TODO
//                 // string stepID = additionalInfo[0];
//                 // string target = additionalInfo[1];
//                 // switch(target) {
//                 //     //TODO Talk to Prashant then implement
//                 //     case "brightness":
//                 //         break;
//                 //     default:
//                 //         break;
//                 // }
//                 break;
//             }
//             case "next_step":
//             {
//                 break;
//             }
//             case "exit":
//                 //TODO
//                 break;
//             case "confirm_completion":
//                 //TODO
//                 // switch(targetFeature) {
//                 //     case "brightness":
//                 //         break;
//                 //     default:
//                 //         break;
//                 // }
//                 break;
//             default:
//                 break;
//         }
//     }

//     private void TransferRequestBotToRead(string feature, string[] values, string[] units) {
//         Debug.Log("read data by");
//         clientController.RequestBotToRead(new DataReadingInfo(){
//             feature = feature, values = values, units = units
//         });
//     }

//     public bool isInSampleTaggingScenario() {
//         return isSampleTagging;
//     }
    }

    public class DataReadingInfo
    {
        public string feature;
        public string[] values;
        public string[] units;

        public override string ToString()
        {
            string result = "";
            result += "feature: " + feature + " ";
            foreach (string v in values)
            {
                result += "value: " + v + " ";
            }

            foreach (string u in units)
            {
                result += "unit: " + u + " ";
            }

            return result;
        }
    }
}