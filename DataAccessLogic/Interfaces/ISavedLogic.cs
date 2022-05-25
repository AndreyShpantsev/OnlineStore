using DataAccessLogic.DatabaseModels;
using System.Threading.Tasks;

namespace DataAccessLogic.Interfaces
{
    public interface ISavedLogic
    {
        Task Create(User user);
        Task<SavedList> Read(User user);
        Task Add(User user, Antiques savedAntiques);
        Task Remove(User user, Antiques savedAntiques);
    }
}