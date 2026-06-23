using HarmonyLib;
using Il2CppAssets.Api.Client;
using Il2CppAssets.Battle.Overseers;
using Il2CppAssets.CustomRendererFeatures;
using Il2CppAssets.GameUi.Scenario;
using System.Linq;
using System.Reflection;

namespace GardenHook;

public class Patch
{
    public static void Initialize()
    {
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(ScenarioChoicePatch));
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
