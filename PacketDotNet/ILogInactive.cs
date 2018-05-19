// ReSharper disable once RedundantUsingDirective
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
        public void Debug (Object message)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Debug (Object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat(String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (System.IFormatProvider provider, String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (String format, Object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (String format, Object arg0, Object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (String format, Object arg0, Object arg1, Object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (Object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (Object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat(String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (System.IFormatProvider provider, String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (String format, Object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (String format, Object arg0, Object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (String format, Object arg0, Object arg1, Object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (Object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (Object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat(String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (System.IFormatProvider provider, String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (String format, Object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (String format, Object arg0, Object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (String format, Object arg0, Object arg1, Object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (Object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (Object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat(String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (System.IFormatProvider provider, String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (String format, Object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (String format, Object arg0, Object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (String format, Object arg0, Object arg1, Object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (Object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (Object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat(String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (System.IFormatProvider provider, String format, params Object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (String format, Object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (String format, Object arg0, Object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (String format, Object arg0, Object arg1, Object arg2)
        {
            throw new System.NotImplementedException ();
        }

        public static ILogInactive GetLogger(Type type)
        {
            return new ILogInactive();
        }
    }
#endif
}