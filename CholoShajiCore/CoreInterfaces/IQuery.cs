using System.Threading.Tasks;

namespace CholoShajiCore.CoreInterfaces
{
    public interface IQuery
    {
    }
    public interface IQueryHandler
    {

    }
    public interface IQueryHandler<in TQuery> : IQueryHandler where TQuery : IQuery
    {
        Task<object> DoExecute(TQuery query);
    }
}
