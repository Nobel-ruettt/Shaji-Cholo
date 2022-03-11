using CholoShajiCore.Settings;

namespace CholoShajiCore.Config
{
    public class Configuration
    {
        public CloudinarySettings CloudinarySetting { get; set; }
        public MongoDataBaseSettings MongodbDataBaseSetting { get; set; }
        public string Index { get; set; }
        public string DataBaseClient { get; set; }
    }
}
