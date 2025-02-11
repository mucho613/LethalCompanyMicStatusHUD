using BepInEx;
using HarmonyLib;

namespace MicStatusHUD
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MicStatusHUD : BaseUnityPlugin
    {
        private Harmony _harmony;

        private static MicStatusHUD Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                throw new System.Exception("More than 1 plugin instance.");
            }

            Instance = this;

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            System.Console.WriteLine("MicStatusHUD started!");
        }        
    }
}
