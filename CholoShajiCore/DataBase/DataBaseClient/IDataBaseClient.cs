using System.Collections.Generic;
using System.Threading.Tasks;
using CholoShajiCore.CoreInterfaces;
using CholoShajiCore.DataBase.Conditions;
using CholoShajiCore.Model;
using CholoShajiCore.Model.Filters;

namespace CholoShajiCore.DataBase.DataBaseClient
{
    public interface IDataBaseClient
    {
        bool ExistIndex(string index);
        void CreateIndexKey<T>(string index, string fieldkey, bool isAssending = true) where T : class, IARepositoryModel;
        #region AsyncMethods
        Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition) where T : class, IARepositoryModel;
        Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition, int from, int size) where T : class, IARepositoryModel;
        Task<List<T>> GetItemsByConditionAsync<T>(string index, ICondition condition, int from, int size, string sortField, bool isAssending) where T : class, IARepositoryModel;
        Task<T> GetItemByIdAsync<T>(string index, string id) where T : class, IARepositoryModel;
        Task<long> CountItemsByConditionAsync<T>(string index, ICondition condition) where T : class, IARepositoryModel;
        Task<bool> SaveAsync<T>(string index, T item) where T : class, IARepositoryModel;
        Task<bool> SaveAsync<T>(string index, List<T> items) where T : class, IARepositoryModel;
        #endregion

        
    }
}