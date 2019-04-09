using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    class FileValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriter;

        public FileValidationActor(IActorRef consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        protected override void OnReceive(object message)
        {
            if (message is string msg)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    _consoleWriter.Tell(new Messages.NullInputError("Input was blank. Pleas try again.\n"));
                    Sender.Tell(new Messages.ContinueProcessing());
                }
                else if (IsFileUri(msg))
                {
                    _consoleWriter.Tell(new Messages.InputSuccess($"Starting process for {msg}"));
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor")
                        .Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriter));
                }
                else
                {
                    _consoleWriter.Tell(new Messages.ValidationError($"{msg} is not existing URI on disk"));
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
        }

        private bool IsFileUri(string msg) => File.Exists(msg);
    }
}
