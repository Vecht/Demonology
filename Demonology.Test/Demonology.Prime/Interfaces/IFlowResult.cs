using System;

namespace Demonology.Prime.Interfaces
{
    public interface IFlowResult
    {
        Guid Key { get; }
        dynamic Data { get; }
        bool MustHandle { get; }
    }
}
