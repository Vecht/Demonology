using System;

namespace Demonology.Prime.Attributes
{
    /// <summary>
    /// Determines how references to the flow are kept. Transient instances will be valid targets for garbage collection. By default, instances are kept and managed by the Daemon.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PermanenceAttribute : Attribute
    {
        public Permanence Permanence { get; }

        public PermanenceAttribute(Permanence permanence)
        {
            Permanence = permanence;
        }
    }

    public enum Permanence
    {
        Keep,
        Transient
    }
}
