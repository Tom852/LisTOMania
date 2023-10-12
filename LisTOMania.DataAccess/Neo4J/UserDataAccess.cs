using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using Neo4jClient;
using System.Collections.Generic;

namespace LisTOMania.DataAccess.Neo4J
{
    public class N4JUserDataAcess : IUserDataAccess
    {
        private IGraphClient graphClient;

        public N4JUserDataAcess(IGraphClient fac)
        {
            graphClient = fac;
        }

        public async Task Create(N4JUser model)
        {
            await this.graphClient.Cypher
                .Create("(u:User)")
                .Set("u = $model")
                .WithParam("model", model)
                .ExecuteWithoutResultsAsync();
        }

        public async Task Edit(N4JUser model)
        {
            await this.graphClient.Cypher
                .Match("(u:User)")
                .Where((N4JUser u) => u.Id == model.Id)
                .Set("u = $model")
                .WithParam("model", model)
                .ExecuteWithoutResultsAsync();
        }

        public async Task Delete(Guid nodeId)
        {
            await this.graphClient.Cypher
                .Match("(u:User)")
                .Where((N4JUser u) => u.Id == nodeId)
                .DetachDelete("u")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<N4JUser?> Get(string user, string hash)
        {
            var queryResult = await this.graphClient.Cypher
                .Match("(n:User)")
                .Where((N4JUser n) => n.Name == user && n.Password == hash)
                .Return<N4JUser>("(n)")
                .ResultsAsync;

            var e = queryResult.SingleOrDefault();

            return e;
        }

        public async Task<N4JUser?> Get(Guid id)
        {
            var queryResult = await this.graphClient.Cypher
                .Match("(n:User)")
                .Where((N4JUser n) => n.Id == id)
                .Return<N4JUser>("(n)")
                .ResultsAsync;

            var e = queryResult.SingleOrDefault();

            return e;
        }

        public async Task<IEnumerable<N4JUser?>> GetAll()
        {
            var queryResult = await this.graphClient.Cypher
                .Match("(n:User)")
                .Return<N4JUser>("(n)")
                .ResultsAsync;

            return queryResult;
        }
    }
}