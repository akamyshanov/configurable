using System;

namespace Configurable.Util
{
    class OnDisposeAction : IDisposable
    {
        private readonly Action _action;

        public OnDisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}