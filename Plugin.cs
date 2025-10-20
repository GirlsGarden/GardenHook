using System.Text;
using System;
using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

[assembly: MelonInfo(typeof(GardenHook.Plugin), "GardenHook-melon", "1.0.3", "GardenHook")]

namespace GardenHook
{
    public class Plugin : MelonMod
    {
        public override void OnInitializeMelon()
        {
            if (Console.LargestWindowWidth > 0)
            {
                Console.OutputEncoding = Encoding.UTF8;
            }

            var log = LoggerInstance;
            Global.Log = log;

            log.Msg($"Plugin GardenHook is loaded!");

            GardenConfig.Read();
            Patch.Initialize();

            ClassInjector.RegisterTypeInIl2Cpp<PluginBehavior>();
            GameObject melonModObject = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = "keybinding"
            };
            melonModObject.AddComponent<PluginBehavior>();
            UnityEngine.Object.DontDestroyOnLoad(melonModObject);
        }

        public class Global
        {
            public static MelonLogger.Instance Log { get; set; }
        }
    }
}