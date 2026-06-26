using System;
using System.Collections.Generic;
using System.Text;

namespace ZoroChatbot.POE
{
    public class ActivityLog
    {
        private List<string> activities;
        private int maxLogSize;

        public ActivityLog(int maxSize = 20)
        {
            activities = new List<string>();
            maxLogSize = maxSize;
        }

        public void AddActivity(string activity)
        {
            activities.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {activity}");
            if (activities.Count > maxLogSize)
            {
                activities.RemoveAt(0);
            }
        }

        public string GetLogSummary()
        {
            if (activities.Count == 0)
                return "No activities logged yet.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📋 Activity Log Summary:");
            sb.AppendLine("═".PadRight(50, '═'));

            int start = Math.Max(0, activities.Count - 10);
            for (int i = start; i < activities.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {activities[i]}");
            }

            if (activities.Count > 10)
                sb.AppendLine($"\n... and {activities.Count - 10} more activities. Type 'show full log' for complete history.");

            return sb.ToString();
        }

        public string GetFullLog()
        {
            if (activities.Count == 0)
                return "No activities logged yet.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📋 Full Activity Log:");
            sb.AppendLine("═".PadRight(50, '═'));

            for (int i = 0; i < activities.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {activities[i]}");
            }

            return sb.ToString();
        }
    }
}