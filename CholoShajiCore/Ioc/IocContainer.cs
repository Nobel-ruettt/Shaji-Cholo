using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CholoShajiCore.Ioc
{
    public class IocContainer : IIocContainer
    {
        private static IIocContainer _instance;
        private static readonly object LockObject = new object();
        private CompositionHost Container { get; set; }
        private ContainerConfiguration ContainerConfiguration { get; }
        private List<string> AssemblyLists { get; }
        private IocContainer()
        {
            ContainerConfiguration = new ContainerConfiguration();
            AssemblyLists = new List<string>();
        }
        public static IIocContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new IocContainer();
                        }
                    }
                }

                return _instance;
            }
            set => _instance = value;
        }
        public void AddAssembly<T>()
        {
            AddAssembly(typeof(T).Assembly);
        }

        public void AddAssembly(Assembly assembly)
        {
            //Console.WriteLine("Registering Assembly {0}", assembly.FullName);
            if (AssemblyLists.Contains(assembly.FullName))
            {
                return;
            }

            ContainerConfiguration.WithAssembly(assembly);
            AssemblyLists.Add(assembly.FullName);
        }

        public void Create()
        {
            if (Container == null)
            {
                lock (LockObject)
                {
                    if (Container != null)
                    {
                        return;
                    }
                    Container = ContainerConfiguration.CreateContainer();
                }
            }
        }

        public T Resolve<T>()
        {
            Create();
            lock (LockObject)
            {
                try
                {
                    var value = Container.GetExport<T>();
                    return value;
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to resolve type {0}", typeof(T).Name);
                    throw;
                }
            }
        }
        public T Resolve<T>(string name)
        {
            Create();
            lock (LockObject)
            {
                try
                {
                    return Container.GetExport<T>(name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to resolve type {0} and key {1}", typeof(T).Name, name);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        public IEnumerable<object> ResolveMany(string name)
        {
            Create();
            lock (LockObject)
            {
                try
                {
                    return Container.GetExports<object>(name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to resolve with key {0}", name);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        public IEnumerable<T> ResolveMany<T>()
        {
            Create();

            lock (LockObject)
            {
                try
                {
                    return Container.GetExports<T>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to resolve type {0}", typeof(T).Name);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        public void AddAllAssemblies()
        {
            var entryLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (string.IsNullOrEmpty(entryLocation) == false)
            {
                AddAllAssemblies(entryLocation);
            }
            var executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(executingLocation) == false && executingLocation.Equals(entryLocation, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                AddAllAssemblies(executingLocation);
            }

        }

        private void AddAllAssemblies(string location)
        {
            if (string.IsNullOrEmpty(location) != false) return;
            //Console.WriteLine("Searching dll and exe from {0}", location);
            var files = Directory.GetFiles(location);
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    if (!IsExeOrDll(fileInfo)) continue;
                    Console.WriteLine("{0}", file);
                    AddAssembly(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static bool IsExeOrDll(FileInfo fileInfo)
        {
            return fileInfo.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase) ||
                   fileInfo.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase);
        }

        private void AddAssembly(string file)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(file);
            if (string.IsNullOrEmpty(assemblyName) != false) return;
            Console.WriteLine("Loading Assembly {0}", assemblyName);
            AddAssembly(Assembly.Load(assemblyName));
        }
    }
}
