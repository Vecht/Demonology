using System;
using System.Collections.Generic;
using System.Linq;
using Demonology.Prime.Attributes;
using Demonology.Prime.Exceptions;
using Demonology.Prime.Interfaces;
using Demonology.Prime.Utility;

namespace Demonology.Prime
{
    public class ResolverCache
    {
        public FlowResolver FlowResolver { get; }
        public FactoryResolver FactoryResolver { get; }

        internal ResolverCache()
        {
            FlowResolver = new FlowResolver();
            FactoryResolver = new FactoryResolver();
        }
    }

    public class FlowResolver
    {
        private readonly Dictionary<Type, HashSet<Type>> _RegisteredInvariantFlows = new Dictionary<Type, HashSet<Type>>();
        private readonly Dictionary<Type, HashSet<Type>> _RegisteredCovariantFlows = new Dictionary<Type, HashSet<Type>>();
        private readonly Dictionary<Type, HashSet<Type>> _RegisteredContravariantFlows = new Dictionary<Type, HashSet<Type>>();
        private readonly Dictionary<Type, HashSet<Type>> _CachedFlowMatches = new Dictionary<Type, HashSet<Type>>();

        internal FlowResolver() { }

        /// <summary>
        /// Register a flow and the types it can process, using its ValidTypes attribute. All registration must happen before any resolutions.
        /// </summary>
        public void RegisterFlowType<TFlow>() where TFlow : IFlow
        {
            var flowType = typeof(TFlow);

            if (!typeof(IFlow).IsAssignableFrom(flowType))
                throw new InvalidOperationException($"{flowType} must be of type {typeof(IFlow)}");

            var validTypes = FlowAttributeUtility.GetValidTypes(flowType);
            if (!validTypes.Any())
                throw new InvalidOperationException($"{flowType} does not have any valid types defined. Use the ValidTypesAttribute to specify this.");

            var variance = FlowAttributeUtility.GetVariance(flowType);
            Dictionary<Type, HashSet<Type>> registration = variance == Variance.Covariant ? _RegisteredCovariantFlows
                : variance == Variance.Contravariant ? _RegisteredContravariantFlows
                : _RegisteredInvariantFlows;

            foreach (var dataType in validTypes)
            {
                if (!registration.ContainsKey(dataType)) registration.Add(dataType, new HashSet<Type>());

                registration[dataType].Add(flowType);
            }
        }

        /// <summary>
        /// Obtain the type signature of all flows which can process the specified data type. Will throw an exception if no such flows have been registered.
        /// </summary>
        public Type[] GetFlowTypesFor<TData>() => GetFlowTypesFor(typeof(TData));

        /// <summary>
        /// Obtain the type signature of all flows which can process the specified data type. Will throw an exception if no such flows have been registered.
        /// </summary>
        public Type[] GetFlowTypesFor(Type dataType)
        {
            var flows = OptionalGetFlowTypesFor(dataType);
            if (!flows.Any()) throw new FlowRegistrationException($"No flows to process the specified type ({dataType}) have been registered.");
            return flows;
        }

        /// <summary>
        /// Obtain the type signature of all flows which can process the specified data type. Returned set may be empty.
        /// </summary>
        public Type[] OptionalGetFlowTypesFor<TData>() => OptionalGetFlowTypesFor(typeof(TData));

        /// <summary>
        /// Obtain the type signature of all flows which can process the specified data type. Returned set may be empty.
        /// </summary>
        public Type[] OptionalGetFlowTypesFor(Type dataType)
        {
            //If the matching flows have already been cached, use that
            if (_CachedFlowMatches.ContainsKey(dataType)) return _CachedFlowMatches[dataType].ToArray();


            //Get types that match invariantly
            var validFlows = new HashSet<Type>();
            if (_RegisteredInvariantFlows.ContainsKey(dataType))
            {
                var invariantMatches = _RegisteredInvariantFlows[dataType];
                foreach (var invariantMatch in invariantMatches)
                {
                    validFlows.Add(invariantMatch);
                }
            }

            //Get types that match covariantly
            _RegisteredCovariantFlows
                .Where(x => dataType.IsAssignableFrom(x.Key))
                .SelectMany(x => x.Value)
                .ToList()
                .ForEach(t => validFlows.Add(t));

            //Get types that match contravariantly
            _RegisteredContravariantFlows
                .Where(x => x.Key.IsAssignableFrom(dataType))
                .SelectMany(x => x.Value)
                .ToList()
                .ForEach(t => validFlows.Add(t));
            
            //Cache and return results
            _CachedFlowMatches.Add(dataType, validFlows);
            return validFlows.ToArray();
        }
    }

    public class FactoryResolver
    {
        private readonly HashSet<Type> _RegisteredFactoryTypes = new HashSet<Type>();

        internal FactoryResolver() { }

        /// <summary>
        /// Register a factory which can provide instances of the desired type. See IFactory for details on constraints. These constraints will not be enforced; this is only convention. Violate them at your own peril.
        /// </summary>
        public void RegisterFactory(Type factoryType)
        {
            if (!typeof(IFactoryType).IsAssignableFrom(factoryType))
                throw new InvalidOperationException($"{factoryType} must be of type {typeof(IFactory<>)}");

            if (_RegisteredFactoryTypes.Contains(factoryType)) return;

            _RegisteredFactoryTypes.Add(factoryType);
        }

        /// <summary>
        /// Returns an instance of the specified factory type, or throws an exception if no such factory was registered.
        /// </summary>
        public TFactory GetFactory<TFactory>() where TFactory : IFactoryType
        {
            var factoryType = typeof(TFactory);
            if (!_RegisteredFactoryTypes.Contains(factoryType)) throw new FactoryRegistrationException($"{factoryType} is not a registered factory.");

            var instance = (TFactory) Activator.CreateInstance(factoryType);
            return instance;
        }
    }
}
