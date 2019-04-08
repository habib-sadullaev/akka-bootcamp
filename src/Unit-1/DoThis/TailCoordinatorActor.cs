using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporter)
            {
                FilePath = filePath;
                Reporter = reporter;
            }

            public string FilePath { get; }
            public IActorRef Reporter { get; }
        }

        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }
        protected override void OnReceive(object message)
        {
            if (message is StartTail start)
            {
                Context.ActorOf(Props.Create(() => new TailActor(start.Reporter, start.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: x =>
                {
                    if (x is ArithmeticException) return Directive.Resume;

                    if (x is NotSupportedException) return Directive.Stop;

                    return Directive.Restart;
                });
        }
    }
}
