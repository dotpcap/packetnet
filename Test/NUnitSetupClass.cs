using System.Reflection;
using NUnit.Framework;

namespace Test
{
    [SetUpFixture]
    public class NUnitSetupClass
    {
        public static string CaptureDirectory {
            get {
                var testDirectory = TestContext.CurrentContext.TestDirectory;

                // trim off everything after '\\bin'
                var index = testDirectory.IndexOf("\\bin");

                // and affix the directory
                var captureDirectory = testDirectory.Remove(index) + "\\CaptureFiles\\";

                return captureDirectory;
            }
        }

        // NOTE: These are used by nunit but they appear to the compiler to be unused
        //       so silence the incorrect warning CS0169
#pragma warning disable 0169
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // load the configuration file
            var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("../../log4net.config"));
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            // ...
        }
#pragma warning restore 0169
    }
}