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

        public JobConfigurationViewModel(Guid jobId)
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IJobRepository)Bootstrap.Instance.Services.GetService(typeof(IJobRepository));

            JobId = jobId;
        }

        private ObservableCollection<Models.JobCollectPath> jobCollectPathItems;
        public ObservableCollection<Models.JobCollectPath> JobCollectPathItems
        {
            get { return jobCollectPathItems; }
            set
            {
                jobCollectPathItems = value;
                RaisePropertyChanged("JobCollectPathItems");
            }
        }

        private Guid jobId = Guid.Empty;
        public Guid JobId
        {
            get { return jobId; }
            set
            {
                jobId = value;
                RaisePropertyChanged("JobId");
            }
        }

        private string jobName;
        public string JobName
        {
            get { return jobName; }
            set
            {
                jobName = value;
                RaisePropertyChanged("JobName");
            }
        }

        private JobType jobType;
        public JobType JobType
        {
            get { return jobType; }
            set
            {
                jobType = value;
                RaisePropertyChanged("JobType");
            }
        }

        private Guid jobConfigurationId;
        public Guid JobConfigurationId
        {
            get { return jobConfigurationId; }
            set
            {
                jobConfigurationId = value;
                RaisePropertyChanged("Job");
            }
        }

        private Day jobConfigurationDays;
        public Day JobConfigurationDays
        {
            get { return jobConfigurationDays; }
            set
            {
                jobConfigurationDays = value;
                RaisePropertyChanged("JobConfigurationDays");
            }
        }

        private int jobConfigurationHours;
        public int JobConfigurationHours
        {
            get { return jobConfigurationHours; }
            set
            {
                jobConfigurationHours = value;
                RaisePropertyChanged("JobConfigurationHours");
            }
        }

        private int jobConfigurationMinutes;
        public int JobConfigurationMinutes
        {
            get { return jobConfigurationMinutes; }
            set
            {
                jobConfigurationMinutes = value;
                RaisePropertyChanged("JobConfigurationMinutes");
            }
        }

        private int jobConfigurationMaxRuntimeMinutes;
        public int JobConfigurationMaxRuntimeMinutes
        {
            get { return jobConfigurationMaxRuntimeMinutes; }
            set
            {
                jobConfigurationMaxRuntimeMinutes = value;
                RaisePropertyChanged("JobConfigurationMaxRuntimeMinutes");
            }
        }

        private string jobConfigurationFileExtensions;
        public string JobConfigurationFileExtensions
        {
            get { return jobConfigurationFileExtensions; }
            set
            {
                jobConfigurationFileExtensions = value;
                RaisePropertyChanged("JobConfigurationFileExtensions");
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
                if (JobId != Guid.Empty)
                    JobCollectPathItems = new ObservableCollection<Models.JobCollectPath>(await _repository.GetJobCollectPath(JobId));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

    }
}
