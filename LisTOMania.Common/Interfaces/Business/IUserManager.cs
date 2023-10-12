using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;

namespace LisTOMania.Common.Interfaces.Business
{
    public interface IUserManager
    {
        Task Create(UserDto user);

        Task Delete(Guid id);

        Task Edit(UserDto user);

        Task<UserDto?> GetUserIfCredentialsCorrect(string user, string password);

        Task<UserDto?> Get(Guid id);

        Task<IEnumerable<UserDto?>> GetAll();
    }
}