using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;

namespace DesignPatterns
{
    #region Bad god logger
    class BadGodLogger
    {
        public void Log(string message)
        {
            // Console logging
            Console.WriteLine(message);

            // File logging
            File.AppendAllLines("log.txt", new[] { message });

            // Imagine 3rd logger and 4th logger and so on...
        }
    } 

    class BadLoggingApp
    {
        static void Main()
        {
            var logger = new BadGodLogger();

            logger.Log("First message");
            logger.Log("second message");
        }
    }
    #endregion

    #region Improved logger but verbose
    interface ILogger
    {
        void Log(string message);
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine(message);
    }

    class FileLogger : ILogger
    {
        public void Log(string message) => File.AppendAllLines("log.txt", new[] { message });
    }

    class ImprovedLoggerApp
    {
        static void Main()
        {
            var loggers = new ILogger[] { new ConsoleLogger(), new FileLogger() };

            foreach (var logger in loggers)
            {
                logger.Log("First message");
            }

            foreach (var logger in loggers)
            {
                logger.Log("Second message");
            }
        }
    }
    #endregion

    #region Good logger using Composite design pattern
    class ForEachLogger : ILogger // The Composite implementation of the interface
    {
        private readonly ILogger[] _loggers;

        public ForEachLogger(params ILogger[] loggers) => _loggers = loggers;

        public void Log(string message)
        {
            foreach (var logger in _loggers)
            { 
                logger.Log(message);
            }
        }
    }

    class GoodLoggingApp
    {
        static void Main()
        {
            var logger = new ForEachLogger(
                    new ConsoleLogger(),
                    new FileLogger());

            logger.Log("First message");
            logger.Log("second message");
        }
    } 
    #endregion
}
