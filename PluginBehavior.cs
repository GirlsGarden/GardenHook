using System;
using System.Collections;
using System.IO;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace GardenHook;

public class PluginBehavior : MonoBehaviour
{
    private static readonly float WaitTime = 1.0f;
    public static bool IsGameSpeedChanged { get; set; }
    public static float CurrentGameSpeed { get; set; }
    private static float LastGSExecuteTime { get; set; }

    private void Update()
    {
        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            CurrentGameSpeed = Time.timeScale + (float)GardenConfig.Speed;
            Time.timeScale += (float)GardenConfig.Speed;
            IsGameSpeedChanged = (int)Time.timeScale != 1;
            LastGSExecuteTime = Time.deltaTime;
            var currSpeed = Time.timeScale.ToString();
            var text = "Game speed increased. Current: " + currSpeed + "x";
            Plugin.Global.Log.Msg(text);

            Notification.Popup("Game Speed", text);
        }

        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            CurrentGameSpeed = Time.timeScale - (float)GardenConfig.Speed;
            Time.timeScale -= (float)GardenConfig.Speed;
            IsGameSpeedChanged = (int)Time.timeScale != 1;
            LastGSExecuteTime = Time.deltaTime;
            var currSpeed = Time.timeScale.ToString();
            var text = "Game speed decreased. Current: " + currSpeed + "x";
            Plugin.Global.Log.Msg(text);

            Notification.Popup("Game Speed", text);
        }

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            CurrentGameSpeed = 1.0f;
            Time.timeScale = 1.0f;
            IsGameSpeedChanged = (int)Time.timeScale != 1;
            var currSpeed = Time.timeScale.ToString();
            var text = "Game speed restored. Current: " + currSpeed + "x";
            Plugin.Global.Log.Msg(text);

            Notification.Popup("Game Speed", text);
        }

        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            CurrentGameSpeed = 0.0f;
            Time.timeScale = 0.0f;
            IsGameSpeedChanged = (int)Time.timeScale != 1;
            LastGSExecuteTime = Time.deltaTime;
            var currSpeed = Time.timeScale.ToString();
            var text = "Game speed freezed. Current: " + currSpeed + "x";
            Plugin.Global.Log.Msg(text);

            Notification.Popup("Game Speed", text);
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            GardenConfig.AutoSkip = !GardenConfig.AutoSkip;
            Plugin.Global.Log.Msg("Auto Skip: " + (GardenConfig.AutoSkip ? "Enabled" : "Disabled"));

            Notification.Popup("Auto Skip", (GardenConfig.AutoSkip ? "Enabled" : "Disabled"));
        }

        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            MelonCoroutines.Start(ScreenshotFrame());
        }

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            GardenConfig.Read();
            Plugin.Global.Log.Msg("[Config] reloaded.");
        }

        LastGSExecuteTime += Time.deltaTime;
        if (IsGameSpeedChanged && LastGSExecuteTime >= WaitTime && Time.timeScale != CurrentGameSpeed)
        {
            LastGSExecuteTime = 0.0f;
            Time.timeScale = CurrentGameSpeed;
            Plugin.Global.Log.Msg("Game speed changed. Reset to: " + CurrentGameSpeed + "x");
        }
    }

    [HideFromIl2Cpp]
    IEnumerator ScreenshotFrame()
    {
        yield return new WaitForEndOfFrame();

        var username = Environment.UserName;
        var timeFormat = DateTime.Now.ToString("yyyyMMdd_HHmmssff");
        var location = string.Format("C:\\Users\\{0}\\Pictures\\garden_{1}.png", username, timeFormat);

        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] dataImage = texture.EncodeToPNG();
        File.WriteAllBytes(location, dataImage);

        Object.Destroy(texture);

        Notification.SsPopup(location);
    }
}