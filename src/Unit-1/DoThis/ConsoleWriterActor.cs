using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Messages.InputError err:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(err.Reason);
                    break;
                case Messages.InputSuccess suc:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(suc.Reason);
                    break;
                default:
                    Console.WriteLine(message);
                    break;
            }

            Console.ResetColor();
        }
    }
}
