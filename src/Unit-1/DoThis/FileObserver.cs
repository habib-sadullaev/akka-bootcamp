using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    internal class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private readonly string _absolutePath;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;
        private FileSystemWatcher _watcher;

        public FileObserver(IActorRef tailActor, string absolutePath)
        {
            _tailActor = tailActor;
            _absolutePath = absolutePath;
            _fileDir = Path.GetDirectoryName(absolutePath);
            _fileNameOnly = Path.GetFileName(absolutePath);
        }

        internal void Start()
        {
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            _watcher.Changed += _watcher_Changed;
            _watcher.Error += _watcher_Error;
            _watcher.EnableRaisingEvents = true;
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            _tailActor.Tell(new TailActor.FileError(_fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}