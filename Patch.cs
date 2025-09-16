using Assets.Battle.Overseers;
using Assets.CustomRendererFeatures;
using HarmonyLib;

namespace GardenHook;

public class Patch
{
    public static void Initialize()
    {
        Harmony.CreateAndPatchAll(typeof(Patch));
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