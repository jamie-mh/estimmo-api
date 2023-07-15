// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Estimmo.Runner.Fixtures
{
    public abstract class Fixture<T>
    {
        private T _value;
        private readonly SemaphoreSlim _lock;

        protected Fixture()
        {
            _lock = new SemaphoreSlim(1, 1);
        }

        public async Task<T> GetValueAsync()
        {
            await _lock.WaitAsync();
            
            if (_value == null)
            {
                _lock.Release();
                throw new InvalidOperationException("Value must be loaded first");
            }

            _lock.Release();
            return _value;
        }

        public async Task LoadAsync()
        {
            await _lock.WaitAsync();
            
            if (_value != null)
            {
                _lock.Release();
                return;
            }

            try
            {
                _value = await ProvideValueAsync();
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<T> LoadAndGetValueAsync()
        {
            await LoadAsync();
            return await GetValueAsync();
        }

        protected abstract Task<T> ProvideValueAsync();
    }
}
