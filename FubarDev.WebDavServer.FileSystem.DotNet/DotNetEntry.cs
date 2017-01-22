﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.WebDavServer.Properties;
using FubarDev.WebDavServer.Properties.Dead;
using FubarDev.WebDavServer.Properties.Live;

namespace FubarDev.WebDavServer.FileSystem.DotNet
{
    public abstract class DotNetEntry : IEntry
    {
        private readonly DotNetDirectory _parent;

        protected DotNetEntry(DotNetFileSystem fileSystem, DotNetDirectory parent, FileSystemInfo info, Uri path)
        {
            _parent = parent;
            Info = info;
            DotNetFileSystem = fileSystem;
            Path = path;
        }

        public FileSystemInfo Info { get; }
        public DotNetFileSystem DotNetFileSystem { get; }
        public string Name => Info.Name;
        public IFileSystem RootFileSystem => DotNetFileSystem;
        public IFileSystem FileSystem => DotNetFileSystem;
        public ICollection Parent => _parent;
        public Uri Path { get; }
        public DateTime LastWriteTimeUtc => Info.LastWriteTimeUtc;

        public IAsyncEnumerable<IUntypedReadableProperty> GetProperties()
        {
            return new EntryProperties(this, GetLiveProperties(), GetPredefinedDeadProperties(), FileSystem.PropertyStore);
        }

        public abstract Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken);

        protected virtual IEnumerable<ILiveProperty> GetLiveProperties()
        {
            var properties = new List<ILiveProperty>()
            {
                this.GetResourceTypeProperty(),
                new LastModifiedProperty(ct => Task.FromResult(Info.LastWriteTimeUtc), SetLastWriteTimeUtc),
                new CreationDateProperty(ct => Task.FromResult(Info.CreationTimeUtc), SetCreateTimeUtc),
            };
            return properties;
        }

        protected virtual IEnumerable<IDeadProperty> GetPredefinedDeadProperties()
        {
            var properties = new List<IDeadProperty>()
            {
                new DisplayNameProperty(this, FileSystem.PropertyStore, !DotNetFileSystem.Options.ShowExtensionsForDisplayName),
            };
            return properties;
        }

        private Task SetCreateTimeUtc(DateTime value, CancellationToken cancellationToken)
        {
            Info.CreationTimeUtc = value;
            return Task.FromResult(0);
        }

        private Task SetLastWriteTimeUtc(DateTime timestamp, CancellationToken ct)
        {
            Info.LastWriteTimeUtc = timestamp;
            return Task.FromResult(0);
        }
    }
}
