using System;
using log4net;
using log4net.Repository.Hierarchy;

namespace Test
{
    public class LoggingConfiguration
    {
        public static log4net.Core.Level GlobalLoggingLevel
        {
            get
            {
                Logger rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
                return rootLogger.Level;
            }

            set
            {
                Logger rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
                rootLogger.Level = value;
            }
        }
    }
}
