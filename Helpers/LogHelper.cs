using System;
using System.Collections.Generic;
using System.Text;

namespace XelLauncher.Helpers
{
    public static class LogHelper
    {
        private static readonly List<string> _entries = new();
        public static event Action OnLog;

        public static void Log(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            lock (_entries) _entries.Add(line);
            OnLog?.Invoke();
        }

        public static string GetAll()
        {
            lock (_entries) return string.Join(Environment.NewLine, _entries);
        }
    }
}
