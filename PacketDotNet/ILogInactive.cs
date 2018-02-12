using System;
using System.Diagnostics;

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

        [Conditional("DEBUG")]
        public void Debug(Object message)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Debug(Object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void DebugFormat(String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void DebugFormat(IFormatProvider provider, String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void DebugFormat(String format, Object arg0)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void DebugFormat(String format, Object arg0, Object arg1)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void DebugFormat(String format, Object arg0, Object arg1, Object arg2)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Error(Object message)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Error(Object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void ErrorFormat(String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void ErrorFormat(IFormatProvider provider, String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void ErrorFormat(String format, Object arg0)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void ErrorFormat(String format, Object arg0, Object arg1)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void ErrorFormat(String format, Object arg0, Object arg1, Object arg2)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Fatal(Object message)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Fatal(Object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void FatalFormat(String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void FatalFormat(IFormatProvider provider, String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void FatalFormat(String format, Object arg0)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void FatalFormat(String format, Object arg0, Object arg1)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void FatalFormat(String format, Object arg0, Object arg1, Object arg2)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Info(Object message)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Info(Object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void InfoFormat(String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void InfoFormat(IFormatProvider provider, String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void InfoFormat(String format, Object arg0)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void InfoFormat(String format, Object arg0, Object arg1)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void InfoFormat(String format, Object arg0, Object arg1, Object arg2)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Warn(Object message)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void Warn(Object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void WarnFormat(String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void WarnFormat(IFormatProvider provider, String format, params Object[] args)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void WarnFormat(String format, Object arg0)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void WarnFormat(String format, Object arg0, Object arg1)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public void WarnFormat(String format, Object arg0, Object arg1, Object arg2)
        {
            throw new NotImplementedException();
        }

        public static ILogInactive GetLogger(Type type)
        {
            return new ILogInactive();
        }
    }
#endif
}