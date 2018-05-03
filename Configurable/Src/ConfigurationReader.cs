using System;
using System.Threading;
using Configurable.Serializers;
using Configurable.TextSources;
using Configurable.Util;

namespace Configurable
{
    public class ConfigurationReader<T> : IConfigurationReader<T>
        where T : class, new()
    {
        private int _changesRegistrationFlag;

        public event Action<Exception> Error;
        public ITextSource TextSource { get; }
        public ISerializer Serializer { get; }

        private event Action<T> Updated;

        public ConfigurationReader(ITextSource textSource, ISerializer serializer)
        {
            TextSource = textSource;
            Serializer = serializer;
        }


        public T Read()
        {
            return TextSource.Exists
                ? ReadFromSource()
                : GetDefault();
        }

        public T GetDefault()
        {
            var configuration = new T();
            TextSource.Save(Serializer.Serialize(configuration));
            return configuration;
        }

        public T ReadFromSource()
        {
            var text = TextSource.Read();
            var configuration = Serializer.Deserialize<T>(text) ?? new T();
            SaveIfDifferent(configuration, text);
            return configuration;
        }

        public IDisposable OnChanges(Action<T> handler)
        {
            const int @true = 1;
            var flag = Interlocked.Exchange(ref _changesRegistrationFlag, @true);
            if (flag != @true)
            {
                TextSource.OnChange(TextSourceOnUpdated);
            }

            Updated += handler;
            return new OnDisposeAction(() => Updated -= handler);
        }

        private void SaveIfDifferent(T configuration, string current)
        {
            var serialized = Serializer.Serialize(configuration);
            if (!String.Equals(current, serialized, StringComparison.Ordinal))
            {
                TextSource.Save(serialized);
            }
        }

        private void TextSourceOnUpdated(string text)
        {
            try
            {
                if (text == null)
                {
                    throw new ArgumentException($"Text from '{TextSource}' is null", nameof(text));
                }

                var configuration = Serializer.Deserialize<T>(text) ?? new T();
                SaveIfDifferent(configuration, text);
                Updated?.Invoke(configuration);
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex);
            }
        }
    }

    public static class ConfigurationReader
    {
        public static IConfigurationReader<T> Default<T>(string path)
            where T : class, new()
        {
            return new ConfigurationReader<T>(
                new FileTextSource(path),
                JsonSerializer.Instance);
        }
    }
}
