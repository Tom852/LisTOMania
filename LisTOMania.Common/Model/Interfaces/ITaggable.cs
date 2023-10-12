namespace LisTOMania.Common.Model.Interfaces
{
    public interface ITaggable
    {
        Guid Id { get; set; }

        string Neo4JLabel { get; }
    }
}