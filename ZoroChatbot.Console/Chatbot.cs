using System;

namespace ZoroChatbot.Console
{
    public class Chatbot
    {
        private ResponseManager responseManager;
        private VoiceService voiceService;
        private bool isRunning;

        public Chatbot()
        {
            responseManager = new ResponseManager();
            voiceService = new VoiceService();
            isRunning = true;
        }

        public void Start()
        {
            Console.Title = "Zoro - Cybersecurity Awareness Bot";
            Console.Clear();

            // Play voice greeting
            voiceService.PlayGreeting();

            // Display ASCII Art
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(AsciiArt.GetLogo());
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(AsciiArt.GetShield());
            Console.ResetColor();

            // Display welcome message
            Console.WriteLine("\n" + new string('═', 70));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    🔐 WELCOME TO ZORO - YOUR CYBERSECURITY AWARENESS BOT 🔐");
            Console.ResetColor();
            Console.WriteLine(new string('═', 70));

            // Get user's name
            Console.Write("\nWhat's your name? ");
            string name = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Please enter your name: ");
                name = Console.ReadLine();
            }

            responseManager.SetUserName(name);

            Console.WriteLine($"\nNice to meet you, {name}! I'm Zoro, your Cybersecurity Awareness Bot.");
            Console.WriteLine("I'm here to help you stay safe online. You can ask me about:");
            Console.WriteLine("  • Password safety 🔑");
            Console.WriteLine("  • Phishing scams 🎣");
            Console.WriteLine("  • Privacy protection 🔒");
            Console.WriteLine("  • Scam awareness 🚨");
            Console.WriteLine("  • Two-Factor Authentication 🔐");
            Console.WriteLine("\nType 'exit' or 'quit' to end the conversation.");

            // Ask about interests
            Console.Write("\nWhat cybersecurity topic are you most interested in? ");
            string interest = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(interest))
            {
                responseManager.SetUserInterest(interest);
            }

            Console.WriteLine("\n" + new string('─', 70));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("💬 You can start chatting with me now!");
            Console.ResetColor();
            Console.WriteLine(new string('─', 70));

            // Main conversation loop
            while (isRunning)
            {
                Console.Write("\nYou: ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Zoro: I didn't quite understand that. Could you rephrase?");
                    continue;
                }

                if (userInput.ToLower() == "exit" || userInput.ToLower() == "quit")
                {
                    Console.WriteLine($"\nZoro: Goodbye, {name}! Stay safe online! 🛡️");
                    break;
                }

                // Show typing effect
                Console.Write("Zoro: ");
                string response = responseManager.GetResponse(userInput);

                // Simulate typing effect
                foreach (char c in response)
                {
                    Console.Write(c);
                    System.Threading.Thread.Sleep(15);
                }
                Console.WriteLine();
            }
        }
    }
}