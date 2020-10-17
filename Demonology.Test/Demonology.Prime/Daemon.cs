using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demonology.Prime.Exceptions;
using Demonology.Prime.FlowResult;
using Demonology.Prime.Interfaces;
using Priority_Queue;

namespace Demonology.Prime
{
    /// <summary>
    /// Magically handles processing data and managing flows. Be sure to register flows and factories with the ResolverCache before processing.
    /// </summary>
    public class Daemon<TData, TReturn> where TReturn : class
    {
        public ResolverCache ResolverCache { get; }

        private FlowCache FlowCache { get; }
        private SimplePriorityQueue<FlowTask, int> FlowTaskQueue { get; }
        private Dictionary<Guid, IFlow> ActiveFlows { get; }
        private Action<TReturn> Callback { get; }

        private int TaskLimit { get; }
        private int OpenTasks => ActiveFlows.Count;
        private readonly object _TaskCacheSync = new object();

        public bool Busy => FlowTaskQueue.Any();

        public Daemon(int taskLimit, Action<TReturn> callback)
        {
            ResolverCache = new ResolverCache();
            FlowCache = new FlowCache(ResolverCache);
            FlowTaskQueue = new SimplePriorityQueue<FlowTask, int>();
            ActiveFlows = new Dictionary<Guid, IFlow>();
            Callback = callback;

            if (taskLimit < 2) taskLimit = 2;
            TaskLimit = taskLimit;
        }

        public void Invoke(TData data)
        {
            //Will throw if nothing was registered
            var flowTasks = ResolverCache.FlowResolver
                .GetFlowTypesFor<TData>()
                .Select(t => new FlowTask(t, data, 0))
                .ToArray();

            foreach (var flowTask in flowTasks)
            {
                FlowTaskQueue.Enqueue(flowTask, flowTask.AbsolutePriority);
            }

            Task.Run(() =>
            {
                Update();
                while (Busy)
                {
                    Thread.Sleep(1000);
                    Update();
                }
            });
        }

        private void Update()
        {
            if (!FlowTaskQueue.Any()) return;

            while (UnderTaskLimit())
            {
                var taskFound = FlowTaskQueue.TryDequeue(out var nextTask);
                if (!taskFound) break;

                var flowType = nextTask.Flow;
                var multiplicity = nextTask.Multiplicity;
                var permanence = nextTask.Permanence;
                var level = nextTask.ExecutionLevel;
                var data = nextTask.DataToProcess;
                var instance = FlowCache.Request(flowType, multiplicity, permanence);

                CreateFlowTask(instance, () =>
                {
                    //Since the Daemon is the creator of this instance, create a callback to dynamically handle the result of the flow
                    void Callback(IFlowResult result) => DaemonFlowEndpoint(result, level + 1);

                    //Invoke!
                    instance.Process(data, (Action<IFlowResult>)Callback);
                });
            }
        }

        private bool UnderTaskLimit()
        {
            lock (_TaskCacheSync)
            {
                return OpenTasks + 1 < TaskLimit;
            }
        }

        private void CreateFlowTask(IFlow instance, Action action)
        {
            Task.Run(() =>
            {
                var taskId = Guid.NewGuid();

                lock (_TaskCacheSync)
                {
                    ActiveFlows.Add(taskId, instance);
                }

                action();

                lock (_TaskCacheSync)
                {
                    ActiveFlows.Remove(taskId);
                }
            });
        }

        private void DaemonFlowEndpoint(IFlowResult result, int nextLevel)
        {
            //If the data is default (empty), just ignore it
            if (result.Data.GetType() == typeof(DefaultFlowResult)) return;

            //If the type of the data matches the return type, notify the Daemon's initiator via callback
            //Note that there may still be other flows processing which might return data
            if (result.Data.GetType() == typeof(TReturn))
            {
                var returnData = (TReturn)result.Data;
                Callback(returnData);
                return;
            }

            try
            {
                //Otherwise, handle the data according to its requirements
                if (result.MustHandle)
                {
                    ProcessFlowRequired(result, nextLevel);
                }
                else
                {
                    ProcessFlowOptional(result, nextLevel);
                }
            }
            catch (Exception e) when (e is FlowRegistrationException || e is FactoryRegistrationException)
            {
                lock (_TaskCacheSync)
                {
                    //Get all tasks and tell them to die
                    var activeFlows = ActiveFlows.Select(x => x.Value).ToArray();
                    foreach (var flow in activeFlows) flow.Die();
                }
                throw;
            }
        }

        private void ProcessFlowRequired(IFlowResult result, int nextLevel)
        {
            var dataType = (Type)result.Data.GetType();

            var flowTasks = ResolverCache.FlowResolver
                .GetFlowTypesFor(dataType)
                .Select(x => new FlowTask(x, result.Data, nextLevel))
                .ToArray();

            QueueTasksAndUpdate(flowTasks);
        }

        private void ProcessFlowOptional(IFlowResult result, int nextLevel)
        {
            var dataType = (Type)result.Data.GetType();

            var flowTasks = ResolverCache.FlowResolver
                .OptionalGetFlowTypesFor(dataType)
                .Select(x => new FlowTask(x, result.Data, nextLevel))
                .ToArray();

            QueueTasksAndUpdate(flowTasks);
        }

        private void QueueTasksAndUpdate(FlowTask[] flowTasks)
        {
            foreach (var flowTask in flowTasks)
            {
                FlowTaskQueue.Enqueue(flowTask, flowTask.AbsolutePriority);
            }

            Task.Run(() => Update());
        }
    }
}
