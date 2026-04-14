using HarmonyLib;
using Il2CppAssets.Api.Client;
using Il2CppAssets.Battle.Overseers;
using Il2CppAssets.CustomRendererFeatures;
using Il2CppAssets.GameUi.Scenario;

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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ScenarioController.__c__DisplayClass125_0), nameof(ScenarioController.__c__DisplayClass125_0._GenerateChoice_b__1))]
    public static void GenerateChoice_b__1(ref SceneBranchSelectionMaster selectionMaster, ref ScenarioController.__c__DisplayClass125_0 __instance)
    {
        __instance.disableAnswer = false;
    }
}