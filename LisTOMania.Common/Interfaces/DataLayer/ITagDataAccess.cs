using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using LisTOMania.Common.Model.Interfaces;

namespace LisTOMania.Common.Interfaces.DataLayer
{
    public interface ITagDataAccess<N4JEntity>
        where N4JEntity : ITaggable

    {
        Task ManageTags(N4JEntity item, IEnumerable<string> tags);
    }
}