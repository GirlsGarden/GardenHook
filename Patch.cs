using HarmonyLib;
using Il2CppAssets.Battle.Overseers;
using Il2CppAssets.CustomRendererFeatures;

namespace GardenHook;

public class Patch
{
    public static void Initialize()
    {
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patch));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudOverseer), "SetSkipAvaiability")]
    public static void SetSkipAvaiability(ref HudOverseer __instance, ref bool available)
    {
        available = true;
        if (GardenConfig.AutoSkip)
        {
            __instance.ProcessSkipButtonClick();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MosaicRendererFeature), "Create")]
    public static void RemoveMosaic(MosaicRendererFeature __instance)
    {
        __instance.passSettings.Keyword = "demosaic";
    }
}