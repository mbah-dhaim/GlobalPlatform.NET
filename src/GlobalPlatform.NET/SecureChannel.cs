using GlobalPlatform.NET.SCP02;

namespace GlobalPlatform.NET
{
    public interface ISecureChannelProtocolPicker
    {
        /// <summary>
        /// Sets up an SCP02 secure channel. 
        /// <para> Based on section E of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <returns></returns>
        IScp02SessionBuilder Scp02();
    }

    public class SecureChannel : ISecureChannelProtocolPicker
    {
        /// <summary>
        /// Starts setting up a secure channel. 
        /// </summary>
        public static ISecureChannelProtocolPicker Setup => new SecureChannel();

        public IScp02SessionBuilder Scp02() => new SecureChannelSession();
    }
}
