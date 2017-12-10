namespace GlobalPlatform.NET.Reference
{
    public static class ApduClass
    {
        /// <summary>
        /// Command defined in ISO/IEC 7816. 
        /// </summary>
        public static byte Iso7816 = 0x00;

        /// <summary>
        /// GlobalPlatform command. 
        /// </summary>
        public static byte GlobalPlatform = 0x80;

        /// <summary>
        /// Secure messaging – GlobalPlatform proprietary. 
        /// </summary>
        public static byte SecureMessaging = 0x84;

        /// <summary>
        /// Defines the type of byte coding used in the CLA byte.
        /// <para> Based on section 11.1.4 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        public enum ByteCoding
        {
            First,
            InterIndustry
        }
    }
}
