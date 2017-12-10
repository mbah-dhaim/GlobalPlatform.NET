using System;

namespace GlobalPlatform.NET.SCP02.Reference
{
    [Flags]
    public enum SecurityLevel : byte
    {
        Authenticated = 0,
        CMac = 1,
        CDecryption = 3,
        RMac = 16,
        CMacRMac = CMac | RMac,
        CDecryptionCMacRMac = CDecryption | CMac | RMac
    }
}
