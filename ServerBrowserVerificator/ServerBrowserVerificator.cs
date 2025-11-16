using HarmonyLib;
using oomtm450PuckMod_ServerBrowserVerificator.Configs;
using oomtm450PuckMod_ServerBrowserVerificator.SystemFunc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace oomtm450PuckMod_ServerBrowserVerificator {
    /// <summary>
    /// Class containing the main code for the ServerBrowserVerificator patch.
    /// </summary>
    public class ServerBrowserVerificator : IPuckMod {
        #region Constants
        /// <summary>
        /// Const string, version of the mod.
        /// </summary>
        private const string MOD_VERSION = "1.0.9";
        #endregion

        #region Fields
        /// <summary>
        /// Harmony, harmony instance to patch the Puck's code.
        /// </summary>
        private static readonly Harmony _harmony = new Harmony(Constants.MOD_NAME);

        /// <summary>
        /// ServerConfig, config set by the client.
        /// </summary>
        private static ClientConfig ClientConfig { get; set; } = new ClientConfig();
        #endregion

        /// <summary>
        /// Class that patches the UpdateServers event from UIServerBrowser.
        /// </summary>
        [HarmonyPatch(typeof(UIServerBrowser), nameof(UIServerBrowser.UpdateServers))]
        public class UIServerBrowser_UpdateServers_Patch {
            [HarmonyPrefix]
            public static bool Prefix(ref List<ServerBrowserServer> serverBrowserServers) {
                try {
                    List<(ServerBrowserServer Server, string Keyword)> serversToRemove = new List<(ServerBrowserServer, string)>();
                    foreach (ServerBrowserServer server in serverBrowserServers) {
                        foreach (string key in ClientConfig.VerificationDictionary.Keys.Where(key => server.name.Contains(key.ToLower()))) {
                            if (!ClientConfig.VerificationDictionary[key].Contains(server.ipAddress))
                                serversToRemove.Add((server, key));
                        }
                    }

                    foreach ((ServerBrowserServer Server, string Keyword) server in serversToRemove) {
                        serverBrowserServers.Remove(server.Server);
                        Logging.Log($"Removed server \"{server.Server.name}\" with IP address \"{server.Server.ipAddress}\", because of the keyword \"{server.Keyword}\".", ClientConfig);
                    }
                }
                catch (Exception ex) {
                    Logging.LogError($"Error in UIServerBrowser_UpdateServers_Patch Prefix().\n{ex}");
                }

                return true;
            }
        }

        /// <summary>
        /// Function that launches when the mod is being enabled.
        /// </summary>
        /// <returns>Bool, true if the mod successfully enabled.</returns>
        public bool OnEnable() {
            try {
                Logging.Log($"Enabling...", ClientConfig, true);

                _harmony.PatchAll();

                Logging.Log($"Enabled.", ClientConfig, true);

                Logging.Log("Setting client sided config.", ClientConfig, true);
                ClientConfig = ClientConfig.ReadConfig();

                return true;
            }
            catch (Exception ex) {
                Logging.LogError($"Failed to enable.\n{ex}");
                return false;
            }
        }

        /// <summary>
        /// Function that launches when the mod is being disabled.
        /// </summary>
        /// <returns>Bool, true if the mod successfully disabled.</returns>
        public bool OnDisable() {
            try {
                Logging.Log($"Disabling...", ClientConfig, true);

                _harmony.UnpatchSelf();

                Logging.Log($"Disabled.", ClientConfig, true);
                return true;
            }
            catch (Exception ex) {
                Logging.LogError($"Failed to disable.\n{ex}");
                return false;
            }
        }
    }
}
