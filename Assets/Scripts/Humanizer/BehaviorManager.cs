using System.Text.RegularExpressions;
using UnityEngine;
using Mangrove;

public class BehaviorManager : MonoBehaviour
{
    [SerializeField] private ApplicationReferences appRef;
    private void OnEnable()
    {
        // Subscribe to bot responses
        Mangrove.AIClient.OnBotResponseReceived += HandleAction;
    }

    private void OnDisable()
    {
        Mangrove.AIClient.OnBotResponseReceived -= HandleAction;
    }

    private void HandleAction(string msg)
    {
        // Debug.Log("Got bot response: " + msg);
        var command = ParseCommand(msg);
        Debug.Log("Parsed command: " + command);
        switch("Sit Down")
        {
            case "Sit Down":
                appRef.navManager.MoveAgentToNearestSeatPlane();
                break;
            case "Follow User":
                appRef.navManager.SetFollowMode(true);
                break;
            case "Stop Following User":
                appRef.navManager.SetFollowMode(false);
                break;
        }
    }

    private string ParseCommand(string msg)
    {
        // Use regex to parse command ie. ["sit down"]
        string pattern = @"\[\s*\""(.*?)\""\s*\]";
        MatchCollection matches = Regex.Matches(msg, pattern);
        string command = "";
        foreach (Match match in matches)
        {
            // Extract the content inside ["..."]
            command = match.Groups[1].Value;
        }
        return command;
    }
}
