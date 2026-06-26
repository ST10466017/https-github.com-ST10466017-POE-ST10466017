using System;
using System.Collections.Generic;

namespace ZoroChatbot.Console
{
    public class ResponseManager
    {
        private Dictionary<string, List<string>> keywordResponses;
        private Dictionary<string, string> generalResponses;
        private Random random;
        private string userName;
        private string userInterest;

        public ResponseManager()
        {
            random = new Random();
            InitializeResponses();
            InitializeGeneralResponses();
        }

        private void InitializeResponses()
        {
            keywordResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "🔑 Make sure to use strong, unique passwords for each account. Consider using a password manager!",
                    "🔑 Never reuse passwords across different websites. Each account should have its own unique password.",
                    "🔑 Your passwords should be at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols.",
                    "🔑 Avoid using personal information like your birthday or pet's name in passwords."
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                    "🎣 Always check the sender's email address carefully - it might look legitimate but have small differences.",
                    "🎣 Never click on suspicious links in emails or messages. Hover over links to see where they really lead.",
                    "🎣 If you receive a suspicious email, report it to your IT department or the legitimate organisation directly."
                },
                ["privacy"] = new List<string>
                {
                    "🔒 Review your privacy settings regularly on all your accounts and social media platforms.",
                    "🔒 Be mindful of what personal information you share online. Once it's out there, it's hard to remove.",
                    "🔒 Use privacy-focused browsers and search engines to protect your online activities.",
                    "🔒 Consider using a VPN when connecting to public Wi-Fi networks."
                },
                ["scam"] = new List<string>
                {
                    "🚨 Scammers often create a sense of urgency. Take a moment to think before acting.",
                    "🚨 If something seems too good to be true, it probably is. Trust your instincts.",
                    "🚨 Never share sensitive information like your ID number or bank details over the phone.",
                    "🚨 Report scams to the relevant authorities to help protect others."
                },
                ["2fa"] = new List<string>
                {
                    "🔐 Enable Two-Factor Authentication on all your important accounts for an extra layer of security.",
                    "🔐 2FA requires a second form of verification, making it much harder for attackers to access your accounts.",
                    "🔐 Use authenticator apps instead of SMS for 2FA as they are more secure.",
                    "🔐 Backup your 2FA recovery codes in a safe place in case you lose access to your device."
                }
            };
        }

        private void InitializeGeneralResponses()
        {
            generalResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["how are you"] = "I'm functioning optimally! Thanks for asking. How can I help you stay safe online today?",
                ["what's your purpose"] = "I'm here to help you stay safe online! I provide cybersecurity tips and answer your questions about digital safety.",
                ["what can i ask"] = "You can ask me about password safety, phishing scams, privacy protection, 2FA, or general cybersecurity tips!",
                ["hello"] = "Hello there! I'm Zoro, your Cybersecurity Awareness Bot. How can I assist you today?",
                ["hi"] = "Hi! I'm Zoro, your digital security companion. Ready to help you stay safe online!",
                ["help"] = "I can help you with:\n• Password safety tips\n• Phishing identification\n• Privacy protection\n• Scam awareness\n• Two-Factor Authentication\n\nJust type your question or topic!"
            };
        }

        public void SetUserName(string name)
        {
            userName = name;
        }

        public void SetUserInterest(string interest)
        {
            userInterest = interest;
        }

        public string GetUserName()
        {
            return userName;
        }

        public string GetUserInterest()
        {
            return userInterest;
        }

        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't quite understand that. Could you rephrase?";
            }

            // Check for general responses first
            foreach (var kvp in generalResponses)
            {
                if (userInput.ToLower().Contains(kvp.Key.ToLower()))
                {
                    return kvp.Value;
                }
            }

            // Check for keyword responses
            foreach (var kvp in keywordResponses)
            {
                if (userInput.ToLower().Contains(kvp.Key.ToLower()))
                {
                    return kvp.Value[random.Next(kvp.Value.Count)];
                }
            }

            // Check for follow-up questions
            if (userInput.ToLower().Contains("tell me more") ||
                userInput.ToLower().Contains("another tip") ||
                userInput.ToLower().Contains("explain more"))
            {
                return GetFollowUpResponse();
            }

            // Default response
            return "I'm not sure I understand. Can you try rephrasing? You can ask me about password safety, phishing, privacy, or 2FA.";
        }

        private string GetFollowUpResponse()
        {
            List<string> followUpResponses = new List<string>
            {
                "Here's another important tip: Always verify the authenticity of requests for personal information, even if they appear to come from someone you know.",
                "Remember to keep your software and apps updated. Updates often include important security patches!",
                "Did you know that using a unique password for each account significantly reduces your risk of being hacked?",
                "Consider using a password manager to generate and store strong, unique passwords for all your accounts."
            };
            return followUpResponses[random.Next(followUpResponses.Count)];
        }

        public string GetPersonalizedGreeting()
        {
            if (!string.IsNullOrEmpty(userName))
            {
                return $"Welcome back, {userName}! I remember you're interested in {userInterest ?? "cybersecurity"}. What would you like to learn about today?";
            }
            return "Welcome! What's your name?";
        }
    }
}