/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Reflection;
using log4net;
using log4net.Repository.Hierarchy;

namespace Test;

    public class LoggingConfiguration
    {
        public static log4net.Core.Level GlobalLoggingLevel
        {
            get
            {
                var rootLogger = ((Hierarchy) LogManager.GetRepository(Assembly.GetCallingAssembly())).Root;
                return rootLogger.Level;
            }
            set
            {
                var rootLogger = ((Hierarchy) LogManager.GetRepository(Assembly.GetCallingAssembly())).Root;
                rootLogger.Level = value;
            }
        }
    }