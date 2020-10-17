using System;

namespace Demonology.Prime.Attributes
{
    /// <summary>
    /// A number between 0 and 100 which informs the Daemon as to the flow's priority. Flows of the same level with higher priority will be given more resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PriorityAttribute : Attribute
    {
        public int Priority { get; }

        public PriorityAttribute(int priority)
        {
            if (priority < 0) priority = 0;
            if (priority > 100) priority = 100;
            Priority = priority;
        }

        public PriorityAttribute(FlowPriority priority)
        {
            Priority = (int)priority;
        }
    }

    public enum FlowPriority
    {
        Minimum = 0,
        Low = 25,
        Moderate = 50,
        High = 75,
        Critical = 100
    }
}
