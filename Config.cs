using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GardenHook;

public class GardenConfig
{
    public static double Speed;
    public static bool AutoSkip;
    public static bool KillNeko = true;

    public static void Read()
    {
        if (File.Exists($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/config.json"))
        {
            var content = File.ReadAllText($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/config.json", Encoding.UTF8);
            var doc = JsonDocument.Parse(content);
            var config = doc.RootElement;

            var needWrite = false;

            if (config.TryGetProperty("speed", out var sValue))
            {
                Speed = sValue.GetDouble();
            }
            else
            {
                Speed = 0.5;
                needWrite = true;
            }

            if (config.TryGetProperty("auto_skip", out var skipValue))
            {
                AutoSkip = skipValue.GetBoolean();
            }
            else
            {
                AutoSkip = false;
                needWrite = true;
            }

            if (config.TryGetProperty("kill_neko", out var nekoValue))
            {
                KillNeko = nekoValue.GetBoolean();
            }
            else
            {
                KillNeko = true;
                needWrite = true;
            }

            if (needWrite) WriteJsonFile(Speed, AutoSkip, KillNeko);

            Plugin.Global.Log.Msg("Current setting:");
            Plugin.Global.Log.Msg("Game speed(each step): " + Speed);
            Plugin.Global.Log.Msg("Auto Skip: " + AutoSkip);
            Plugin.Global.Log.Msg("Kill Neko: " + KillNeko);
        }
        else
        {
            Plugin.Global.Log.Warning("config.json not found!!!");
            Plugin.Global.Log.Warning("Using default config.");
            Speed = 0.5;
            AutoSkip = false;

            // Create default JSON file
            WriteJsonFile(0.5, false, true);
        }
    }

    public static void WriteJsonFile(double speed, bool auto_skip, bool kill_neko)
    {
        var config = new config
        {
            speed = speed,
            auto_skip = auto_skip,
            kill_neko = kill_neko
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/config.json", json);
    }

    public class config
    {
        [JsonPropertyName("speed")]
        public double speed { get; set; }
        [JsonPropertyName("auto_skip")]
        public bool auto_skip { get; set; }
        [JsonPropertyName("kill_neko")]
        public bool kill_neko { get; set; }
    }
}