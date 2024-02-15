using System.Reflection;
using Common.Utils;
using Config.Net;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Common
{
    public static class Global
    {
        public static readonly string configFile = "config.json";
        public static readonly IConfig config = new ConfigurationBuilder<IConfig>().UseJsonFile(configFile).Build();
        public static readonly Logger c = new("Global");

        public static readonly MongoClient MongoClient = new(config.DatabaseUri);
        public static readonly IMongoDatabase db = MongoClient.GetDatabase("PemukulPaku");
        public static long GetUnixInSeconds() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        public static uint GetRandomSeed() => (uint)(GetUnixInSeconds() * new Random().Next(1, 10) / 10);

        public static void CreateConfigFile()
        {
            try
            {
                File.WriteAllText(configFile, JsonConvert.SerializeObject(Global.config, Formatting.Indented));
            }
            catch (Exception ex)
            {
                c.Error($"无法创建 {configFile} 错误: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }

    public interface IConfig
    {
#if DEBUG
        [Option(DefaultValue = VerboseLevel.Debug)]
#else
        [Option(DefaultValue = VerboseLevel.Normal)]
#endif
        VerboseLevel VerboseLevel { get; set; }

        [Option(DefaultValue = false)]
        bool UseLocalCache { get; set; }

        [Option(DefaultValue = true)]
        bool CreateAccountOnLoginAttempt { get; set; }

        [Option(DefaultValue = "mongodb://127.0.0.1:27017/PemukulPaku")]
        string DatabaseUri { get; set; }

        [Option]
        IGameserver Gameserver { get; set; }

        [Option]
        IHttp Http { get; set; }

        public interface IGameserver
        {
            [Option(DefaultValue = "127.0.0.1")]
            public string Host { get; set; }

            [Option(DefaultValue = (uint)(16100))]
            public uint Port { get; set; }

            [Option(DefaultValue = "overseas01")]
            public string RegionName { get; set; }
        }

        public interface IHttp
        {

            [Option(DefaultValue = (uint)(80))]
            public uint HttpPort { get; set; }

            [Option(DefaultValue = (uint)(443))]
            public uint HttpsPort { get; set; }

            [Option(DefaultValue = "127.0.0.1")]
            public string Host { get; set; }

            [Option(DefaultValue = "127.0.0.1:1314")]
            public string AssetServerHost { get; set; }
        }
    }

    public enum VerboseLevel
    {
        Silent = 0,
        Normal = 1,
        Debug = 2
    }
}