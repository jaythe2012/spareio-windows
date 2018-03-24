using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using CoreLib;

namespace CoreLib
{
    public static class LogWriter
    {
        private static string _logFilePath;
        private static object _locker = new object();
        private static ProducerConsumerQueue<string> _queue;

        enum MessageType
        {
            ERROR,
            INFO,
            Warning
        }

        public static bool Enable { get; set; } = true;

        public static void Initialize(string fileName, bool useQueue = false)
        {
            lock (_locker)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    Debug.Assert(false, "fileName is not defined");
                    fileName = Assembly.GetEntryAssembly().GetName().Name + ".log";
                }
                _logFilePath = Path.Combine(PathConstants.GetLogPath(), fileName + ".log");

                if (useQueue)
                {
                    _queue = new ProducerConsumerQueue<string>(OnMessageReceived, true, "LogWriter");
                }
                Info("========================= LogWriter initialized ==========================================================");
            }
        }
        private static void OnMessageReceived(string text)
        {
            WriteToFileImpl(text);
        }
        public static void Warning(string text)
        {
            WriteToFile(FormatMessage(MessageType.Warning, text));
        }
        public static void Info(string text)
        {
            WriteToFile(FormatMessage(MessageType.INFO, text));
        }
        public static void InfoFormat(string format, params object[] args)
        {
            WriteToFile(FormatMessage(MessageType.INFO, string.Format(format, args)));
        }
        public static void Error(Exception exception)
        {
            WriteToFile(FormatMessage(MessageType.ERROR, ExceptionHandler.GetExceptionDetails(exception)));
        }
        public static void Error(string error)
        {
            WriteToFile(FormatMessage(MessageType.ERROR, error));
        }
        public static void ErrorFormat(string format, params object[] args)
        {
            WriteToFile(FormatMessage(MessageType.ERROR, string.Format(format, args)));
        }
        private static string FormatMessage(MessageType type, string message)
        {
            return string.Format("{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), type, message);
        }
        private static void WriteToFile(string text)
        {
            if (!Enable)
                return;

            if (_queue != null)
            {
                _queue.EnqueueTask(text);
            }
            else
            {
                new Action<string>(WriteToFileImplSync).BeginInvoke(text, new AsyncCallback((IAsyncResult ar) =>
                {
                    AsyncResult result = (AsyncResult)ar;
                    Action<string> caller = (Action<string>)result.AsyncDelegate;
                    caller.EndInvoke(ar);
                }), null);
            }
        }
        private static void WriteToFileImplSync(string text)
        {
            lock (_locker)
            {
                WriteToFileImpl(text);
            }
        }
        private static void WriteToFileImpl(string text)
        {
            if (!Enable)
                return;
            try
            {
                using (StreamWriter file = new StreamWriter(_logFilePath, true))
                {
                    file.WriteLine(text);
                }
            }
            catch
            {
            }
        }
    }
}
