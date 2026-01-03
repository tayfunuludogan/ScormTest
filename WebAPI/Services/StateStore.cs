using System.Collections.Concurrent;

namespace WebAPI.Services
{
    public class StateStore
    {
        private readonly ConcurrentDictionary<string, (string ContentType, byte[] Data)> _data = new();

        public void Save(string key, string ct, byte[] data) => _data[key] = (ct, data);

        public (string ContentType, byte[] Data)? Get(string key) =>
            _data.TryGetValue(key, out var v) ? v : null;

        public void Remove(string key) => _data.TryRemove(key, out _);
    }
}
