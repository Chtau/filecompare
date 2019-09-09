using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.DAL
{
    public class DBContext : IDBContext
    {
        private readonly Internal.ILogger _logger;
        private readonly Internal.ISettings _settings;
        internal SQLiteAsyncConnection DB = null;

        public DBContext()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _settings = (Internal.ISettings)Bootstrap.Instance.Services.GetService(typeof(Internal.ISettings));
            Connect().GetAwaiter();
        }

        public async Task Connect()
        {
            DB = new SQLiteAsyncConnection(_settings.DBPath);
            await OnBuildModel();
        }

        private async Task OnBuildModel()
        {
            await DB.CreateTableAsync<Features.Folders.Models.CollectPath>();
            await DB.CreateTableAsync<Features.Jobs.Models.Job>();
            await DB.CreateTableAsync<Features.Jobs.Models.JobCollectPath>();
            await DB.CreateTableAsync<Features.Jobs.Models.JobConfiguration>();
            await DB.CreateTableAsync<Features.JobService.Models.PathCompareValue>();
            await DB.CreateTableAsync<Features.JobService.Models.DuplicateValue>();
            await DB.CreateTableAsync<Features.JobService.Models.PathDuplicate>();
            await DB.CreateTableAsync<Features.JobService.Models.DuplicateResultProgress>();
            await DB.CreateTableAsync<Features.JobService.Models.DuplicateResultProgressIndex>();
            await DB.CreateTableAsync<Features.Jobs.Models.JobConfigurationDuplicates>();
        }

        public SQLiteAsyncConnection Instance
        {
            get
            {
                return DB;
            }
        }
    }
}
