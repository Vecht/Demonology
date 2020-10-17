using System;
using System.Threading;
using Demonology.Prime;
using Demonology.Test.Data;

namespace Demonology.Test
{
    public class Program
    {
        private static bool _done;

        static void Main(string[] args)
        {
            var daemon = new Daemon<RitualInit, RitualComplete>(100, response => _done = true);

            daemon.ResolverCache.FlowResolver.RegisterFlowType<RitualInitFlow>();
            daemon.ResolverCache.FlowResolver.RegisterFlowType<SacrificeFlow>();
            daemon.ResolverCache.FlowResolver.RegisterFlowType<RitualResultFlow>();
            daemon.ResolverCache.FactoryResolver.RegisterFactory(typeof(ConsoleAccessorFactory));
            
            daemon.Invoke(new RitualInit());

            while (!_done) { Thread.Sleep(200); }
        }
    }
}
