using System;
using Demonology.Prime.Interfaces;

namespace Demonology.Prime.FlowResult
{
    public class FlowResult : IFlowResult
    {
        public Guid Key { get; }
        public dynamic Data { get; }
        public bool MustHandle { get; }

        public FlowResult(dynamic data)
        {
            Key = Guid.Empty;
            Data = data;
            MustHandle = true;
        }

        public FlowResult(Guid key, dynamic data, bool mustHandle)
        {
            Key = key;
            Data = data;
            MustHandle = MustHandle;
        }
    }
}
