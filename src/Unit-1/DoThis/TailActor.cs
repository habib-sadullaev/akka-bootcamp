using System;
using System.IO;
using System.Text;
using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        public class FileWrite
        {
            public FileWrite(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class FileError
        {
            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }

            public string FileName { get; }
            public string Reason { get; }
        }

        public class InitialRead
        {
            public InitialRead(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }

            public string FileName { get; }
            public string Text { get; }
        }

        private readonly IActorRef _reporter;
        private readonly string _filePath;
        private FileObserver _observer;
        private FileStream _fileStream;
        private StreamReader _fileStreamReader;

        public TailActor(IActorRef reporter, string filePath)
        {
            _reporter = reporter;
            _filePath = filePath;
        }

        protected override void PreStart()
        {
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            _fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, text));
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reporter.Tell(text);
                }
            }
            else if (message is FileError fe)
            {
                _reporter.Tell($"Tail error: {fe.Reason}");
            }
            else if (message is InitialRead ir)
            {
                _reporter.Tell(ir.Text);
            }
        }

        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }
    }
}
