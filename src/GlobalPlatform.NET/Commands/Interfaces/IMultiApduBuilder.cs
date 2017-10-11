using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IMultiApduBuilder
    {
        IEnumerable<CommandApdu> AsApdus();

        IEnumerable<byte[]> AsBytes();
    }
}
