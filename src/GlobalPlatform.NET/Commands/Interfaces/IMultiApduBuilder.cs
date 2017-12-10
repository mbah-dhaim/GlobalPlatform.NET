using System.Collections.Generic;
using Iso7816;

namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IMultiApduBuilder
    {
        IEnumerable<CommandApdu> AsApdus();

        IEnumerable<byte[]> AsBytes();
    }
}
