using System;
using Akka.Actor;

namespace WinTail
{
    class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message is string msg)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    _consoleWriterActor.Tell(new Messages.NullInputError("No input received"));
                }
                else if (IsValid(msg))
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid"));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid! Input had odd number of characters"));
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValid(string msg) => msg.Length % 2 == 0;
    }
}
