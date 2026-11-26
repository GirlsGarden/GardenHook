using HarmonyLib;
using Il2CppAssets.Api.Client;
using Il2CppAssets.Battle.Overseers;
using Il2CppAssets.Configuration;
using Il2CppAssets.CustomRendererFeatures;
using Il2CppAssets.GameUi.CharacterDetail.CharacterIllustViewer;
using Il2CppAssets.GameUi.Scenario;
using Il2CppAssets.GameUi.Services;
using Il2CppAssets.Platform.Implementation;
using System.Linq;
using System.Reflection;
using Il2CppAssets.Battle;
using Il2CppAssets.Platform;

namespace GardenHook;

public class Patch
{
    public static void Initialize()
    {
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(BootstrapPatches));
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

[HarmonyPatch]
public class ScenarioChoicePatch
{
    [HarmonyTargetMethod]
    public static MethodBase TargetMethod()
    {
        var nestedTypes = typeof(ScenarioController).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        MethodBase fallbackMethod = null;

        foreach (var type in nestedTypes)
        {
            if (!type.Name.Contains("DisplayClass")) continue;

            var targetMethod = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "_GenerateChoice_b__1");

            if (targetMethod != null)
            {
                return targetMethod;
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (fallbackMethod == null)
            {
                var looseMethod = methods.FirstOrDefault(m => m.Name.Contains("_GenerateChoice_b_"));
                if (looseMethod != null)
                {
                    fallbackMethod = looseMethod;
                }
            }
        }

        if (fallbackMethod != null)
        {
            return fallbackMethod;
        }

        Plugin.Global.Log.Error("Patch Error: Could not find any DisplayClass containing '_GenerateChoice_b__1'");
        return null;
    }

    [HarmonyPrefix]
    public static void Prefix(SceneBranchSelectionMaster selectionMaster, object __instance)
    {
        if (__instance == null || selectionMaster == null) return;

        var instanceType = __instance.GetType();

        var disableAnswerProp = instanceType.GetProperty("disableAnswer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (disableAnswerProp != null)
        {
            var setterMethod = disableAnswerProp.GetSetMethod(true);
            if (setterMethod != null)
            {
                setterMethod.Invoke(__instance, new object[] { false });
            }
        }
    }
}

public class BootstrapPatches
{
    private static bool _steamPatchesLoaded = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnvironmentService), nameof(EnvironmentService.CreateAssetEnvironmentSettings))]
    public static void CreateAssetEnvironmentSettings(string assetsUrl, string staticContentUrl, ref AssetEnvironmentSettings __result)
    {
        if (__result == null || string.IsNullOrEmpty(__result.BundleAssetUrl))
            return;

        // Steam Restoration: why need neko? husky kill neko!
        if (GardenConfig.KillNeko && __result.BundleAssetUrl.Contains("/Guard/"))
        {
            if (!_steamPatchesLoaded)
            {
                _steamPatchesLoaded = true;
                HarmonyLib.Harmony.CreateAndPatchAll(typeof(SteamRestorePatches));
                Plugin.Global.Log.Msg("Loaded SteamRestorePatches");
            }

            __result = new AssetEnvironmentSettings(
                __result.BundleAssetUrl.Replace("/Guard/", "/Default/"),
                __result.DirectAssetUrl.Replace("/XG/", "/X/"),
                __result.IsAstcFormatSupported
            );
        }

        // DMM to Steam: This time, neko kill husky!
        if (!GardenConfig.KillNeko && __result.BundleAssetUrl.Contains("/Default/"))
        {
            Plugin.Global.Log.Msg($"Loli Censoring...");

            __result = new AssetEnvironmentSettings(
                __result.BundleAssetUrl.Replace("/Default/", "/Guard/"),
                __result.DirectAssetUrl.Replace("/X/", "/XG/"),
                __result.IsAstcFormatSupported
            );
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Platform), nameof(Platform.TryCreateWindowsPlatform))]
    public static void TryCreateWindowsPlatformPostfix(Platform __instance, IPlatformImplementation __result)
    {
        if (!GardenConfig.KillNeko)
        {
            return;
        }

        if (__instance?.configuration != null)
        {
            var appConfig = __instance.configuration.TryCast<ApplicationConfiguration>();
            if (appConfig != null)
            {
                appConfig.GameRating = GameRatings.Rated;
                var platformName = __result?.TryCast<Il2CppSystem.Object>()?.GetIl2CppType().Name ?? "null";
                Plugin.Global.Log.Msg($"Forced GameRating = Rated (Platform: {platformName})");
            }
        }
    }
}

// Steam Restoration: why need neko? husky kill neko!
public class SteamRestorePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.UpdateDataModel))]
    public static void UpdateDataModelPostfix(BattleController __instance)
    {
        if (__instance.model != null)
        {
            __instance.model.RemoveCockpitButton = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterIllustViewerController), nameof(CharacterIllustViewerController.Refresh))]
    public static void RefreshPostfix(CharacterIllustViewerController __instance)
    {
        var vm = __instance.Model?.Value;
        if (vm != null)
        {
            vm.IsShowableCockpit = true;
        }
    }
}
