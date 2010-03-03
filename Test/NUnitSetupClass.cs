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

        // NOTE: These are used by nunit but they appear to the compiler to be unused
        //       so silence the incorrect warning CS0169
#pragma warning disable 0169
        [SetUp]
        public void RunBeforeAnyTests()
        {
            // load the configuration file
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("../../log4net.config"));
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            // ...
        }
#pragma warning restore 0169
    }
}
