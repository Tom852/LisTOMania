using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using LisTOMania.Common.Model.Interfaces;
using Neo4jClient;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LisTOMania.DataAccess.Neo4J
{
    public class N4JTagDataAcess<N4JEntity> : ITagDataAccess<N4JEntity>
        where N4JEntity : ITaggable
    {
        private IGraphClient graphClient;

        public N4JTagDataAcess(IGraphClient fac)
        {
            graphClient = fac;
        }

        public async Task ManageTags(N4JEntity item, IEnumerable<string> tags)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            // Optimaziation: Check if there are changes
            if (await IsUnchanged(item, tags))
            {
                return;
            };

            // Remove Tags from existing
            await graphClient.Cypher
                .Match($"(i:{item.Neo4JLabel})-[relation:HasTag]->(:Tag)")
                .Where((N4JEntity i) => i.Id == item.Id)
                .Delete("relation")
                .ExecuteWithoutResultsAsync();

            foreach (var tagDesignation in tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                // create tag if not exists, then relate
                await graphClient.Cypher
                    .Merge("(t:Tag { Designation: $tagDesignation })")
                    .With("t")
                    .Match($"(i:{item.Neo4JLabel})")
                    .Where((N4JEntity i) => i.Id == item.Id)
                    .Create("(i)-[:HasTag]->(t)")
                    .WithParam("tagDesignation", tagDesignation)
                    .ExecuteWithoutResultsAsync();
            }

            // Garbage Collect Tags // TODO HART TESTEN
            await this.graphClient.Cypher
                .Match("(t:Tag)")
                .Where("NOT (t)<-[:HasTag]-()")
                .Delete("(t)")
                .ExecuteWithoutResultsAsync();
        }

        private async Task<bool> IsUnchanged(N4JEntity item, IEnumerable<string> newTags)
        {
            var currentTags = await graphClient.Cypher
             .Match($"(i:{item.Neo4JLabel})-[relation:HasTag]->(t:Tag)")
             .Where((N4JEntity i) => i.Id == item.Id)
             .Return<N4JTag>("t")
             .ResultsAsync;

            var oldTags = currentTags.Select(t => t.Designation);

            return
                newTags.All(a => oldTags.Any(b => b == a) &&
                oldTags.All(a => newTags.Any(b => a == b)));
        }
    }
}