using System.Reflection;
using System.IO;
using NUnit.Framework;

namespace Test
{
    [SetUpFixture]
    public class NUnitSetupClass
    {
        public static string CaptureDirectory {
            get {
                var testDirectory = TestContext.CurrentContext.TestDirectory;

                var index = testDirectory.IndexOf(Path.DirectorySeparatorChar + "bin");

                // trim off everything after 'Path.DirectorySeparatorCharPath' and affix the directory
                var captureDirectory = testDirectory.Remove(index) + Path.DirectorySeparatorChar + "CaptureFiles" + Path.DirectorySeparatorChar;

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