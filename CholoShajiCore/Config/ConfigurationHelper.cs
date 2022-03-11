using System;
using System.IO;
using System.Linq;
using CholoShajiCore.DataBase.DataBaseClient;
using CholoShajiCore.Ioc;
using Newtonsoft.Json;

namespace CholoShajiCore.Config
{
    public static class ConfigurationHelper
    {
        private static Configuration _configuration;

        private static readonly object LockObject = new object();
        public static Configuration Instance
        {
            get
            {
                if (_configuration == null)
                {
                    lock (LockObject)
                    {
                        if (_configuration == null)
                        {
                            _configuration = GetConfiguration();
                        }
                    }
                }

                return _configuration;
            }
            set => _configuration = value;
        }

        private static Configuration GetConfiguration()
        {
            var d = GetConfigDirectoryInfo();
            var files = d.GetFiles("LocalConfig.json").ToList();
            var jsonString = File.ReadAllText(files[0].FullName);
            var configuration = JsonConvert.DeserializeObject<Configuration>(jsonString);
            return configuration;
        }

        private static DirectoryInfo GetConfigDirectoryInfo()
        {
            var rootFolder = Directory.GetCurrentDirectory();
            DirectoryInfo d = new DirectoryInfo($"{rootFolder}");
            return d;
        }

        public static string GetIndex()
        {
            return Instance.Index;
        }

        public static IDataBaseClient GetDataBaseClient()
        {
            var client = IocContainer.Instance.Resolve<IDataBaseClient>(Instance.DataBaseClient);
            return client;
        }
    }
}
