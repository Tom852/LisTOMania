using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.DataLayer
{
    public interface IUserDataAccess
    {
        Task Delete(Guid id);

        Task Edit(N4JUser model);

        Task Create(N4JUser model);

        Task<N4JUser?> Get(string user, string password);

        Task<IEnumerable<N4JUser?>> GetAll();

        Task<N4JUser> Get(Guid id);
    }
}