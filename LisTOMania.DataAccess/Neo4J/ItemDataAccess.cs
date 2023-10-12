using LisTOMania.Common.Interfaces.DataLayer;
using LisTOMania.Common.Model;
using LisTOMania.Common.Model.DataBase;
using Neo4jClient;
using System.Collections.Generic;

namespace LisTOMania.DataAccess.Neo4J
{
    public class N4JItemDataAcess : IItemDataAccess
    {
        private IGraphClient graphClient;
        private readonly ITagDataAccess<N4JItem> tagAccess;

        public N4JItemDataAcess(IGraphClient fac, ITagDataAccess<N4JItem> tagAccess)
        {
            graphClient = fac;
            this.tagAccess = tagAccess;
        }

        public async Task Add(Guid listId, ItemDto itemDto)
        {
            var item = new N4JItem()
            {
                Designation = itemDto.Designation,
                AdditionalText = itemDto.AdditionalText,
                DoneAt = itemDto.DoneAt,
                Id = Guid.NewGuid(),
                IsDone = itemDto.IsDone,
                IsRepeatable = itemDto.IsRepeatable,
                Prio = itemDto.Prio,
            };

            await graphClient.Cypher
                .Match("(l:List)")
                .Where((N4JList l) => l.Id == listId)
                .Create("(newItem:Item $itemParam)-[:BelongsTo]->(l)")
                .WithParam("itemParam", item)
                .ExecuteWithoutResultsAsync();

            await this.tagAccess.ManageTags(item, itemDto.Tags);
        }

        public async Task Edit(ItemDto itemDto)
        {
            var item = new N4JItem()
            {
                Designation = itemDto.Designation,
                AdditionalText = itemDto.AdditionalText,
                DoneAt = itemDto.DoneAt,
                Id = itemDto.Id.Value,
                IsDone = itemDto.IsDone,
                IsRepeatable = itemDto.IsRepeatable,
                Prio = itemDto.Prio,
            };
            await this.graphClient.Cypher
                .Match("(i:Item)")
                .Where((N4JItem i) => i.Id == item.Id)
                .Set("i = $item")
                .WithParam("item", item)
                .ExecuteWithoutResultsAsync();

            await this.tagAccess.ManageTags(item, itemDto.Tags);
        }

        public async Task<ItemDto?> Get(Guid itemId)
        {
            var queryResult = await this.graphClient.Cypher
                .Match("(i:Item)-[:BelongsTo]->(l:List)")
                .Where((N4JItem i) => i.Id == itemId)
                .OptionalMatch("(i)-[:HasTag]->(tag:Tag)")
                .Return((i, tag, l) => new
                {
                    Item = i.As<N4JItem>(),
                    Tags = tag.CollectAsDistinct<N4JTag>(),
                    ContainingList = l.As<N4JList>(),
                })
                .ResultsAsync;

            var e = queryResult.SingleOrDefault();

            if (e is null)
            {
                return null;
            }

            return new ItemDto()
            {
                Id = e.Item.Id,
                Designation = e.Item.Designation,
                AdditionalText = e.Item.AdditionalText,
                DoneAt = e.Item.DoneAt,
                IsDone = e.Item.IsDone,
                IsRepeatable = e.Item.IsRepeatable,
                Prio = e.Item.Prio,
                Tags = e.Tags.Select(t => t.Designation).ToList(),
                ContainingListId = e.ContainingList.Id,
                ContainingListDesignation = e.ContainingList.Designation,
            };
        }

        public async Task Remove(Guid itemId)
        {
            await this.graphClient.Cypher
                .Match("(i:Item)")
                .Where((N4JItem i) => i.Id == itemId)
                .DetachDelete("i")
                .ExecuteWithoutResultsAsync();
        }

        public async Task ResetRepeatableItems()
        {
            await this.graphClient.Cypher
                .Match("(i:Item)")
                .Where((N4JItem i) => i.IsRepeatable == true)
                .Set("i.IsDone = false")
                .ExecuteWithoutResultsAsync();
        }
    }
}