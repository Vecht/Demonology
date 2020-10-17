using System;
using Demonology.Prime;
using Demonology.Prime.Attributes;
using Demonology.Prime.FlowResult;
using Demonology.Prime.Interfaces;
using Demonology.Test.Data;

namespace Demonology.Test
{
    [Variance(Variance.Invariant)]
    [ValidTypes(typeof(Sacrifice))]
    [Priority(666)]
    public class SacrificeFlow : FlowBase
    {
        private ConsoleAccessor ConsoleAccessor { get; set; }

        protected override void _Initialize(Guid key, ResolverCache resolver)
        {
            ConsoleAccessor = resolver.FactoryResolver.GetFactory<ConsoleAccessorFactory>().Default();
        }

        protected override void _Process(dynamic incomingData, Action<IFlowResult> callBack)
        {
            var sacrificeType = (incomingData as Sacrifice)?.Type;

            ConsoleAccessor.WriteLine($"The {sacrificeType} has been sacrificed, and the ritual has begun. Now, speak!");
            ConsoleAccessor.Write("-> ");

            var input = ConsoleAccessor.Read();

            bool satanPleased;
            if (string.IsNullOrEmpty(input))
            {
                ConsoleAccessor.WriteLine("Satan responds: \"Insolent wretch!\"");
                satanPleased = false;
            }
            else if (input == "Hail, Satan!")
            {
                ConsoleAccessor.WriteLine("Satan responds: \"Your master is pleased, vassal.\"");
                satanPleased = true;
            }
            else
            {
                ConsoleAccessor.WriteLine("Satan responds: \"Begone, whelp!\"");
                satanPleased = sacrificeType == SacrificeType.Human;
            }

            var ritualResult = new RitualResult
            {
                SoulIntact = satanPleased
            };

            callBack(new FlowResult(ritualResult));
        }
    }
}
