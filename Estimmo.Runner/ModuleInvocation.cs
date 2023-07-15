// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace Estimmo.Runner
{
    public class ModuleInvocation
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
    }
}
