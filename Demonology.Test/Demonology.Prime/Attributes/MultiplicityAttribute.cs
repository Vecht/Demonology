using System;

namespace Demonology.Prime.Attributes
{
    /// <summary>
    /// Determines the multiplicity of the flow; whether multiplicity is allowed or if only a single instance should be instantiated. The default is allow.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MultiplicityAttribute : Attribute
    {
        public Multiplicity Multiplicity { get; }

        public MultiplicityAttribute(Multiplicity multiplicity)
        {
            Multiplicity = multiplicity;
        }
    }

    public enum Multiplicity
    {
        Allow,
        Singleton
    }
}
