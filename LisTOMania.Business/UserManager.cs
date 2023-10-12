using LisTOMania.Common.Interfaces.Business;
using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using System.Security.Cryptography;
using System.Text;

namespace LisTOMania.Business
{
    public class UserManager : IUserManager
    {
        private readonly IUserDataAccess dataAccess;

        public UserManager(IUserDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<UserDto?> GetUserIfCredentialsCorrect(string userName, string password)
        {
            var hash = ComputeSha256Hash(password);
            var data = await this.dataAccess.Get(userName, hash);
            if (data is null)
            {
                return null;
            }
            var dto = new UserDto(data);
            return dto;
        }

        public async Task<IEnumerable<UserDto?>> GetAll()
        {
            var data = await this.dataAccess.GetAll();
            return data.Select(d => new UserDto(d)).ToList();
        }

        public async Task Create(UserDto user)
        {
            var model = new N4JUser(user);
            model.Password = ComputeSha256Hash(user.Password);
            model.Id = Guid.NewGuid();
            await this.dataAccess.Create(model);
        }

        public async Task Edit(UserDto user)
        {
            var model = new N4JUser(user);
            model.Password = ComputeSha256Hash(user.Password);
            await this.dataAccess.Edit(model);
        }

        public async Task Delete(Guid id)
        {
            await this.dataAccess.Delete(id);
        }

        public async Task<UserDto> Get(Guid id)
        {
            var user = await this.dataAccess.Get(id);
            return new UserDto(user);
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData + "lmaa"));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}