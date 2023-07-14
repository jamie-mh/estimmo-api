// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Shared.Entities;

namespace Estimmo.Shared.Services
{
    public interface IEstimationService
    {
        public Task<Estimate> GetEstimateAsync(EstimateRequest request, DateTime date);
    }
}
