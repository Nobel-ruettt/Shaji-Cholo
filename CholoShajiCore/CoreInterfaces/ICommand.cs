using System.Threading.Tasks;

namespace CholoShajiCore.CoreInterfaces
{
    public interface ICommand
    {
    }
    public interface ICommandConsumer
    {

    }
    public interface ICommandConsumer<in T> : ICommandConsumer where T : ICommand
    {
        Task<object> DoExecute(T command);
    }
}
