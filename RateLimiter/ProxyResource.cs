using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class ProxyResource<T> : IResource<T>
    {
        private readonly int _numberOfAllowedCalls;
        private readonly TimeSpan _window;
        private readonly IResource<T> _realResource;

        private readonly List<DateTimeOffset> _calls;

        public ProxyResource(int numberOfAllowedCalls, TimeSpan window, IResource<T> realResource)
        {
            _numberOfAllowedCalls = numberOfAllowedCalls;
            _window = window;
            _realResource = realResource;

            _calls = new List<DateTimeOffset>();
        }

        public T Invoke()
        {
            _calls.RemoveAll(c => c <= DateTimeOffset.Now - _window);

            if (_calls.Count() >= _numberOfAllowedCalls)
                throw new LimitedException();

            _calls.Add(DateTimeOffset.Now);
            return _realResource.Invoke();
        }
    }
}
