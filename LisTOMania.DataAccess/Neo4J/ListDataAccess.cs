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
    public class N4JListDataAccess : IListDataAccess
    {
        private IGraphClient graphClient;
        private readonly IItemDataAccess itemDataAccess;

        public N4JListDataAccess(IGraphClient fac, IItemDataAccess itemDataAccess)
        {
            graphClient = fac;
            this.itemDataAccess = itemDataAccess;
        }

        public async Task ClearAll(Guid listId, bool onlyDone)
        {
            var query = graphClient.Cypher
                .Match("(l:List)")
                .Where((N4JList l) => l.Id == listId)
                .OptionalMatch("(l)<-[:BelongsTo]-(i:Item)");

            if (onlyDone)
            {
                query = query
                    .Where((N4JItem i) => i.IsDone == true);
            }

            query = query.OptionalMatch("(l)-[:SAIO*1..]->(otherList:List)<-[:BelongsTo]-(incluededItems:Item)");
            if (onlyDone)
            {
                query = query
                    .Where((N4JItem incluededItems) => incluededItems.IsDone == true);
            }

            //var o = query
            //    .DetachDelete("i")
            //    .DetachDelete("incluededItems").Query.DebugQueryText;
            await query
                .DetachDelete("i")
                .DetachDelete("incluededItems")
                .ExecuteWithoutResultsAsync();
        }

        /* Removed weil ich doppelte logik drinhab und es auch mit den included items schwerer wrid, unhd für die multiple eh pro item in request her muss. drum mach ich ein foreach auf item edit.- performacne scheint aber ok, aber falls net hätt ichs hier noch
        public async Task MarkAllAs(Guid listId, bool isDone)
        {
            await graphClient.Cypher
                .Match("(l:List)<-[:BelongsTo]-(i:Item)")
                .Where((N4JList l) => l.Id == listId)
                .AndWhere((N4JItem i) => i.IsRepeatable == false)
                .Set("i.IsDone = $isDone")
                .WithParam("isDone", isDone)
                .ExecuteWithoutResultsAsync();

            if (isDone)
            {
                // todo: dass heir diese logik ist, ist schlecht. gefällt mir nicht. v.a. der text ist jetzt 2x, ist auch im item manager wo es eig hingehört.
                var repeatableItems = await graphClient.Cypher
                    .Match("(l:List)<-[:BelongsTo]-(i:Item)")
                    .Where((N4JList l) => l.Id == listId)
                    .AndWhere((N4JItem i) => i.IsRepeatable == true)
                    .Return<N4JItem>("i")
                    .ResultsAsync;

                foreach (var repItem in repeatableItems)
                {
                    var additionalText = repItem.AdditionalText;
                    if (!string.IsNullOrWhiteSpace(additionalText))
                    {
                        additionalText += "\n";
                    }
                    additionalText += $"RepetableItem was marked done at: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";

                    await graphClient.Cypher
                        .Match("(i:Item)")
                        .Where((N4JItem i) => i.Id == repItem.Id)
                        .Set("i.AdditionalText = $additionalText")
                        .WithParam("additionalText", additionalText)
                        .ExecuteWithoutResultsAsync();
                }
            }
        }
        */

        public async Task Create(ListDto listDto)
        {
            var list = new N4JList()
            {
                Id = Guid.NewGuid(),
                Designation = listDto.Designation,
                LastAccess = DateTime.MinValue,
            };

            var query = this.graphClient.Cypher
                .Create("(l:List $props)")
                .WithParam("props", list);

            await query.ExecuteWithoutResultsAsync();

            await AssignSaioRelations(list, listDto);
            await AssignRechteRelations(list, listDto);
        }

        public async Task RefreshLastAccess(Guid listid)
        {
            await this.graphClient.Cypher
                .Match("(n:List)")
                .Where((N4JList n) => n.Id == listid)
                .Set("n.LastAccess = $nnnn")
                .WithParam("nnnn", DateTime.Now)
                .ExecuteWithoutResultsAsync();
        }

        public async Task Edit(ListDto listDto)
        {
            var list = new N4JList()
            {
                Id = listDto.Id.Value,
                Designation = listDto.Designation,
                LastAccess = listDto.LastAccess,
            };

            await this.graphClient.Cypher
                .Match("(n:List)")
                .Where((N4JList n) => n.Id == list.Id)
                .Set("n = $newList")
                .WithParam("newList", list)
                .ExecuteWithoutResultsAsync();

            await AssignRechteRelations(list, listDto);
            await AssignSaioRelations(list, listDto);
        }

        private async Task AssignRechteRelations(N4JList list, ListDto listDto)
        {
            var canReadUserNames = listDto.CanRead?.Select(d => d.Name).ToList() ?? new List<string>();
            var canEditUserNames = listDto.CanEdit?.Select(d => d.Name).ToList() ?? new List<string>();

            await graphClient.Cypher
                .Match("(l:List)<-[relation]-(u:User)")
                .Where((N4JList l) => l.Id == list.Id)
                .AndWhere("(type(relation) = 'CanRead' OR type(relation) = 'CanEdit')") //funny wenn man hier die klammer vergisst... :D
                .Delete("relation")
                .ExecuteWithoutResultsAsync();

            foreach (var reader in canReadUserNames)
            {
                await graphClient.Cypher
                    .Match("(l:List)")
                    .Where((N4JList l) => l.Id == list.Id)
                    .Match("(u:User)")
                    .Where((N4JUser u) => u.Name == reader)
                    .Merge("(l)<-[:CanRead]-(u)")
                    .ExecuteWithoutResultsAsync();
            }

            foreach (var editor in canEditUserNames)
            {
                await graphClient.Cypher
                    .Match("(l:List)")
                    .Where((N4JList l) => l.Id == list.Id)
                    .Match("(u:User)")
                    .Where((N4JUser u) => u.Name == editor)
                    .Merge("(l)<-[:CanEdit]-(u)")
                    .ExecuteWithoutResultsAsync();
            }
        }

        public async Task<ListDto?> Get(Guid id)
        {
            var query2 = this.graphClient.Cypher
                .Match("(root:List)")
                .Where((N4JList root) => root.Id == id)
                .OptionalMatch("(root)-[:SAIO]->(linked:List)")
                .OptionalMatch("(directItem:Item)-[:BelongsTo]->(root)")
                .OptionalMatch("(root)-[:SAIO*1..]->(otherList:List)<-[:BelongsTo]-(itemInOtherList:Item)")
                .OptionalMatch("(root)-[:HasTag]->(tag:Tag)")
                .OptionalMatch("(root)<-[:CanRead]-(reader:User)")
                .OptionalMatch("(root)<-[:CanEdit]-(editor:User)")

                //.Match("(tag:Tag)<-[:HasTag]-(item)")
                .Return((root, linked, directItem, itemInOtherList, tag, reader, editor) => new
                {
                    Root = root.As<N4JList>(),
                    Linked = linked.CollectAsDistinct<N4JList>(),
                    Items = directItem.CollectAsDistinct<N4JItem>(),
                    ItemsIncluded = itemInOtherList.CollectAsDistinct<N4JItem>(),
                    Tags = root.CollectAsDistinct<N4JTag>(),
                    Readers = reader.CollectAsDistinct<N4JUser>(),
                    Editors = editor.CollectAsDistinct<N4JUser>(),
                });

            var queryResult = (await query2.ResultsAsync).SingleOrDefault();

            if (queryResult is null)
            {
                return null;
            }

            // todo: paralliisieren? glaub gar net nötig
            List<ItemDto> itemsFinalized = new();
            foreach (var item in queryResult.Items.Union(queryResult.ItemsIncluded))
            {
                var i = await itemDataAccess.Get(item.Id);
                itemsFinalized.Add(i);
            }

            var dtoresult = new ListDto()
            {
                Id = queryResult.Root.Id,
                Items = itemsFinalized,
                LastAccess = queryResult.Root.LastAccess,
                Designation = queryResult.Root.Designation,
                ShowsAlsoItemsOf = queryResult.Linked.Select(l => new ListDto()
                {
                    Id = l.Id,
                    Designation = l.Designation
                }).ToList(),
                CanRead = queryResult.Readers.Select(u => new UserDto(u)),
                CanEdit = queryResult.Editors.Select(u => new UserDto(u))
            };

            return dtoresult;
        }

        public async Task<IEnumerable<ListDto>> GetAll()
        {
            // simple variant when performance is an issue:
            //var query = this.graphClient.Cypher
            //    .Match("(n:List)")
            //    .Return<N4JList>("(n)");

            var query = this.graphClient.Cypher
                .Match("(l:List)")
                .OptionalMatch("(i:Item)-[:BelongsTo]->(l)")
                .Return((l, i) => new
                {
                    List = l.As<N4JList>(),
                    Items = i.CollectAsDistinct<N4JItem>(),
                });

            var result = await query.ResultsAsync;
            return result.Select(r => new ListDto()
            {
                Id = r.List.Id,
                Designation = r.List.Designation,
                LastAccess = r.List.LastAccess,
                Items = r.Items.Select(n4ji => new ItemDto(n4ji)).ToList(), // bit tricky if this is used elsewhere. should maybe make a ctor n4j -> dto
            });
        }

        public async Task<IEnumerable<ListDto>> GetAllForUser(string user)
        {
            // simple variant when performance is an issue:
            //var query = this.graphClient.Cypher
            //    .Match("(n:List)")
            //    .Return<N4JList>("(n)");

            var query = this.graphClient.Cypher
                .Match("(l:List)<-[relation]-(u:User)")
                .Where((N4JUser u) => u.Name == user)
                .AndWhere("type(relation) = 'CanRead' OR type(relation) = 'CanEdit'")
                .OptionalMatch("(i:Item)-[:BelongsTo]->(l)")
                .Return((l, i) => new
                {
                    List = l.As<N4JList>(),
                    Items = i.CollectAsDistinct<N4JItem>(),
                });

            var result = await query.ResultsAsync;
            return result.Select(r => new ListDto()
            {
                Id = r.List.Id,
                Designation = r.List.Designation,
                LastAccess = r.List.LastAccess,
                Items = r.Items.Select(n4ji => new ItemDto(n4ji)).ToList(), // bit tricky if this is used elsewhere. should maybe make a ctor n4j -> dto
            });
        }

        public async Task Remove(Guid id)
        {
            await this.graphClient.Cypher
                .Match("(l:List)")
                .Where((N4JList l) => l.Id == id)
                .DetachDelete("l")
                .ExecuteWithoutResultsAsync();
        }

        private async Task AssignSaioRelations(N4JList list, ListDto listDto)
        {
            var showsAlsoITemsOfIds = listDto.ShowsAlsoItemsOf.Select(l => l.Id.Value);

            await graphClient.Cypher
                .Match("(l:List)-[relation:SAIO]->(linked:List)")
                .Where((N4JList l) => l.Id == list.Id)
                .Delete("relation")
                .ExecuteWithoutResultsAsync();

            foreach (var saoid in showsAlsoITemsOfIds)
            {
                await graphClient.Cypher
                    .Match("(l:List)")
                    .Where((N4JList l) => l.Id == list.Id)
                    .Match("(toLink:List)")
                    .Where((N4JList toLink) => toLink.Id == saoid)
                    .Merge("(l)-[:SAIO]->(toLink)")
                    .ExecuteWithoutResultsAsync();
            }
        }
    }
}