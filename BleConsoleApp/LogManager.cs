using System;
using System.Collections.Generic;
using System.Text;

namespace BleConsoleApp
{
    public class LogManager
    {
        private static readonly Lazy<LogManager> _lazy = new Lazy<LogManager>(() => new LogManager());
        public static LogManager Instance => _lazy.Value;

        private LogManager()
        {
        }

        public void Add(string tag, string msg)
        {
            Console.WriteLine($"[{DateTimeOffset.Now:HH:mm:ss}][{tag}]{msg}");
        }
    }
}
