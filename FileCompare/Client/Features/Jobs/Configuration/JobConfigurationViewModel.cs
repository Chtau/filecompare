using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Jobs.Configuration
{
    public class JobConfigurationViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IJobRepository _repository;
        private readonly Folders.IFolderRepository _repositoryFolders;

        public JobConfigurationViewModel(Guid jobId)
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IJobRepository)Bootstrap.Instance.Services.GetService(typeof(IJobRepository));
            _repositoryFolders = (Folders.IFolderRepository)Bootstrap.Instance.Services.GetService(typeof(Folders.IFolderRepository));

            JobId = jobId;
            JobTypeEnum = Internal.ComboBoxBindingModelBuilder.FromEnum(typeof(JobType), false);
            Paths = new ObservableCollection<KeyValuePair<Guid, string>>();
        }

        private ObservableCollection<ViewModels.JobPathView> jobCollectPathItems;

        public ObservableCollection<ViewModels.JobPathView> JobCollectPathItems
        {
            get { return jobCollectPathItems; }
            set
            {
                jobCollectPathItems = value;
                RaisePropertyChanged(nameof(JobCollectPathItems));
            }
        }

        public bool IsNew
        {
            get { return JobId == Guid.Empty; }
        }

        public bool IsNotNew
        {
            get { return JobId != Guid.Empty; }
        }

        private Guid jobId = Guid.Empty;

        public Guid JobId
        {
            get { return jobId; }
            set
            {
                jobId = value;
                RaisePropertyChanged(nameof(JobId));
                RaisePropertyChanged(nameof(IsNew));
                RaisePropertyChanged(nameof(IsNotNew));
            }
        }

        private string jobName;

        public string JobName
        {
            get { return jobName; }
            set
            {
                jobName = value;
                RaisePropertyChanged(nameof(JobName));
            }
        }

        private JobType jobType;

        public JobType JobType
        {
            get { return jobType; }
            set
            {
                jobType = value;
                RaisePropertyChanged(nameof(JobType));
            }
        }

        private Guid jobConfigurationId;

        public Guid JobConfigurationId
        {
            get { return jobConfigurationId; }
            set
            {
                jobConfigurationId = value;
                RaisePropertyChanged(nameof(JobConfigurationId));
            }
        }

        private Day jobConfigurationDays;

        public Day JobConfigurationDays
        {
            get { return jobConfigurationDays; }
            set
            {
                jobConfigurationDays = value;
                RaisePropertyChanged(nameof(JobConfigurationDays));
                OnSetJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysMonday;

        public bool JobConfigurationDaysMonday
        {
            get { return (JobConfigurationDays & Day.Monday) == Day.Monday; }
            set
            {
                jobConfigurationDaysMonday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysMonday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysTuesday;

        public bool JobConfigurationDaysTuesday
        {
            get { return (JobConfigurationDays & Day.Tuesday) == Day.Tuesday; }
            set
            {
                jobConfigurationDaysTuesday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysTuesday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysWednesday;

        public bool JobConfigurationDaysWednesday
        {
            get { return (JobConfigurationDays & Day.Wednesday) == Day.Wednesday; }
            set
            {
                jobConfigurationDaysWednesday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysWednesday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysThursday;

        public bool JobConfigurationDaysThursday
        {
            get { return (JobConfigurationDays & Day.Thursday) == Day.Thursday; }
            set
            {
                jobConfigurationDaysThursday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysThursday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysFriday;

        public bool JobConfigurationDaysFriday
        {
            get { return (JobConfigurationDays & Day.Friday) == Day.Friday; }
            set
            {
                jobConfigurationDaysFriday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysFriday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysSaturday;

        public bool JobConfigurationDaysSaturday
        {
            get { return (JobConfigurationDays & Day.Saturday) == Day.Saturday; }
            set
            {
                jobConfigurationDaysSaturday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysSaturday));
                OnChangeJobConfigurationDays();
            }
        }

        private bool jobConfigurationDaysSunday;

        public bool JobConfigurationDaysSunday
        {
            get { return (JobConfigurationDays & Day.Sunday) == Day.Sunday; }
            set
            {
                jobConfigurationDaysSunday = value;
                RaisePropertyChanged(nameof(JobConfigurationDaysSunday));
                OnChangeJobConfigurationDays();
            }
        }

        private void OnChangeJobConfigurationDays()
        {
            if (!internJobDayChange)
            {
                var days = Day.None;
                if (jobConfigurationDaysMonday)
                    days |= Day.Monday;
                if (jobConfigurationDaysTuesday)
                    days |= Day.Tuesday;
                if (jobConfigurationDaysWednesday)
                    days |= Day.Wednesday;
                if (jobConfigurationDaysThursday)
                    days |= Day.Thursday;
                if (jobConfigurationDaysFriday)
                    days |= Day.Friday;
                if (jobConfigurationDaysSaturday)
                    days |= Day.Saturday;
                if (jobConfigurationDaysSunday)
                    days |= Day.Sunday;
                JobConfigurationDays = days;
            }
        }

        private bool internJobDayChange;

        private void OnSetJobConfigurationDays()
        {
            internJobDayChange = true;
            if ((jobConfigurationDays & Day.Monday) == Day.Monday)
                JobConfigurationDaysMonday = true;
            if ((jobConfigurationDays & Day.Tuesday) == Day.Tuesday)
                JobConfigurationDaysTuesday = true;
            if ((jobConfigurationDays & Day.Wednesday) == Day.Wednesday)
                JobConfigurationDaysWednesday = true;
            if ((jobConfigurationDays & Day.Thursday) == Day.Thursday)
                JobConfigurationDaysThursday = true;
            if ((jobConfigurationDays & Day.Friday) == Day.Friday)
                JobConfigurationDaysFriday = true;
            if ((jobConfigurationDays & Day.Saturday) == Day.Saturday)
                JobConfigurationDaysSaturday = true;
            if ((jobConfigurationDays & Day.Sunday) == Day.Sunday)
                JobConfigurationDaysSunday = true;
            internJobDayChange = false;
        }

        private int jobConfigurationHours;

        public int JobConfigurationHours
        {
            get { return jobConfigurationHours; }
            set
            {
                jobConfigurationHours = value;
                RaisePropertyChanged(nameof(JobConfigurationHours));
            }
        }

        private int jobConfigurationMinutes;

        public int JobConfigurationMinutes
        {
            get { return jobConfigurationMinutes; }
            set
            {
                jobConfigurationMinutes = value;
                RaisePropertyChanged(nameof(JobConfigurationMinutes));
            }
        }

        private int jobConfigurationMaxRuntimeMinutes;

        public int JobConfigurationMaxRuntimeMinutes
        {
            get { return jobConfigurationMaxRuntimeMinutes; }
            set
            {
                jobConfigurationMaxRuntimeMinutes = value;
                RaisePropertyChanged(nameof(JobConfigurationMaxRuntimeMinutes));
            }
        }

        private int jobConfigurationMaxParallelism;

        public int JobConfigurationMaxParallelism
        {
            get { return jobConfigurationMaxParallelism; }
            set
            {
                jobConfigurationMaxParallelism = value;
                RaisePropertyChanged(nameof(JobConfigurationMaxParallelism));
            }
        }

        private string jobConfigurationFileExtensions;

        public string JobConfigurationFileExtensions
        {
            get { return jobConfigurationFileExtensions; }
            set
            {
                jobConfigurationFileExtensions = value;
                RaisePropertyChanged(nameof(JobConfigurationFileExtensions));
            }
        }

        public List<KeyValuePair<int, string>> JobTypeEnum { get; set; }

        public int jobTypeEnumSelected;

        public int JobTypeEnumSelected
        {
            get { return jobTypeEnumSelected; }
            set
            {
                jobTypeEnumSelected = value;
                RaisePropertyChanged(nameof(JobTypeEnumSelected));
            }
        }

        public ObservableCollection<KeyValuePair<Guid, string>> paths;

        public ObservableCollection<KeyValuePair<Guid, string>> Paths
        {
            get { return paths; }
            set
            {
                paths = value;
                RaisePropertyChanged(nameof(Paths));
            }
        }

        public Guid pathsSelected = Guid.Empty;

        public Guid PathsSelected
        {
            get { return pathsSelected; }
            set
            {
                pathsSelected = value;
                RaisePropertyChanged(nameof(PathsSelected));
            }
        }

        public bool includeSubFolders;

        public bool IncludeSubFolders
        {
            get { return includeSubFolders; }
            set
            {
                includeSubFolders = value;
                RaisePropertyChanged(nameof(IncludeSubFolders));
            }
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
                Paths = new ObservableCollection<KeyValuePair<Guid, string>>();
                var paths = await _repositoryFolders.GetPaths();
                Paths.Add(new KeyValuePair<Guid, string>(Guid.Empty, "---"));
                foreach (var item in paths)
                {
                    Paths.Add(new KeyValuePair<Guid, string>(item.Id, item.Path));
                }
                PathsSelected = Guid.Empty;
                if (JobId != Guid.Empty)
                {
                    var jobModel = await _repository.GetJobs(jobId);
                    JobName = jobModel.Name;
                    var jobConfigModel = await _repository.GetJobConfiguration(JobId);
                    if (jobConfigModel != null)
                    {
                        JobConfigurationDays = jobConfigModel.Days;
                        JobConfigurationFileExtensions = jobConfigModel.FileExtensions;
                        JobConfigurationHours = jobConfigModel.Hours;
                        JobConfigurationId = jobConfigModel.Id;
                        JobConfigurationMaxRuntimeMinutes = jobConfigModel.MaxRuntimeMinutes;
                        JobConfigurationMinutes = jobConfigModel.Minutes;
                        JobConfigurationMaxParallelism = jobConfigModel.MaxParallelism;
                    }
                    await OnRefreshCollectPaths();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        private async Task OnRefreshCollectPaths()
        {
            JobCollectPathItems = new ObservableCollection<ViewModels.JobPathView>(await _repository.GetJobCollectPath(JobId));
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
                bool isNew = false;
                Models.Job job;
                Models.JobConfiguration jobConfiguration;
                if (JobId == Guid.Empty)
                {
                    isNew = true;
                    job = new Models.Job
                    {
                        Id = Guid.NewGuid(),
                        JobType = (JobType)JobTypeEnumSelected,
                        Name = JobName,
                    };
                    if (!await _repository.Insert(job))
                        throw new Exception("Could not insert Job");
                } else
                {
                    job = await _repository.GetJobs(JobId);
                    job.Name = JobName;
                    job.JobType = (JobType)JobTypeEnumSelected;
                    if (!await _repository.Update(job))
                        throw new Exception("Could not update Job");
                }
                job = await _repository.GetJobs(job.Id);
                if (isNew)
                {
                    jobConfiguration = new Models.JobConfiguration
                    {
                        Id = Guid.NewGuid(),
                        Days = JobConfigurationDays,
                        FileExtensions = JobConfigurationFileExtensions,
                        Hours = JobConfigurationHours,
                        JobId = job.Id,
                        MaxRuntimeMinutes = JobConfigurationMaxRuntimeMinutes,
                        Minutes = JobConfigurationMinutes,
                        MaxParallelism = JobConfigurationMaxParallelism
                    };
                    if (!await _repository.Insert(jobConfiguration))
                        throw new Exception("Could not insert JobConfiguration");
                } else
                {
                    jobConfiguration = await _repository.GetJobConfiguration(job.Id);
                    jobConfiguration.Days = JobConfigurationDays;
                    jobConfiguration.FileExtensions = JobConfigurationFileExtensions;
                    jobConfiguration.Hours = JobConfigurationHours;
                    jobConfiguration.MaxRuntimeMinutes = JobConfigurationMaxRuntimeMinutes;
                    jobConfiguration.Minutes = JobConfigurationMinutes;
                    jobConfiguration.MaxParallelism = JobConfigurationMaxParallelism;
                    if (!await _repository.Update(jobConfiguration))
                        throw new Exception("Could not update JobConfiguration");
                }
                JobId = job.Id;
                await OnRefresh();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnSave failed to load data");
            }
        }

        private ICommand _addPathCommand;

        public ICommand AddPathCommand
        {
            get
            {
                if (_addPathCommand == null)
                {
                    _addPathCommand = new RelayCommand(
                        p => true,
                        async p => await OnAddPath());
                }
                return _addPathCommand;
            }
        }

        private async Task OnAddPath()
        {
            try
            {
                if (JobId != Guid.Empty)
                {
                    var path = new Models.JobCollectPath
                    {
                        Id = Guid.NewGuid(),
                        CollectPathId = PathsSelected,
                        IncludeSubFolders = IncludeSubFolders,
                        JobId = JobId
                    };
                    if (!await _repository.Insert(path))
                        throw new Exception("Could not insert JobCollectPath");
                    await OnRefreshCollectPaths();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnAddPath failed to add data");
            }
        }

        public async Task DeleteCollectPath(ViewModels.JobPathView path)
        {
            try
            {
                if (path != null)
                {
                    var collectPath = await _repository.GetJobPath(path.JobCollectPathId);
                    if (!await _repository.Delete(collectPath))
                        throw new Exception("Could not delete JobCollectPath");
                    await OnRefreshCollectPaths();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteCollectPath failed to delete data");
            }
        }

    }
}
