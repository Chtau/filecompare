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
            if (internJobDayChange == false)
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
        private bool internJobDayChange = false;
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
        public int jobTypeEnumSelected = 0;
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
                if (JobId != Guid.Empty)
                    JobCollectPathItems = new ObservableCollection<ViewModels.JobPathView>(await _repository.GetJobCollectPath(JobId));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

    }
}
