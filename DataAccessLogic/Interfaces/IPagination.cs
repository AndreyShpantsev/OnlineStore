using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLogic.Interfaces
{
    public interface IPagination<TModel>
    {
        Task<List<TModel>> GetPage(int pageNumber, TModel model);
        Task<int> GetCount(TModel model);
    }
}