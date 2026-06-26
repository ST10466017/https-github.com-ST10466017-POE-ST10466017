using System;
using System.Collections.Generic;

namespace ZoroChatbot.POE
{
    public class SentimentAnalyzer
    {
        private Dictionary<string, string> sentimentKeywords;

        public SentimentAnalyzer()
        {
            InitializeKeywords();
        }

        private void InitializeKeywords()
        {
            sentimentKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Worried keywords
                ["worried"] = "worried",
                ["concerned"] = "worried",
                ["anxious"] = "worried",
                ["nervous"] = "worried",
                ["scared"] = "worried",
                ["afraid"] = "worried",
                ["unsafe"] = "worried",

                // Curious keywords
                ["curious"] = "curious",
                ["interested"] = "curious",
                ["want"] = "curious",
                ["learn"] = "curious",
                ["tell"] = "curious",
                ["explain"] = "curious",

                // Frustrated keywords
                ["frustrated"] = "frustrated",
                ["annoyed"] = "frustrated",
                ["confused"] = "frustrated",
                ["not working"] = "frustrated",
                ["doesn't work"] = "frustrated",
                ["useless"] = "frustrated"
            };
        }

        public string DetectSentiment(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "neutral";

            foreach (var kvp in sentimentKeywords)
            {
                if (userInput.ToLower().Contains(kvp.Key.ToLower()))
                {
                    return kvp.Value;
                }
            }
            return "neutral";
        }

        public string GetEncouragingResponse(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe. Would you like to hear them?";
                case "frustrated":
                    return "I understand your frustration. Let me try to explain this in a simpler way. Feel free to ask questions if anything is unclear.";
                case "curious":
                    return "That's great! Learning about cybersecurity is important. I'd be happy to share more information with you.";
                default:
                    return null;
            }
        }
    }
}