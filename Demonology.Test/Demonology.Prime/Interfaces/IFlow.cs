using System;

namespace Demonology.Prime.Interfaces
{
    /// <summary>
    /// Represents a flow that takes in data and asynchronously processes the data, providing the result via the given callback.
    /// </summary>
    public interface IFlow
    {
        void Initialize(Guid key, ResolverCache resolver);
        void Process<TData>(TData incomingData, Action<IFlowResult> callBack);
        void Hurry();
        void Die();
        bool Busy { get; }
    }
}
