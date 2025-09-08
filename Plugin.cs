using BepInEx;
using BepInEx.Unity.IL2CPP;
using System.Text;
using System;
using BepInEx.Logging;

namespace GardenHook
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Global.Log = Log;

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Patch.Initialize();
            AddComponent<PluginBehavior>();
        }

        public class Global
        {
            public static ManualLogSource Log { get; set; }
        }
    }
}