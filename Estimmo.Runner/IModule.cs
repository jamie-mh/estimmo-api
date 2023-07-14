// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner
{
    public interface IModule
    {
        public Task RunAsync(Dictionary<string, string> args);
    }
}
