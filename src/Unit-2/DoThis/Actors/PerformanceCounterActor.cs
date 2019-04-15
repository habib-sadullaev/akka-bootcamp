﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;
using Akka.Util.Internal;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : UntypedActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _counter;

        private readonly HashSet<IActorRef> _subscriptions = new HashSet<IActorRef>();
        private readonly Cancelable _cancelPublishing = new Cancelable(Context.System.Scheduler);

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
        }

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(250),
                Self,
                new GatherMetrics(),
                Self,
                _cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            catch
            { }
            finally
            {
                base.PostStop();
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GatherMetrics _:
                    var metric = new Metric(_seriesName, _counter.NextValue());
                    foreach (var sub in _subscriptions)
                        sub.Tell(metric);
                    break;
                case SubscribeCounter sc:
                    _subscriptions.Add(sc.Subscriber);
                    break;
                case UnsubscribeCounter uc:
                    _subscriptions.Remove(uc.Subscriber);
                    break;
                default:
                    throw new InvalidOperationException(message.ToString());
            }   
        }
    }
}
