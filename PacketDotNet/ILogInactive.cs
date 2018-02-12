using System;

#if DEBUG
#endif

namespace PacketDotNet
{
#if !DEBUG
    // For Release builds we disable logging by using this class
    // in place of a log4net logger
    internal class ILogInactive
    {
#if false
        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }
#endif

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Debug (object message)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Debug (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (object message)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (object message, Exception exception)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat(string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1)
        {
            throw new NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException ();
        }

        public static ILogInactive GetLogger(Type type)
        {
            return new ILogInactive();
        }
    }
#endif
}
