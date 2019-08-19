using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Folders
{
    public class FoldersContentViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IFolderRepository _repository;

        public FoldersContentViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IFolderRepository)Bootstrap.Instance.Services.GetService(typeof(IFolderRepository));
        }

        private ObservableCollection<Models.CollectPath> collectPathItems;
        public ObservableCollection<Models.CollectPath> CollectPathItems
        {
            get { return collectPathItems; }
            set
            {
                collectPathItems = value;
                RaisePropertyChanged("CollectPathItems");
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
                CollectPathItems = new ObservableCollection<Models.CollectPath>(await _repository.GetPaths());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        public async Task<bool> InsertCollectPath(string folder)
        {
            try
            {
                if (Internal.StaticFolders.IsValidFolder(folder))
                {
                    var model = new Models.CollectPath
                    {
                        Added = DateTime.UtcNow,
                        FilesFound = 0,
                        Id = Guid.NewGuid(),
                        LastCheck = null,
                        Path = folder,
                        SubFoldersFound = 0
                    };
                    await _repository.Insert(model);
                    return true;
                }
                else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> DeleteCollectPath(Models.CollectPath collectPath)
        {
            try
            {
                if (collectPath != null)
                    return await _repository.Delete(collectPath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
    }
}
