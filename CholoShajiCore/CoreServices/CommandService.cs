using System;
using System.Composition;
using System.Threading.Tasks;
using CholoShajiCore.CoreInterfaces;
using CholoShajiCore.Ioc;

namespace CholoShajiCore.CoreServices
{
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        public async Task<object> ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var type = command.GetType().Name;
            var commandConsumer = IocContainer.Instance.Resolve<ICommandConsumer>(type+"Consumer") as ICommandConsumer<TCommand>;
            if (commandConsumer == null)
            {
                throw new Exception("CommandConsumer does not exist");
            }
            var result = await commandConsumer.DoExecute(command);
            return result;
        }
    }
}
