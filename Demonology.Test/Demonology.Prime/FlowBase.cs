using System;
using Demonology.Prime.Interfaces;

namespace Demonology.Prime
{
    public abstract class FlowBase : IFlow
    {
        public bool Initialized { get; private set; }
        public void Initialize(Guid key, ResolverCache resolver)
        {
            _Initialize(key, resolver);
            Initialized = true;
        }

        protected abstract void _Initialize(Guid key, ResolverCache resolver);

        public virtual void Process<TData>(TData incomingData, Action<IFlowResult> callBack)
        {
            if (!Initialized) throw new InvalidOperationException("The flow has not been initialized.");
            Busy = true;
            _Process(incomingData, callBack);
            Busy = false;
        }
        protected abstract void _Process(dynamic incomingData, Action<IFlowResult> callBack);


        protected bool HurryRequested { get; private set; }
        public void Hurry()
        {
            HurryRequested = true;
        }


        protected bool DieRequested { get; private set; }
        public void Die()
        {
            DieRequested = true;
        }

        /// <summary>
        /// Signals that the flow is busy and that it should not be used for additional processing. Note that this will be ignored for Singleton instances
        /// </summary>
        public bool Busy { get; private set; }
    }
}
