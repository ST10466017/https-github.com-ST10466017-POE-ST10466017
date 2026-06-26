using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;

namespace ZoroChatbot.GUI
{
    public partial class MainWindow : Window
    {
        private ChatbotGUI chatbot;
        private DispatcherTimer typingTimer;
        private string currentResponse;
        private int charIndex;

        public MainWindow()
        {
            InitializeComponent();
            chatbot = new ChatbotGUI();

            // Play voice greeting
            PlayVoiceGreeting();

            // Display welcome message
            DisplayWelcomeMessage();

            // Initialize typing timer
            typingTimer = new DispatcherTimer();
            typingTimer.Interval = TimeSpan.FromMilliseconds(20);
            typingTimer.Tick += TypingTimer_Tick;
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio", "greeting.wav");
                if (File.Exists(audioPath))
                {
                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail for audio
            }
        }

        private void DisplayWelcomeMessage()
        {
            AddMessage("Zoro", "Hello! I'm Zoro, your Cybersecurity Awareness Bot. I'm here to help you stay safe online! 🛡️", "#00D2FF");
            AddMessage("Zoro", "You can ask me about:\n• Password safety 🔑\n• Phishing scams 🎣\n• Privacy protection 🔒\n• Scam awareness 🚨\n• Two-Factor Authentication 🔐", "#00D2FF");
            AddMessage("System", "Type your question below to get started!", "#FFD700");
        }

        private void AddMessage(string sender, string message, string colorHex)
        {
            var border = new Border
            {
                Style = (Style)FindResource("ChatBubbleStyle"),
                Margin = new Thickness(0, 3, 0, 3),
                MaxWidth = 500,
                HorizontalAlignment = sender == "User" ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = sender == "User"
                    ? new SolidColorBrush(Color.FromRgb(0, 210, 255))
                    : new SolidColorBrush(Color.FromRgb(22, 33, 62))
            };

            var stackPanel = new StackPanel();

            var senderLabel = new TextBlock
            {
                Text = sender == "User" ? "You" : sender,
                FontWeight = FontWeights.Bold,
                Foreground = sender == "User"
                    ? new SolidColorBrush(Colors.White)
                    : new SolidColorBrush(Color.FromRgb(0, 210, 255)),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 3)
            };
            stackPanel.Children.Add(senderLabel);

            var messageText = new TextBlock
            {
                Text = message,
                Foreground = sender == "User"
                    ? new SolidColorBrush(Colors.Black)
                    : new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            stackPanel.Children.Add(messageText);

            border.Child = stackPanel;
            MessagePanel.Children.Add(border);

            MessageScrollViewer.ScrollToBottom();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ProcessUserInput();
                e.Handled = true;
            }
        }

        private void ProcessUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                AddMessage("System", "Please enter a question or topic.", "#FF6B6B");
                return;
            }

            AddMessage("User", userInput, "#00D2FF");
            InputTextBox.Clear();

            // Get response with typing effect
            currentResponse = chatbot.GetResponse(userInput);
            charIndex = 0;

            var typingMessage = new Border
            {
                Style = (Style)FindResource("ChatBubbleStyle"),
                Margin = new Thickness(0, 3, 0, 3),
                MaxWidth = 500,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = new SolidColorBrush(Color.FromRgb(22, 33, 62))
            };

            var stackPanel = new StackPanel();

            var senderLabel = new TextBlock
            {
                Text = "Zoro",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 210, 255)),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 3)
            };
            stackPanel.Children.Add(senderLabel);

            var messageText = new TextBlock
            {
                Text = "",
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            stackPanel.Children.Add(messageText);

            typingMessage.Child = stackPanel;
            MessagePanel.Children.Add(typingMessage);

            typingTimer.Tag = new Tuple<Border, TextBlock>(typingMessage, messageText);
            typingTimer.Start();
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            var tag = (Tuple<Border, TextBlock>)typingTimer.Tag;
            var messageText = tag.Item2;

            if (charIndex < currentResponse.Length)
            {
                messageText.Text += currentResponse[charIndex];
                charIndex++;
                MessageScrollViewer.ScrollToBottom();
            }
            else
            {
                typingTimer.Stop();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MessagePanel.Children.Clear();
            DisplayWelcomeMessage();
        }
    }
}