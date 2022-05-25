using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLogic.Interfaces
{
    public interface ICrudLogic<T>
    {
        Task Create(T model);
        Task Update(T model);
        Task Delete(T model);
        Task<List<T>> Read(T model);
    }
}