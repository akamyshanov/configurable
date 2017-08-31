using System;

namespace Configurable.TextSources
{
    public interface ITextSource
    {
        event Action<string> Updated;
        event Action<Exception> Error;
        bool Exists { get; }
        string Read();
        void Save(string text);
    }
}