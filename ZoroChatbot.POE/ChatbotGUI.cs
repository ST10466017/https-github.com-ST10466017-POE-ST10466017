using System;
using System.Collections.Generic;
using ZoroChatbot.Console;

namespace ZoroChatbot.GUI
{
    public class ChatbotGUI
    {
        private ResponseManager responseManager;

        public ChatbotGUI()
        {
            responseManager = new ResponseManager();
        }

        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't quite understand that. Could you rephrase?";
            }

            return responseManager.GetResponse(userInput);
        }
    }
}