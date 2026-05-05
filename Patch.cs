using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Il2CppAssets.Api.Client.ConnectionManager;

namespace GardenHook;

public class Patch
{
    public static void Initialize()
    {
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patch));
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(HudOverseer), "SetSkipAvaiability")]
    //public static void SetSkipAvaiability(ref HudOverseer __instance, ref bool available)
    //{
    //    available = true;
    //    if (GardenConfig.AutoSkip)
    //    {
    //        __instance.ProcessSkipButtonClick();
    //    }
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(MosaicRendererFeature), "Create")]
    //public static void RemoveMosaic(MosaicRendererFeature __instance)
    //{
    //    __instance.passSettings.Keyword = "demosaic";
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(ScenarioController.__c__DisplayClass125_0), nameof(ScenarioController.__c__DisplayClass125_0._GenerateChoice_b__1))]
    //public static void GenerateChoice_b__1(ref SceneBranchSelectionMaster selectionMaster, ref ScenarioController.__c__DisplayClass125_0 __instance)
    //{
    //    __instance.disableAnswer = false;
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IzanamiNetworkUtilizationManager), "ProcessRequestFinished")]
    public static void ProcessRequestFinished(Il2CppBest.HTTP.HTTPRequest request, Il2CppBest.HTTP.HTTPResponse response)
    {
        if (request.Uri.AbsolutePath == "/api/circle-battle-rankings")
        {
            try
            {
                var ts = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                using var fs = new FileStream(ts+request.Uri.AbsolutePath.Replace("/", "_"), FileMode.Create, FileAccess.Write);
                fs.Write(response.Data.ToArray());
                Plugin.Global.Log.Msg("circle ranking saved. Filename: " + ts + request.Uri.AbsolutePath.Replace("/", "_"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
            }
        }
    }
}