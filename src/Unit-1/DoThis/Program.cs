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

            var tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
            var tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

            var consoleWriterProps = Props.Create<ConsoleWriterActor>();
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

            var fileValidationProps = Props.Create(() => new FileValidationActor(consoleWriterActor));
            var fileValidationActor = MyActorSystem.ActorOf(fileValidationProps, "fileValidationActor");

            var consoleReaderProps = Props.Create<ConsoleReaderActor>();
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            await MyActorSystem.WhenTerminated;
        }
    }
    #endregion
}
