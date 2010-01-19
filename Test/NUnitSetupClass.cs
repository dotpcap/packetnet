using System;
using NUnit.Framework;

namespace Test
{
    [SetUpFixture]
    public class MySetUpClass
    {
        public MySetUpClass()
        {
        }

        [SetUp]
        void RunBeforeAnyTests()
        {
            // load the configuration file
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("../../log4net.config"));
        }

        [TearDown]
        void RunAfterAnyTests()
        {
            // ...
        }
    }
}
