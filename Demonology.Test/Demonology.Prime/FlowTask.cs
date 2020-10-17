using System;
using System.Linq;
using Demonology.Prime.Attributes;

namespace Demonology.Prime
{
    public class FlowTask
    {
        public Type Flow { get; }
        public dynamic DataToProcess { get; }

        public Multiplicity Multiplicity { get; }
        public Permanence Permanence { get; }

        public int ExecutionLevel { get; }
        public int RelativePriority { get; }
        public int AbsolutePriority => (ExecutionLevel * 1000) + (100 - RelativePriority);

        public FlowTask(Type flow, dynamic data, int level)
        {
            Flow = flow;
            DataToProcess = data;

            Multiplicity = GetMultiplicity(flow);
            Permanence = GetPermanence(flow);

            ExecutionLevel = level;
            RelativePriority = GetPriority(flow);
        }

        /// <summary>
        /// Multiplicity determines instance count of the flow; either multiplicity is allowed or only a single instance should be instantiated. The default is allow.
        /// </summary>
        private static Multiplicity GetMultiplicity(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(MultiplicityAttribute), false).FirstOrDefault() as MultiplicityAttribute;
            return att?.Multiplicity ?? Multiplicity.Allow;
        }

        /// <summary>
        /// Permanence determines how references to the flow are kept. Transient instances will be valid targets for garbage collection. By default, instances are kept and managed by the Daemon.
        /// </summary>
        private static Permanence GetPermanence(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(PermanenceAttribute), false).FirstOrDefault() as PermanenceAttribute;
            return att?.Permanence ?? Permanence.Keep;
        }

        /// <summary>
        /// Priority is a number between 0 and 100 which informs the Daemon as to the flow's priority. Flows of the same level with higher priority will be given more resources. The default priority is "Low."
        /// </summary>
        private static int GetPriority(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(PriorityAttribute), false).FirstOrDefault() as PriorityAttribute;
            return att?.Priority ?? (int)FlowPriority.Low;
        }
    }
}
