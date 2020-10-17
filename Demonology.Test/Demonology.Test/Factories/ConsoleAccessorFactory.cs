using Demonology.Prime.Interfaces;

namespace Demonology.Test
{
    public sealed class ConsoleAccessorFactory : IFactory<ConsoleAccessor>
    {
        public ConsoleAccessor Default()
        {
            return new ConsoleAccessor();
        }
    }
}
