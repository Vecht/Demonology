using System;

namespace Demonology.Prime.Attributes
{
    /// <summary>
    /// Determines the variance of the flow with respect to input types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VarianceAttribute : Attribute
    {
        public Variance Variance { get; }

        public VarianceAttribute(Variance variance)
        {
            Variance = variance;
        }
    }

    public enum Variance
    {
        Invariant,
        Covariant,
        Contravariant
    }
}
