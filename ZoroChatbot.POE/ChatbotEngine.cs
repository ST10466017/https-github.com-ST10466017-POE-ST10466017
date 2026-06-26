using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZoroChatbot.POE;

namespace ZoroChatbot.POE
{
    public class ChatbotEngine
    {
        private DatabaseHelper dbHelper;
        private QuizGame quizGame;
        private ActivityLog activityLog;
        private SentimentAnalyzer sentimentAnalyzer;
        private ResponseManager responseManager;
        private Random random;
        private string userName;
        private bool quizActive;

        public ChatbotEngine()
        {
            dbHelper = new DatabaseHelper();
            quizGame = new QuizGame();
            activityLog = new ActivityLog();
            sentimentAnalyzer = new SentimentAnalyzer();
            responseManager = new ResponseManager();
            random = new Random();
            quizActive = false;
        }

        public void SetUserName(string name)
        {
            userName = name;
            responseManager.SetUserName(name);
        }

        public string ProcessUserInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't quite understand that. Could you rephrase?";
            }

            // Check for sentiment
            string sentiment = sentimentAnalyzer.DetectSentiment(userInput);
            string encouragement = sentimentAnalyzer.GetEncouragingResponse(sentiment);
            if (!string.IsNullOrEmpty(encouragement))
            {
                activityLog.AddActivity($"Sentiment detected: {sentiment}");
                return encouragement;
            }

            // Check for activity log request
            if (userInput.ToLower().Contains("show activity log") ||
                userInput.ToLower().Contains("what have you done") ||
                userInput.ToLower().Contains("show log"))
            {
                return activityLog.GetLogSummary();
            }

            if (userInput.ToLower().Contains("show full log"))
            {
                return activityLog.GetFullLog();
            }

            // Check for task-related commands
            if (userInput.ToLower().Contains("add task") || userInput.ToLower().Contains("new task"))
            {
                return HandleAddTask(userInput);
            }

            if (userInput.ToLower().Contains("show tasks") || userInput.ToLower().Contains("list tasks"))
            {
                return HandleShowTasks();
            }

            if (userInput.ToLower().Contains("complete task") || userInput.ToLower().Contains("done task"))
            {
                return HandleCompleteTask(userInput);
            }

            if (userInput.ToLower().Contains("delete task") || userInput.ToLower().Contains("remove task"))
            {
                return HandleDeleteTask(userInput);
            }

            // Check for quiz commands
            if (userInput.ToLower().Contains("start quiz") || userInput.ToLower().Contains("begin quiz"))
            {
                quizActive = true;
                quizGame.ResetQuiz();
                activityLog.AddActivity("Started cybersecurity quiz");
                return GetNextQuizQuestion();
            }

            if (quizActive)
            {
                return HandleQuizAnswer(userInput);
            }

            // Check for random cybersecurity tip
            if (userInput.ToLower().Contains("give me a tip") ||
                userInput.ToLower().Contains("random tip") ||
                userInput.ToLower().Contains("cyber tip"))
            {
                return GetRandomTip();
            }

            // Check for 2FA commands
            if (userInput.ToLower().Contains("2fa") || userInput.ToLower().Contains("two factor"))
            {
                return Get2FAResponse();
            }

            // Check for password-related commands (NLP simulation)
            if (userInput.ToLower().Contains("password") || userInput.ToLower().Contains("pass word"))
            {
                return responseManager.GetResponse("password");
            }

            // Check for phishing-related commands
            if (userInput.ToLower().Contains("phish") || userInput.ToLower().Contains("scam"))
            {
                return responseManager.GetResponse("phishing");
            }

            // Check for privacy-related commands
            if (userInput.ToLower().Contains("privac"))
            {
                return responseManager.GetResponse("privacy");
            }

            // Check for follow-up questions
            if (userInput.ToLower().Contains("tell me more") ||
                userInput.ToLower().Contains("another tip") ||
                userInput.ToLower().Contains("explain more"))
            {
                return GetFollowUpResponse();
            }

            // Handle general questions
            string generalResponse = responseManager.GetResponse(userInput);
            if (!generalResponse.Contains("I'm not sure I understand"))
            {
                return generalResponse;
            }

            return "I'm not sure I understand. You can ask me about:\n• Cybersecurity tips\n• Tasks (add, show, complete, delete)\n• The quiz ('start quiz')\n• Specific topics like password, phishing, or privacy";
        }

        private string HandleAddTask(string userInput)
        {
            // Extract task title
            string taskTitle = userInput.Replace("add task", "").Replace("new task", "").Trim();

            if (string.IsNullOrEmpty(taskTitle))
            {
                return "Please specify what task you'd like to add. For example: 'Add task: Review privacy settings'";
            }

            string description = $"Task: {taskTitle}";
            dbHelper.AddTask(taskTitle, description, null);
            activityLog.AddActivity($"Task added: {taskTitle}");

            return $"✅ Task '{taskTitle}' has been added to your list. Would you like to set a reminder? (Type 'remind me in X days' or 'no reminder')";
        }

        private string HandleShowTasks()
        {
            var tasks = dbHelper.GetTasks();
            if (tasks.Count == 0)
            {
                return "📋 You don't have any tasks yet. Would you like to add one?";
            }

            string response = "📋 Your Tasks:\n" + "═".PadRight(50, '═') + "\n";
            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "✅" : "⏳";
                response += $"{status} {task.Title}";
                if (task.ReminderDate.HasValue)
                {
                    response += $" (Reminder: {task.ReminderDate.Value:yyyy-MM-dd})";
                }
                response += $"\n   ID: {task.Id}\n";
            }
            return response;
        }

        private string HandleCompleteTask(string userInput)
        {
            // Try to extract task ID
            var match = Regex.Match(userInput, @"\d+");
            if (!match.Success)
            {
                return "Please specify the task ID to complete. For example: 'complete task 3'";
            }

            int taskId = int.Parse(match.Value);
            dbHelper.UpdateTaskStatus(taskId, true);
            activityLog.AddActivity($"Task {taskId} marked as completed");

            return $"✅ Task {taskId} has been marked as completed! Great job staying on top of your cybersecurity tasks!";
        }

        private string HandleDeleteTask(string userInput)
        {
            var match = Regex.Match(userInput, @"\d+");
            if (!match.Success)
            {
                return "Please specify the task ID to delete. For example: 'delete task 3'";
            }

            int taskId = int.Parse(match.Value);
            dbHelper.DeleteTask(taskId);
            activityLog.AddActivity($"Task {taskId} deleted");

            return $"🗑️ Task {taskId} has been deleted.";
        }

        private string GetNextQuizQuestion()
        {
            var question = quizGame.GetNextQuestion();
            if (question == null)
            {
                quizActive = false;
                return QuizComplete();
            }

            string response = $"📝 Question {quizGame.GetScore() + 1} of {quizGame.GetTotalQuestions()}:\n";
            response += question.Question + "\n\n";
            foreach (var option in question.Options)
            {
                response += option + "\n";
            }
            response += "\nEnter the letter of your answer (A, B, C, or D):";
            return response;
        }

        private string HandleQuizAnswer(string userInput)
        {
            int selectedIndex = -1;
            string input = userInput.ToUpper().Trim();

            if (input == "A") selectedIndex = 0;
            else if (input == "B") selectedIndex = 1;
            else if (input == "C") selectedIndex = 2;
            else if (input == "D") selectedIndex = 3;

            if (selectedIndex == -1)
            {
                return "Please enter A, B, C, or D for your answer.";
            }

            bool isCorrect = quizGame.AnswerQuestion(selectedIndex);
            var currentQuestion = quizGame.GetNextQuestion(); // This gets the next question after answering

            string response;
            if (isCorrect)
            {
                response = "✅ Correct! ";
                activityLog.AddActivity($"Quiz: Correct answer");
            }
            else
            {
                response = "❌ Incorrect. ";
                activityLog.AddActivity($"Quiz: Incorrect answer");
            }

            if (quizGame.IsQuizComplete())
            {
                return QuizComplete();
            }

            // Get the next question
            return response + "\n\n" + GetNextQuizQuestion();
        }

        private string QuizComplete()
        {
            int score = quizGame.GetScore();
            int total = quizGame.GetTotalQuestions();
            string feedback = quizGame.GetFeedback();
            quizActive = false;
            activityLog.AddActivity($"Quiz completed: {score}/{total}");

            return $"🎉 Quiz Complete!\n" +
                   $"Score: {score}/{total}\n" +
                   $"{feedback}";
        }

        private string GetRandomTip()
        {
            List<string> tips = new List<string>
            {
                "🔐 Use different passwords for each of your accounts.",
                "🛡️ Enable 2FA on all accounts that support it.",
                "📱 Keep your software and apps updated.",
                "🔍 Verify the sender before clicking on email links.",
                "🛑 Never share sensitive information over the phone unless you initiated the call.",
                "💡 Use a VPN on public Wi-Fi networks.",
                "🔒 Regular review your privacy settings on social media.",
                "🗑️ Shred documents containing personal information before disposing."
            };
            return tips[random.Next(tips.Count)];
        }

        private string Get2FAResponse()
        {
            List<string> responses = new List<string>
            {
                "🔐 Two-Factor Authentication adds an extra layer of security. Even if someone gets your password, they can't access your account without the second factor.",
                "🔐 Use an authenticator app like Google Authenticator or Microsoft Authenticator for 2FA instead of SMS when possible.",
                "🔐 Always backup your 2FA recovery codes in a secure place.",
                "🔐 2FA is one of the most effective ways to protect your online accounts from unauthorized access."
            };
            return responses[random.Next(responses.Count)];
        }

        private string GetFollowUpResponse()
        {
            List<string> responses = new List<string>
            {
                "Here's another important tip: Always verify the authenticity of requests for personal information, even if they appear to come from someone you know.",
                "Remember to keep your software and apps updated. Updates often include important security patches!",
                "Consider using a password manager to generate and store strong, unique passwords for all your accounts.",
                "Did you know that enabling 2FA significantly reduces your risk of being hacked?"
            };
            return responses[random.Next(responses.Count)];
        }
    }
}// Add this to the ChatbotEngine class
private TicTacToeGame ticTacToeGame;
private bool ticTacToeActive;

// Add to the constructor:
public ChatbotEngine()
{
    // ... existing initialization ...
    ticTacToeGame = new TicTacToeGame();
    ticTacToeActive = false;
}

// Add this method to handle Tic-Tac-Toe commands
public string HandleTicTacToeCommand(string userInput)
{
    if (userInput.ToLower().Contains("start tic") ||
        userInput.ToLower().Contains("play tic") ||
        userInput.ToLower().Contains("new game tic"))
    {
        ticTacToeActive = true;
        ticTacToeGame.ResetGame();
        activityLog.AddActivity("Tic-Tac-Toe game started");
        return "🎮 Tic-Tac-Toe game started!\n\n" +
               ticTacToeGame.GetBoardDisplay() +
               "\nIt's your turn (X). Enter your move as: 'move 1,1' or 'place 2,3'";
    }

    if (ticTacToeActive)
    {
        // Parse move command
        if (userInput.ToLower().Contains("move") ||
            userInput.ToLower().Contains("place") ||
            userInput.ToLower().Contains("put"))
        {
            // Extract numbers from input
            var numbers = System.Text.RegularExpressions.Regex.Matches(userInput, @"\d+");
            if (numbers.Count >= 2)
            {
                int row = int.Parse(numbers[0].Value) - 1;
                int col = int.Parse(numbers[1].Value) - 1;

                string result = ticTacToeGame.MakeMove(row, col);
                activityLog.AddActivity($"Tic-Tac-Toe move at ({row + 1}, {col + 1})");

                if (ticTacToeGame.IsGameOver())
                {
                    ticTacToeActive = false;
                    return result + "\n\nGame over! Type 'start tic' to play again.";
                }

                return result + "\n\n" + ticTacToeGame.GetGameStatus();
            }
            else
            {
                return "Please specify your move as 'move row,col'. For example: 'move 1,2'";
            }
        }

        if (userInput.ToLower().Contains("show board") ||
            userInput.ToLower().Contains("display board"))
        {
            return ticTacToeGame.GetBoardDisplay() + "\n" + ticTacToeGame.GetGameStatus();
        }

        if (userInput.ToLower().Contains("end tic") ||
            userInput.ToLower().Contains("quit tic") ||
            userInput.ToLower().Contains("stop tic"))
        {
            ticTacToeActive = false;
            activityLog.AddActivity("Tic-Tac-Toe game ended");
            return "Tic-Tac-Toe game ended. Thanks for playing!";
        }

        if (userInput.ToLower().Contains("help tic") ||
            userInput.ToLower().Contains("tic help"))
        {
            return GetTicTacToeHelp();
        }
    }

    return null;
}

private string GetTicTacToeHelp()
{
    return @"🎮 Tic-Tac-Toe Commands:
    • 'start tic' - Start a new game
    • 'move row,col' - Place your X (e.g., 'move 1,2')
    • 'show board' - Display the current board
    • 'end tic' - End the game
    • 'help tic' - Show this help

    💡 You are X, Computer is O
    💡 First to get 3 in a row wins!";
}
public string ProcessUserInput(string userInput)
{
    // ... existing code ...

    // Check for Tic-Tac-Toe commands
    if (userInput.ToLower().Contains("tic") ||
        userInput.ToLower().Contains("tac") ||
        userInput.ToLower().Contains("toe") ||
        ticTacToeActive)
    {
        string ticResult = HandleTicTacToeCommand(userInput);
        if (ticResult != null)
            return ticResult;
    }

    // Check for other commands...
    // ... existing code ...
}