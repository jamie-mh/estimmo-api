using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner
{
    public class BatchListProcessor<T, TU>
    {
        private const int DefaultBufferSize = 10000;

        private readonly List<TU> _buffer;
        private readonly Func<T, Task<TU>> _processItemAsync;
        private readonly Func<List<TU>, int, Task> _flushBufferAsync;

        public BatchListProcessor(Func<T, Task<TU>> processItemAsync, Func<List<TU>, int, Task> flushBufferAsync, int bufferSize = DefaultBufferSize)
        {
            _buffer = new List<TU>(bufferSize);
            _processItemAsync = processItemAsync;
            _flushBufferAsync = flushBufferAsync;
        }

        public BatchListProcessor(Func<T, TU> processItem, Func<List<TU>, int, Task> flushBufferAsync, int bufferSize = DefaultBufferSize)
        {
            _buffer = new List<TU>(bufferSize);
            _processItemAsync = i => Task.FromResult(processItem(i));
            _flushBufferAsync = flushBufferAsync;
        }

        private async Task FlushBufferIfFullAsync(int processed, bool force)
        {
            if (force || _buffer.Count % _buffer.Capacity == 0)
            {
                await _flushBufferAsync(_buffer, processed);
                _buffer.Clear();
            }
        }

        public async Task ProcessAsync(IAsyncEnumerable<T> source)
        {
            var processed = 0;

            await foreach (var item in source)
            {
                var result = await _processItemAsync(item);

                if (result == null)
                {
                    continue;
                }

                processed++;
                _buffer.Add(result);

                await FlushBufferIfFullAsync(processed, false);
            }

            await FlushBufferIfFullAsync(processed, _buffer.Any());
        }

        public async Task ProcessAsync(IEnumerable<T> source)
        {
            var processed = 0;

            foreach (var item in source)
            {
                var result = await _processItemAsync(item);

                if (result == null)
                {
                    continue;
                }

                processed++;
                _buffer.Add(result);

                await FlushBufferIfFullAsync(processed, false);
            }

            await FlushBufferIfFullAsync(processed, _buffer.Any());
        }
    }
}
