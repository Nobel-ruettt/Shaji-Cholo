using System.Threading.Tasks;
using CholoShajiCore.CoreInterfaces;

namespace CholoShajiCore.CoreServices
{
    public interface ICommandService
    {
        Task<object> ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand;
    }
}