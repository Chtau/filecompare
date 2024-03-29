﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates
{
    public interface IDuplicatesRepository
    {
        Task<List<ViewModels.DuplicatesResult>> Duplicates();
        Task<List<ViewModels.DuplicatesResultPath>> DuplicatesPaths(Guid duplicateValueId);
        Task<bool> DeletePathDuplicate(Guid duplicateValueId, Guid pathCompareValueId);
        Task<bool> CheckDuplicateRemove(Guid duplicateId);
        Task<bool> ClearDuplicates();
        Task<bool> AutoResolveAllDuplicates();
    }
}
