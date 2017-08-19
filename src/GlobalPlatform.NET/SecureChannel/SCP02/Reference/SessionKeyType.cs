namespace GlobalPlatform.NET.SecureChannel.SCP02.Reference
{
    /// <summary>
    /// Based on section E.4.1 of the v2.3 GlobalPlatform Card Specification. 
    /// </summary>
    internal enum SessionKeyType
    {
        CMac = 0x01,
        RMac = 0x02,
        SEnc = 0x82,
        Dek = 0x81
    }
}
