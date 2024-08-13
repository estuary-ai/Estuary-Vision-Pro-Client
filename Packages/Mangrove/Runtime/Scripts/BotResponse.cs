using System;
using Newtonsoft.Json.Linq;

namespace Mangrove
{
    [Serializable]
    public class BotResponse
    {
        public string[] text;
        public Command[] commands;


        public override string ToString()
        {
            string result = "Text : " + string.Join(" ", text);
            foreach (Command c in commands)
            {
                result += "\n" + c.ToString();
            }

            return result;
        }

        public string ToJson()
        {
            //TODO
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return jsonString;
        }

    }

    public class Command
    {
        // Debating on whehter the fields should be an enum
        public string target; // ShortRangeNavigation, MiniMap, Map
        public string action; // Toggle, query, set, select,

        public string[]
            additionalInfo; // If object == "pin" && action == "set", additionalInfo should contain "blue || red" or "big || small"

        public Command(string t, string a, string[] addInfo)
        {
            target = t;
            action = a;
            additionalInfo = addInfo;
        }

        public override string ToString()
        {
            string result = "Target " + target + "\n";
            result += "Action " + action + "\n";
            result += "Additional Info ";
            foreach (string info in additionalInfo)
            {
                result += info + " ";
            }

            result += "\n";
            return result;
        }
    }
}