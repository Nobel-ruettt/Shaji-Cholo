using System.Threading.Tasks;
using CholoShajiCore.CoreInterfaces;

namespace CholoShajiCore.CoreServices
{
    public interface IQueryService
    {
        Task<object> ExecuteQuery<TQuery>(TQuery query) where TQuery : IQuery;
    }
}