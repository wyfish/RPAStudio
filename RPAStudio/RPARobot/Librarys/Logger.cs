using log4net;
using System;

namespace RPARobot.Librarys
{
    public class Logger
    {
        public static void Debug(string message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Debug(message);
            }
        }

        public static void Debug(object message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Debug(message.ToString());
            }
        }

        public static void Info(string message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Info(message);
            }
        }

        public static void Info(object message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Info(message.ToString());
            }
        }


        public static void Warn(string message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Warn(message);
            }
        }

        public static void Warn(object message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Warn(message.ToString());
            }
        }
        public static void Error(string message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Error(message);
            }
        }

        public static void Error(object message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Error(message.ToString());
            }
        }

        public static void Fatal(string message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Fatal(message);
            }
        }

        public static void Fatal(object message, ILog logger = null)
        {
#if DEBUG
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (logger != null)
            {
                logger.Fatal(message.ToString());
            }
        }

    }
}
