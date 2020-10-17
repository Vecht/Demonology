using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demonology.Prime.Attributes;
using Demonology.Prime.Interfaces;

namespace Demonology.Prime
{
    internal class FlowCache
    {
        private Dictionary<Type, List<Func<IFlow>>> Cache { get; } = new Dictionary<Type, List<Func<IFlow>>>();
        private readonly object _CacheSync = new object();

        private readonly ResolverCache _ResolverCache;
        public FlowCache(ResolverCache resolverCache)
        {
            _ResolverCache = resolverCache;
        }

        /// <summary>
        /// Guaranteed to return an instance of the specified type. Will cache and manage instances already instantiated.
        /// </summary>
        public IFlow Request(Type instanceType, Multiplicity multiplicity, Permanence permanence)
        {
            //We only want to allow a single instance of the type, so just get one if there's already one created.
            //Unfortunately, this will not respect the flow's "busy" state. Hey -- don't look at me like that. That's what you get for using singletons.
            if (multiplicity == Multiplicity.Singleton && MaybeGetFirstInstance(instanceType, out var instance)) return instance;

            //Otherwise, try to get the first non-busy instance of the type
            if (MaybeGetFirstNonBusyInstance(instanceType, out instance)) return instance;

            //Else we have to create a new one
            instance = (IFlow)Activator.CreateInstance(instanceType);
            instance.Initialize(Guid.NewGuid(), _ResolverCache);

            //The stuff here is just bookkeeping, so no need to hold up the requesting thread.
            Task.Run(() =>
            {
                //First, clear any instances that have expired.
                //Of course, they might expire at any point -- this is just to keep space down.
                ClearExpiredInstances(instanceType);

                //Either hold on to the object ourselves, or only keep a weak reference, depending on the flow metadata
                Func<IFlow> accessor;
                if (permanence == Permanence.Keep) accessor = () => instance;
                else
                {
                    //Closures are AWESOME.
                    var weakReference = new WeakReference<IFlow>(instance);
                    accessor = () =>
                    {
                        weakReference.TryGetTarget(out var maybeInstance);
                        return maybeInstance;
                    };
                }
                CacheAdd(instanceType, accessor);
            });

            return instance;
        }

        private bool MaybeGetFirstInstance(Type instanceType, out IFlow instance)
        {
            lock (_CacheSync)
            {
                if (!Cache.ContainsKey(instanceType))
                {
                    instance = null;
                    return false;
                }

                instance = Cache[instanceType]
                    .Select(x => x.Invoke())
                    .FirstOrDefault(x => x != null);
            }

            return instance != null;
        }

        private bool MaybeGetFirstNonBusyInstance(Type instanceType, out IFlow instance)
        {
            lock (_CacheSync)
            {
                if (!Cache.ContainsKey(instanceType))
                {
                    instance = null;
                    return false;
                }

                instance = Cache[instanceType]
                    .Select(x => x.Invoke())
                    .FirstOrDefault(x => x?.Busy == false);
            }

            return instance != null;
        }

        private void CacheAdd(Type instanceType, Func<IFlow> accessor)
        {
            lock (_CacheSync)
            {
                if (!Cache.ContainsKey(instanceType)) Cache.Add(instanceType, new List<Func<IFlow>>());
                Cache[instanceType].Add(accessor);
            }
        }

        private void ClearExpiredInstances(Type instanceType)
        {
            lock (_CacheSync)
            {
                var activatorLists = Cache.Where(x => x.Key == instanceType).Select(x => x.Value);
                foreach (var activatorList in activatorLists)
                {
                    activatorList.RemoveAll(x => x() == null);
                }
            }
        }
    }
}
