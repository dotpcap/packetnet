using System.Reflection;
using PacketDotNet;
using SharpPcap;

namespace Test
{
    public static class RawCaptureExtensions
    {
        private static readonly MethodInfo GetLinkLayerType;

        /// <summary>
        /// Initializes static members of the <see cref="RawCaptureExtensions" /> class.
        /// </summary>
        static RawCaptureExtensions()
        {
            var propertyInfo = typeof(RawCapture).GetProperty("LinkLayerType", BindingFlags.Public | BindingFlags.Instance);
            GetLinkLayerType = propertyInfo?.GetMethod;
        }

        public static LinkLayers GetLinkLayers(this RawCapture rawCapture)
        {
            // Allows using PacketDotNet versions other than the one used by SharpPcap.
            return (LinkLayers) (GetLinkLayerType?.Invoke(rawCapture, null) ?? 0);
        }
    }
}