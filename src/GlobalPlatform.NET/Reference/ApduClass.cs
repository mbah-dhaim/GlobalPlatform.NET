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
    }
}
