using System;
using System.Linq;
using Demonology.Prime.Attributes;

namespace Demonology.Prime.Utility
{
    public static class FlowAttributeUtility
    {
        /// <summary>
        /// Multiplicity determines instance count of the flow; either multiplicity is allowed or only a single instance should be instantiated. The default is allow.
        /// </summary>
        public static Multiplicity GetMultiplicity(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(MultiplicityAttribute), false).FirstOrDefault() as MultiplicityAttribute;
            return att?.Multiplicity ?? Multiplicity.Allow;
        }

        /// <summary>
        /// Permanence determines how references to the flow are kept. Transient instances will be valid targets for garbage collection. By default, instances are kept and managed by the Daemon.
        /// </summary>
        public static Permanence GetPermanence(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(PermanenceAttribute), false).FirstOrDefault() as PermanenceAttribute;
            return att?.Permanence ?? Permanence.Keep;
        }

        /// <summary>
        /// Priority is a number between 0 and 100 which informs the Daemon as to the flow's priority. Flows of the same level with higher priority will be given more resources. The default priority is "Low."
        /// </summary>
        public static double GetPriority(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(PriorityAttribute), false).FirstOrDefault() as PriorityAttribute;
            return att?.Priority ?? (int)FlowPriority.Low;
        }

        /// <summary>
        /// Determines the variance of the flow with respect to input types. The default is Invariant.
        /// </summary>
        public static Variance GetVariance(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(VarianceAttribute), false).FirstOrDefault() as VarianceAttribute;
            return att?.Variance ?? Variance.Invariant;
        }

        /// <summary>
        /// Specifies the valid types the flow can take as parameters. This must be specified, otherwise the flow will not be usable.
        /// </summary>
        public static Type[] GetValidTypes(Type flowType)
        {
            var att = flowType.GetCustomAttributes(typeof(ValidTypesAttribute), false).FirstOrDefault() as ValidTypesAttribute;
            return att?.ValidTypes ?? new Type[] { };
        }
    }
}
