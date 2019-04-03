using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static async Task Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create(nameof(MyActorSystem));

            var consoleWriterProps = Props.Create<ConsoleWriterActor>();
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");
            var validationProps = Props.Create(() => new ValidationActor(consoleWriterActor));
            var validationActor = MyActorSystem.ActorOf(validationProps, "validationActor");
            var consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            await MyActorSystem.WhenTerminated;
        }
    }
    #endregion
}
