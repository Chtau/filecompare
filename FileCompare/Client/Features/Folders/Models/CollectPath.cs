using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Folders.Models
{
    public class CollectPath
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Path { get; set; }
        public DateTime Added { get; set; }
        public DateTime? LastCheck { get; set; }
        public int FilesFound { get; set; }
        public int SubFoldersFound { get; set; }
        public bool CheckSubFolders { get; set; }
    }
}
