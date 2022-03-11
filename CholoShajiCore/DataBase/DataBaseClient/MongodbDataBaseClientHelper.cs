using CholoShajiCore.CoreInterfaces;
using CholoShajiCore.DataBase.Conditions;
using CholoShajiCore.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CholoShajiCore.DataBase.DataBaseClient
{
    public static class MongodbDataBaseClientHelper
    {
        public static BsonDocument GetBsonDocumentItem<T>(T item, string x) where T : class, IARepositoryModel
        {
            var newItem = item.ToBsonDocument();
            if (newItem.Contains("_id"))
                newItem.Remove("_id");
            newItem.Add("_id", x);
            return newItem;
        }

        public static FilterDefinition<BsonDocument> GetIdFilter(string id) 
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", id);
            return filter;
        }

        public static ProjectionDefinition<BsonDocument> GetBuilderUsingProjectionDefinition<T>() where T : class, IARepositoryModel
        {
            var builder = Builders<BsonDocument>.Projection.Exclude("Id");
            if (typeof(T).GetProperty("Id") == null)
                builder = Builders<BsonDocument>.Projection.Exclude("_id");
            return builder;
        }

        public static BsonDocument GetBsonDocumentQuery(ICondition condition)
        {
            var mongoQuery = condition.GetMongoQuery();
            var query = new BsonDocument(mongoQuery);
            return query;
        }
    }
}