using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Jobs.Configuration.Duplicate
{
    public class DuplicateConfigurationViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IJobRepository _repository;

        public DuplicateConfigurationViewModel(Guid jobId)
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IJobRepository)Bootstrap.Instance.Services.GetService(typeof(IJobRepository));

            JobId = jobId;
        }

        private Guid jobId = Guid.Empty;

        public Guid JobId
        {
            get { return jobId; }
            set
            {
                jobId = value;
                RaisePropertyChanged(nameof(JobId));
            }
        }

        private Compare.CompareValue.Types compareValueTypes;

        public Compare.CompareValue.Types CompareValueTypes
        {
            get { return compareValueTypes; }
            set
            {
                compareValueTypes = value;
                RaisePropertyChanged(nameof(CompareValueTypes));
                OnSetCompareValueTypes();
            }
        }

        private bool compareValueTypesDirectory;

        public bool CompareValueTypesDirectory
        {
            get { return (CompareValueTypes & Compare.CompareValue.Types.Directory) == Compare.CompareValue.Types.Directory; }
            set
            {
                compareValueTypesDirectory = value;
                RaisePropertyChanged(nameof(CompareValueTypesDirectory));
                OnChangeCompareValueTypes();
            }
        }

        private bool compareValueTypesExtension;

        public bool CompareValueTypesExtension
        {
            get { return (CompareValueTypes & Compare.CompareValue.Types.Extension) == Compare.CompareValue.Types.Extension; }
            set
            {
                compareValueTypesExtension = value;
                RaisePropertyChanged(nameof(CompareValueTypesExtension));
                OnChangeCompareValueTypes();
            }
        }

        private bool compareValueTypesFileName;

        public bool CompareValueTypesFileName
        {
            get { return (CompareValueTypes & Compare.CompareValue.Types.FileName) == Compare.CompareValue.Types.FileName; }
            set
            {
                compareValueTypesFileName = value;
                RaisePropertyChanged(nameof(CompareValueTypesFileName));
                OnChangeCompareValueTypes();
            }
        }

        private bool compareValueTypesFileNamePartial;

        public bool CompareValueTypesFileNamePartial
        {
            get { return (CompareValueTypes & Compare.CompareValue.Types.FileNamePartial) == Compare.CompareValue.Types.FileNamePartial; }
            set
            {
                compareValueTypesFileNamePartial = value;
                RaisePropertyChanged(nameof(CompareValueTypesFileNamePartial));
                OnChangeCompareValueTypes();
            }
        }

        private bool compareValueTypesHash;

        public bool CompareValueTypesHash
        {
            get { return (CompareValueTypes & Compare.CompareValue.Types.Hash) == Compare.CompareValue.Types.Hash; }
            set
            {
                compareValueTypesHash = value;
                RaisePropertyChanged(nameof(CompareValueTypesHash));
                OnChangeCompareValueTypes();
            }
        }


        private void OnChangeCompareValueTypes()
        {
            if (!internCompareValueTypesChange)
            {
                var types = Compare.CompareValue.Types.None;
                if (compareValueTypesHash)
                    types |= Compare.CompareValue.Types.Hash;
                if (compareValueTypesFileNamePartial)
                    types |= Compare.CompareValue.Types.FileNamePartial;
                if (compareValueTypesFileName)
                    types |= Compare.CompareValue.Types.FileName;
                if (compareValueTypesExtension)
                    types |= Compare.CompareValue.Types.Extension;
                if (compareValueTypesDirectory)
                    types |= Compare.CompareValue.Types.Directory;
                CompareValueTypes = types;
            }
        }

        private bool internCompareValueTypesChange;

        private void OnSetCompareValueTypes()
        {
            internCompareValueTypesChange = true;
            if ((compareValueTypes & Compare.CompareValue.Types.Directory) == Compare.CompareValue.Types.Directory)
                CompareValueTypesDirectory = true;
            if ((compareValueTypes & Compare.CompareValue.Types.Extension) == Compare.CompareValue.Types.Extension)
                CompareValueTypesExtension = true;
            if ((compareValueTypes & Compare.CompareValue.Types.FileName) == Compare.CompareValue.Types.FileName)
                CompareValueTypesFileName = true;
            if ((compareValueTypes & Compare.CompareValue.Types.FileNamePartial) == Compare.CompareValue.Types.FileNamePartial)
                CompareValueTypesFileNamePartial = true;
            if ((compareValueTypes & Compare.CompareValue.Types.Hash) == Compare.CompareValue.Types.Hash)
                CompareValueTypesHash = true;
            internCompareValueTypesChange = false;
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        p => true,
                        async p => await OnRefresh());
                }
                return _refreshCommand;
            }
        }

        private async Task OnRefresh()
        {
            try
            {
                var model =  await _repository.JobConfigurationDuplicates(JobId);
                if (model == null)
                {
                    model = new Models.JobConfigurationDuplicates();
                    model.CompareValueTypes = (int)Compare.CompareValue.Types.Hash;
                }
                CompareValueTypes = (Compare.CompareValue.Types)model.CompareValueTypes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        private ICommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        p => true,
                        async p => await OnSave());
                }
                return _saveCommand;
            }
        }

        private async Task OnSave()
        {
            try
            {
                await _repository.JobConfigurationDuplicatesChange(new Models.JobConfigurationDuplicates
                {
                    CompareValueTypes = (int)compareValueTypes,
                    Id = Guid.NewGuid(),
                    JobId = jobId
                });
                await OnRefresh();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnSave failed to load data");
            }
        }
    }
}
