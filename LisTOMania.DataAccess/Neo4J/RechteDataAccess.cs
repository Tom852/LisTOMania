using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using Neo4jClient;
using Neo4jClient.Extensions;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace LisTOMania.DataAccess.Neo4J
{
    public class RechteDataAccess : IRechteDataAccess
    {
        private IGraphClient graphClient;

        public RechteDataAccess(IGraphClient fac)
        {
            graphClient = fac;
        }

        public async Task<bool> CanRead(Guid listId, string userName)
        {
            // if i come from a child, i shall be permitted
            var query = await this.graphClient.Cypher
                .Match("(childList:List)-[:SAIO*0..]->(l:List)")
                .Where((N4JList l) => l.Id == listId)
                .Match("(u:User)")
                .Where((N4JUser u) => u.Name == userName)
                .OptionalMatch("(u)-[r2:CanRead]->(childList)")
                .Return<int>("count(r2)")
                .ResultsAsync;

            return query.Single() > 0;
        }

        public async Task<bool> CanEdit(Guid listId, string userName)
        {
            // if i come from a child, i shall be permitted
            var query = await this.graphClient.Cypher
                .Match("(childList:List)-[:SAIO*0..]->(l:List)")
                .Where((N4JList l) => l.Id == listId)
                .Match("(u:User)")
                .Where((N4JUser u) => u.Name == userName)
                .OptionalMatch("(u)-[r2:CanEdit]->(childList)")
                .Return<int>("count(r2)")
                .ResultsAsync;

            return query.Single() > 0;
        }

        public async Task<bool> HasPermit(string userName, string permissionType, Guid listId)
        {
            var query = await this.graphClient.Cypher
                .Match($"(u:User)-[:{permissionType}]->(l:List)")
                .Where((N4JList l) => l.Id == listId)
                .AndWhere((N4JUser u) => u.Name == userName)
                .Return<int>("count(u)")
                .ResultsAsync;

            return query.Single() == 1;
        }
    }
}