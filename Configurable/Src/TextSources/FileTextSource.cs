using System;
using System.IO;
using System.Threading;
using Configurable.Util;

namespace Configurable.TextSources
{
    public class FileTextSource : ITextSource, IDisposable
    {
        public string Path { get; }
        public string FullPath { get; }

        public event Action<Exception> Error;


        private string _cache;

        private int _watcherInitFlag;
        private Timer _updatedTimer;
        private FileSystemWatcher _watcher;
        private event Action<string> Updated;

        public FileTextSource(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Invalid path", nameof(path));
            }

            Path = path;
            FullPath = System.IO.Path.GetFullPath(path);
        }

        public bool Exists => File.Exists(Path);

        public string Read()
        {
            return _cache = ReadInternal();
        }

        private string ReadInternal()
        {
            return File.ReadAllText(FullPath);
        }

        public void Save(string text)
        {
            var dir = System.IO.Path.GetDirectoryName(FullPath);
            if (dir != null)
            {
                Directory.CreateDirectory(dir);
            }

            _cache = text;
            File.WriteAllText(FullPath, text);
        }

        public IDisposable OnChange(Action<string> handler)
        {
            InitializeWatcher();
            Updated += handler;
            return new OnDisposeAction(() => Updated -= handler);
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _updatedTimer?.Dispose();
        }

        private void InitializeWatcher()
        {
            const int @true = 1;
            var flag = Interlocked.Exchange(ref _watcherInitFlag, @true);
            if (flag == @true)
            {
                return;
            }

            _updatedTimer = new Timer(FireChanged);
            _watcher = CreateWatcher();
        }

        private FileSystemWatcher CreateWatcher()
        {
            var directory = System.IO.Path.GetDirectoryName(FullPath);
            var filename = System.IO.Path.GetFileName(FullPath);

            // ReSharper disable AssignNullToNotNullAttribute
            const NotifyFilters filters =
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.CreationTime |
                NotifyFilters.Size |
                NotifyFilters.FileName;

            var watcher = new FileSystemWatcher(directory, filename)
            {
                NotifyFilter = filters
            };
            // ReSharper restore AssignNullToNotNullAttribute

            watcher.Changed += OnFileChanged;
            watcher.Renamed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            _updatedTimer.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        private void FireChanged(object state)
        {
            try
            {
                var text = Exists ? ReadInternal() : null;
                var previous = Interlocked.Exchange(ref _cache, text);

                if (String.Equals(text, previous, StringComparison.Ordinal))
                {
                    return;
                }

                Updated?.Invoke(text);
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
                _cache = null;
            }
        }

    }
}
