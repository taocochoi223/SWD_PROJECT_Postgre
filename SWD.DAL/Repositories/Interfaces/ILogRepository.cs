using SWD.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.DAL.Repositories.Interfaces
{
    public interface ILogRepository
    {
        // Hàm này nhận vào một đối tượng SystemLog và lưu xuống DB
        Task AddLogAsync(SystemLog log);
    }
}
