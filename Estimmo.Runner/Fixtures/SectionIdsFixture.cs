// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Fixtures
{
    public class SectionIdsFixture : Fixture<HashSet<string>>
    {
        private readonly ILogger _log = Log.ForContext<SectionIdsFixture>();
        private readonly EstimmoContext _context;

        public SectionIdsFixture(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task<HashSet<string>> ProvideValueAsync()
        {
            _log.Information("Populating section ID fixture");
            return new HashSet<string>(await _context.Sections.Select(t => t.Id).ToListAsync());
        }
    }
}
