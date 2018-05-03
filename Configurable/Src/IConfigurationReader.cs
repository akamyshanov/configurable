using System;
using Configurable.Serializers;
using Configurable.TextSources;

namespace Configurable
{
    public interface IConfigurationReader<out T>
    {
        event Action<Exception> Error;

        ITextSource TextSource { get; }
        ISerializer Serializer { get; }
        T Read();
        T GetDefault();
        T ReadFromSource();
        IDisposable OnChanges(Action<T> handler);
    }
}