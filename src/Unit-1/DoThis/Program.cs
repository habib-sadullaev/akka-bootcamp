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

            var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor)));

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            await MyActorSystem.WhenTerminated;
        }
    }
    #endregion
}
