﻿using Akka.Actor;
using System;
using System.IO;

namespace WinTail
{
    /// <summary>
    /// Turns <see cref="FileSystemWatcher"/> events about a specific file into
    /// messages for <see cref="TailActor"/>.
    /// </summary>
    public class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private readonly string _absoluteFilePath;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _absoluteFilePath = absoluteFilePath;
            _fileDir = Path.GetDirectoryName(absoluteFilePath);
            _fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        /// <summary>
        /// Begin monitoring file.
        /// </summary>
        public void Start()
        {
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Error += OnFileError;

            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stop monitoring file.
        /// </summary>
        public void Dispose()
        {
            _watcher.Dispose();
        }

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file error events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFileError(object sender, ErrorEventArgs e)
        {
            _tailActor.Tell(new TailActor.FileError(_fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
        }

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file change events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }
    }
}
