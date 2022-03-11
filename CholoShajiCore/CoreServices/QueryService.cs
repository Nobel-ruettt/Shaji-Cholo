using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CholoShajiCore.CoreInterfaces;
using CholoShajiCore.Ioc;

namespace CholoShajiCore.CoreServices
{
    [Export(typeof(IQueryService))]
    public class QueryService : IQueryService
    {
        public async Task<object> ExecuteQuery<TQuery>(TQuery query) where TQuery : IQuery
        {
            var type = query.GetType().Name;
            var queryHandler = IocContainer.Instance.Resolve<IQueryHandler>(type + "Handler") as IQueryHandler<TQuery>;
            if (queryHandler == null)
            {
                throw new Exception("queryHandler does not exist");
            }
            var result = await queryHandler.DoExecute(query);
            return result;
        }
    }
}
