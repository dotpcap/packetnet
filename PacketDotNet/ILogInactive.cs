// ReSharper disable once RedundantUsingDirective

#if !DEBUG
using System;

namespace PacketDotNet
{
// For Release builds we disable logging by using this class
// in place of a log4net logger
    internal class ILogInactive
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug (object message)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Error (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Error (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void ErrorFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void ErrorFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void ErrorFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Fatal (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Fatal (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void FatalFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void FatalFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void FatalFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Info (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Info (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void InfoFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void InfoFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void InfoFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Warn (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Warn (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        public static ILogInactive GetLogger(Type type)
        {
            return new ILogInactive();
        }
    }
}
#endif