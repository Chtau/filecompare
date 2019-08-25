using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates.DuplicateAction
{
    public class DuplicateActionDialogViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IDuplicatesRepository _repository;
        private readonly Folders.IFolderRepository _repositoryFolders;

        private readonly Guid _duplicateValueId;

        public DuplicateActionDialogViewModel(Guid duplicateValueId)
        {
            _duplicateValueId = duplicateValueId;
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IDuplicatesRepository)Bootstrap.Instance.Services.GetService(typeof(IDuplicatesRepository));
        }


    }
}
