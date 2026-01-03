using System.Collections.Concurrent;

namespace WebAPI.Services
{
    public class StatementStore
    {
        private readonly ConcurrentDictionary<string, string> _data = new();

        public void Save(string id, string raw) => _data[id] = raw;

        public IEnumerable<object> All() =>
            _data.Select(x => new { id = x.Key, raw = x.Value });
    }
}
