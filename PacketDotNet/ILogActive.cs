using System;
using log4net.Core;

namespace PacketDotNet
{
#if DEBUG
    internal class ILogActive
    {
        public static log4net.ILog GetLogger(Type type)
        {
            return log4net.LogManager.GetLogger(type);
        }
    }
#else
    internal class ILogActive
    {
        private log4net.ILog wrappedLogger;

        private ILogActive(log4net.ILog wrappedLogger)
        {
            this.wrappedLogger = wrappedLogger;
        }

        public ILogger Logger
        {
            get { return wrappedLogger.Logger; }
        }

        public bool IsDebugEnabled
        {
            get { return wrappedLogger.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return wrappedLogger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return wrappedLogger.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return wrappedLogger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return wrappedLogger.IsFatalEnabled; }
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Debug (object message)
        {
            wrappedLogger.Debug(message);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Debug (object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat(string format, params object[] args)
        {
            wrappedLogger.DebugFormat(format, args);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (System.IFormatProvider provider, string format, params object[] args)
        {
            wrappedLogger.DebugFormat(provider, format, args);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0)
        {
            wrappedLogger.DebugFormat(format, arg0);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1)
        {
            wrappedLogger.DebugFormat(format, arg0, arg1);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void DebugFormat (string format, object arg0, object arg1, object arg2)
        {
            wrappedLogger.DebugFormat(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Error (object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void ErrorFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Fatal (object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void FatalFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Info (object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void InfoFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (object message)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void Warn (object message, System.Exception exception)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat(string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (System.IFormatProvider provider, string format, params object[] args)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1)
        {
            throw new System.NotImplementedException ();
        }

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public void WarnFormat (string format, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException ();
        }

        public static ILogActive GetLogger(Type type)
        {
            return new ILogActive(log4net.LogManager.GetLogger(type));
        }
    }
#endif
}
