using System;
using Demonology.Prime;
using Demonology.Prime.Attributes;
using Demonology.Prime.FlowResult;
using Demonology.Prime.Interfaces;
using Demonology.Test.Data;

namespace Demonology.Test
{
    [Variance(Variance.Invariant)]
    [ValidTypes(typeof(RitualInit))]
    public class RitualInitFlow : FlowBase
    {
        private ConsoleAccessor ConsoleAccessor { get; set; }

        protected override void _Initialize(Guid key, ResolverCache resolver)
        {
            ConsoleAccessor = resolver.FactoryResolver.GetFactory<ConsoleAccessorFactory>().Default();
        }

        protected override void _Process(dynamic incomingData, Action<IFlowResult> callBack)
        {
            ConsoleAccessor.WriteLine("Demonology v0.1: Initiation Ritual.");
            ConsoleAccessor.WriteLine("-------------------------------------");
            ConsoleAccessor.WriteLine("What offering do you want to sacrifice?");
            ConsoleAccessor.WriteLine("Goat | Ox | Human");
            ConsoleAccessor.Write("-> ");

            var initialInput = ConsoleAccessor.Read();
            if (string.IsNullOrEmpty(initialInput) || !Enum.TryParse(initialInput, ignoreCase: true, result: out SacrificeType offering))
            {
                ConsoleAccessor.WriteLine("Your sacrifice is unacceptable!");
                callBack(new FlowResult(Guid.Empty, new RitualComplete(), mustHandle: true));
                return;
            }

            var sacrifice = new Sacrifice(offering);
            callBack(new FlowResult(sacrifice));
        }
    }
}
