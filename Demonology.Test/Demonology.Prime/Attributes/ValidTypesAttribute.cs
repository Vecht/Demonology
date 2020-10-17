using System;

namespace Demonology.Prime.Attributes
{
    /// <summary>
    /// Specifies the valid types the flow can take as parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidTypesAttribute : Attribute
    {
        public Type[] ValidTypes { get; }

        public ValidTypesAttribute(params Type[] validTypes)
        {
            ValidTypes = validTypes;
        }
    }
}
