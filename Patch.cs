using Assets.Battle.Overseers;
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
    public static void SetSkipAvaiability(ref bool available)
    {
        available = true;
    }
}