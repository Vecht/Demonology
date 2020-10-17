using System;
using Demonology.Prime;
using Demonology.Prime.Attributes;
using Demonology.Prime.FlowResult;
using Demonology.Prime.Interfaces;
using Demonology.Test.Data;

namespace Demonology.Test
{
    [Variance(Variance.Invariant)]
    [ValidTypes(typeof(RitualResult))]
    public class RitualResultFlow : FlowBase
    {
        private ConsoleAccessor ConsoleAccessor { get; set; }

        protected override void _Initialize(Guid key, ResolverCache resolver)
        {
            ConsoleAccessor = resolver.FactoryResolver.GetFactory<ConsoleAccessorFactory>().Default();
        }

        protected override void _Process(dynamic incomingData, Action<IFlowResult> callBack)
        {
            var soulIntact = (incomingData as RitualResult)?.SoulIntact == true;

            if (soulIntact)
            {
                ConsoleAccessor.WriteLine("You have completed the ritual with your soul intact! Congratulations.");
            }
            else
            {
                ConsoleAccessor.WriteLine("Your soul has been consumed.");
            }

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("This concludes the initiation ritual.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            callBack(new FlowResult(new RitualComplete()));
        }
    }
}
