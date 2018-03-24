using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using CoreLib;

namespace CoreLib
{
    public static class ExceptionHandler
    {
        public static void HandleException<T>(Exception exception)
        {
            string error;
            HandleException(exception, out error);
        }
        public static void HandleException(Exception exception, out string error)
        {
            error = GetExceptionDetails(exception);
            Debug.Assert(false, error);
            LogWriter.Error(exception);
        }
        public static void HandleException(Exception exception)
        {
            string error;
            HandleException(exception, out error);
        }
        public static string GetExceptionDetails(Exception e)
        {
            string details = "";
            /*            if (e is AggregateException)
                        {
                            foreach (var ex in ((AggregateException)e).Flatten().InnerExceptions)
                            {
                                details += GetDetails(ex);
                            }
                        }
                        else
                        {*/
            e = e.GetBaseException();
            details = GetDetails(e);
            //            }
            return details;
        }

        public static string GetWebExceptionDetails(WebException e)
        {
            string text = "";
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                WebResponse resp = e.Response;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
            }
            return string.Format("Text: {0}\nStatus: {1}\nDetails:{2}", text, e.Status, GetDetails(e));
        }
        private static string GetDetails(Exception e)
        {
            return string.Format("{0}:{1}\n{2}\n", e.GetType().FullName, e.Message, e.StackTrace);
        }
    }
}
