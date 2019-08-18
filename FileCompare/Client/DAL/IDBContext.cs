using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.DAL
{
    public interface IDBContext
    {
        Task Connect();
        SQLiteAsyncConnection Instance { get; }
    }
}
