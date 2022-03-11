using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CholoShajiCore.Ioc
{
    public interface IIocContainer
    {
        void AddAssembly<T>();
        void AddAssembly(Assembly assembly);
        void Create();
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<object> ResolveMany(string name);
        IEnumerable<T> ResolveMany<T>();
        void AddAllAssemblies();
    }
}
