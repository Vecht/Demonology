using System;
using Demonology.Prime.Interfaces;

namespace Demonology.Prime.FlowResult
{
    public class DefaultFlowResult : IFlowResult
    {
        public Guid Key => Guid.Empty;
        public dynamic Data => new object();
        public bool MustHandle => false;
    }
}
