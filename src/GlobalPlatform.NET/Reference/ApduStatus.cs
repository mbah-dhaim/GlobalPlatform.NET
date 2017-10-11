using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPlatform.NET.Reference
{
    public enum ApduStatus : ushort
    {
        DataAvailable = 0x6100,
        NoSpecificDiagnosis = 0x6400,
        WrongLengthInLc = 0x6700,
        LogicalChannelNotSupportedOrNotActive = 0x6881,
        SecurityStatusNotSatisfied = 0x6982,
        ConditionsOfUseNotSatisfied = 0x6985,
        IncorrectP1P2 = 0x6A86,
        WrongLengthInLe = 0x6C00,
        InvalidInstruction = 0x6D00,
        InvalidClass = 0x6E00,
        NoPreciseDiagnosis = 0x6F00,
        NoFurtherQualification = 0x9000
    }
}
