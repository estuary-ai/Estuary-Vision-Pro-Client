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
        string command = ParseCommand(msg);
        Debug.Log($"Parsed command: '{command}'");

        if (string.IsNullOrEmpty(command)) {
            Debug.Log("No valid parsed command found in message");
            return;
        }

        if (command == "Sit Down")
        {
            appRef.navManager.MoveAgentToNearestSeatPlane();
        }
        else if (command == "Follow User")
        {
            Debug.Log("[Parsed] following user now");
            appRef.navManager.SetFollowMode(true);
        }
        else if (command == "Stop Following User")
        {
            appRef.navManager.SetFollowMode(false);
        }
        else if (command == "Summon Karakasa")
        {
            appRef.yokaiManager.SummonKarakasa();
        }
        else if (command == "Summon Zashiki")
        {

        }
        else
        {
            Debug.Log($"Parsed unrecognized command: '{command}'");
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
