using Newtonsoft.Json;
using oomtm450PuckMod_ServerBrowserVerificator.SystemFunc;
using System.Collections.Generic;
using System.IO;

namespace oomtm450PuckMod_ServerBrowserVerificator.Configs {
    /// <summary>
    /// Class containing the configuration from oomtm450_serverbrowserverificator_clientconfig.json used for this mod.
    /// </summary>
    public class ClientConfig : IConfig {
        #region Constants
        /// <summary>
        /// Const string, name used when sending the config data to the client.
        /// </summary>
        public const string CONFIG_DATA_NAME = Constants.MOD_NAME + "_clientconfig.json";
        #endregion

        #region Properties
        /// <summary>
        /// Bool, true if the info logs must be printed.
        /// </summary>
        public bool LogInfo { get; set; } = true;

        /// <summary>
        /// Dictionary of string and List of string, keyword with the list of ip addresses to verify.
        /// </summary>
        public Dictionary<string, List<string>> VerificationDictionary { get; set; } = new Dictionary<string, List<string>> {
            { "ponce",
                new List<string> {
                    "155.138.234.217", "144.202.54.222", "144.202.86.178", "149.28.252.129", "45.77.100.45", "70.34.205.198", "80.240.17.115", "45.76.19.168",
                    "45.76.62.18",
                }
            },
        };
        #endregion

        /// <summary>
        /// Function that serialize the ClientConfig object.
        /// </summary>
        /// <returns>String, serialized ClientConfig.</returns>
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Function that unserialize a ClientConfig.
        /// </summary>
        /// <param name="json">String, JSON that is the serialized ClientConfig.</param>
        /// <returns>ClientConfig, unserialized ClientConfig.</returns>
        internal static ClientConfig SetConfig(string json) {
            return JsonConvert.DeserializeObject<ClientConfig>(json);
        }

        /// <summary>
        /// Function that reads the config file for the mod and create a ClientConfig object with it.
        /// Also creates the file with the default values, if it doesn't exists.
        /// </summary>
        /// <returns>ClientConfig, parsed config.</returns>
        internal static ClientConfig ReadConfig() {
            ClientConfig config = new ClientConfig();

            string rootPath = Path.GetFullPath(".");
            string configPath = Path.Combine(rootPath, CONFIG_DATA_NAME);
            if (File.Exists(configPath)) {
                string configFileContent = File.ReadAllText(configPath);
                ClientConfig readConfig = SetConfig(configFileContent);

                foreach (string key in new List<string>(readConfig.VerificationDictionary.Keys)) {
                    if (config.VerificationDictionary.ContainsKey(key))
                        readConfig.VerificationDictionary[key] = config.VerificationDictionary[key];
                }

                config = readConfig;
            }

            File.WriteAllText(configPath, config.ToString());

            Logging.Log($"Writing client config : {config}", config);

            return config;
        }
    }
}
