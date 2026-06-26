using System;
using System.Media;
using System.IO;

namespace ZoroChatbot.Console
{
    public class VoiceService
    {
        private string audioFilePath;

        public VoiceService()
        {
            // Create audio directory if it doesn't exist
            string audioDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio");
            if (!Directory.Exists(audioDir))
            {
                Directory.CreateDirectory(audioDir);
            }

            audioFilePath = Path.Combine(audioDir, "greeting.wav");
        }

        public void PlayGreeting()
        {
            try
            {
                if (File.Exists(audioFilePath))
                {
                    using (SoundPlayer player = new SoundPlayer(audioFilePath))
                    {
                        player.PlaySync();
                    }
                }
                else
                {
                    Console.WriteLine("[Voice] Audio file not found. Please ensure greeting.wav is in the audio folder.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Voice] Error playing audio: {ex.Message}");
            }
        }

        public void SaveAudioFile(byte[] audioData)
        {
            try
            {
                File.WriteAllBytes(audioFilePath, audioData);
                Console.WriteLine("[Voice] Audio file saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Voice] Error saving audio: {ex.Message}");
            }
        }
    }
}