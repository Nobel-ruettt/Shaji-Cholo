using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CholoShajiCore.Config;
using CholoShajiCore.CoreInterfaces;
using CholoShajiCore.DataBase.Conditions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CholoShajiCore.DataBase.DataBaseClient
{
    [Export("MongodbDataBaseClient", typeof(IDataBaseClient))]
    [Shared]
    public class MongodbDataBaseClient : IDataBaseClient
    {
        public class BsonClassMapHelper
        {

            public void RegisterType<T>()
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    BsonClassMap.RegisterClassMap<T>();
                }
            }

            public void Register(Type type)
            {
                var method = GetType().GetMethod("RegisterType");
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(this, new object[] { });
            }
        }
        private  MongoClient _client { get; set; }
        private  readonly object LockObject = new object();
        public  MongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (LockObject)
                    {
                        if (_client == null)
                        {
                            _client = Create();
                            CreateMappings();
                        }
                    }
                }

                return _client;
            }
            set => _client = value;
        }
        // Need To Understand it latter
        private void CreateMappings()
        {
            var assembly = typeof(IARepositoryModel).Assembly;
            var repositoryInterface = typeof(IARepositoryModel);
            var types = assembly.GetTypes()
                .Where(p => repositoryInterface.IsAssignableFrom(p));
            var bsonClassMapHelper = new BsonClassMapHelper();
            foreach (var type in types.Where(t => t.IsClass && t.IsAbstract == false))
            {
                bsonClassMapHelper.Register(type);
                var properties = type.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    bsonClassMapHelper.Register(propertyInfo.GetType());
                }
            }
        }
        private MongoClient Create()
        {
            return new MongoClient(ConfigurationHelper.Instance.MongodbDataBaseSetting.ConnectionString);
        }
        public bool ExistIndex(string index)
        {
            var databaseNames = Client.ListDatabases();
            var cancelToken = new CancellationToken();
            while (databaseNames.MoveNext(cancelToken))
            {
                var d = databaseNames.Current.ToList();
                foreach (var db in d)
                    if (db.Values.ToList()[0] == index)
                        return true;
            }
            return false;
        }
        public void CreateIndexKey<T>(string index, string fieldkey, bool isAssending = true) where T : class, IARepositoryModel
        {
            try
            {
                var collection = Client.GetDatabase(index).GetCollection<T>(typeof(T).Name.ToLower());
                collection.Indexes
                    .CreateOne(new CreateIndexModel<T>(new BsonDocumentIndexKeysDefinition<T>(new BsonDocument(fieldkey, isAssending ? 1 : -1))));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition) where T : class, IARepositoryModel
        {
            var items = await Task.Run(() => GetItemsByCondition<T>(index, condition, 0, int.MaxValue));
            return items;
        }
        public async Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition, int from, int size) where T : class, IARepositoryModel
        {
            var items = await Task.Run(() => GetItemsByCondition<T>(index, condition, from, size));
            return items;
        }
        public List<T> GetItemsByCondition<T>(string index, ICondition condition, int from, int size) where T : class, IARepositoryModel
        {
            var builder = MongodbDataBaseClientHelper.GetBuilderUsingProjectionDefinition<T>();
            var query = MongodbDataBaseClientHelper.GetBsonDocumentQuery(condition);
            var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
            var result = collection.Find(query).Project<T>(builder)
                .Skip(from)
                .Limit(size)
                .ToList();
            return result;
        }

        public async Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition, int from, int size, string sortField, bool isAssending) where T : class, IARepositoryModel
        {
            var items = await Task.Run(() =>
                GetItemsByCondition<T>(index, condition, from, size, sortField, isAssending));
            return items;
        }
        public List<T> GetItemsByCondition<T>(string index, ICondition condition, int from, int size, string sortField, bool isAssending) where T : class, IARepositoryModel
        {
            var builder = MongodbDataBaseClientHelper.GetBuilderUsingProjectionDefinition<T>();
            var query = MongodbDataBaseClientHelper.GetBsonDocumentQuery(condition);
            var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
            var result = collection.Find(query).Project<T>(builder)
                .Skip(from)
                .Limit(size)
                .Sort(new BsonDocument(sortField, isAssending ? 1 : -1))
                .ToList();
            return result;
        }

        public async Task<T> GetItemByIdAsync<T>(string index, string id) where T : class, IARepositoryModel
        {
            var item = await Task.Run(() => GetItemById<T>(index, id));
            return item;
        }
        public T GetItemById<T>(string index, string id) where T : class, IARepositoryModel
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", id.ToLower());
            var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
            var bsonDocument = collection.Find<BsonDocument>(filter).FirstOrDefault<BsonDocument>();
            if (bsonDocument == null)
            {
                return null;
            }
            if (typeof(T).GetProperty("Id") == null)
                bsonDocument.Remove("_id");

            T result = BsonSerializer.Deserialize<T>(bsonDocument.ToJson());
            return result;
        }

        public async Task<long> CountItemsByConditionAsync<T>(string index, ICondition condition) where T : class, IARepositoryModel
        {
            var ans = await Task.Run(() => CountItemsByCondition<T>(index, condition));
            return ans;
        }
        public long CountItemsByCondition<T>(string index, ICondition condition) where T : class, IARepositoryModel
        {
            var querydoc = new BsonDocument(condition.GetMongoQuery());
            var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
            return collection.CountDocuments(querydoc);
        }
        public async Task<bool> SaveAsync<T>(string index, T item) where T : class, IARepositoryModel
        {
            var result = await Task.Run(() => Save(index, item));
            return result;
        }
        public bool Save<T>(string index, T item) where T : class, IARepositoryModel
        {
            var isSuccessful = true;
            try
            {
                var id = item.Id.ToLower();
                var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
                var newItem = MongodbDataBaseClientHelper.GetBsonDocumentItem(item, id);
                var builder = Builders<BsonDocument>.Filter;
                var filter = MongodbDataBaseClientHelper.GetIdFilter(id);
                collection.ReplaceOne(filter, newItem, new ReplaceOptions {IsUpsert = true});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                isSuccessful = false;
            }
            return isSuccessful;
        }

        public async Task<bool> SaveAsync<T>(string index, List<T> items) where T : class, IARepositoryModel
        {
            var result = await Task.Run(() => Save(index, items));
            return result;
        }
        public bool Save<T>(string index, List<T> items) where T : class, IARepositoryModel
        {
            if (items.Count <= 0)
                return true;

            bool isSuccessful = true;
            try
            {
                var collection = Client.GetDatabase(index).GetCollection<BsonDocument>(typeof(T).Name.ToLower());
                var bulkOps = new List<WriteModel<BsonDocument>>();
                foreach (var obj in items)
                {
                    var id = obj.Id.ToLower();
                    var newobj = MongodbDataBaseClientHelper.GetBsonDocumentItem(obj, id);
                    var filter = MongodbDataBaseClientHelper.GetIdFilter(id);
                    var upsertOne = new ReplaceOneModel<BsonDocument>(filter, newobj) { IsUpsert = true };
                    bulkOps.Add(upsertOne);
                }
                collection.BulkWrite(bulkOps);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                isSuccessful = false;
            }
            return isSuccessful;
        }
    }
}
