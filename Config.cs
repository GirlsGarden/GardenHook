using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GardenHook;

public class GardenConfig
{
    public static double Speed;

    public static void Read()
    {
        if (File.Exists("./BepInEx/plugins/config.json"))
        {
            var content = File.ReadAllText("./BepInEx/plugins/config.json", Encoding.UTF8);
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

            if (needWrite) WriteJsonFile(Speed);

            Plugin.Global.Log.LogMessage("Current setting:");
            Plugin.Global.Log.LogMessage("Game speed(each step): " + Speed);
        }
        else
        {
            Plugin.Global.Log.LogWarning("config.json not found!!!");
            Plugin.Global.Log.LogWarning("Using default config.");
            Speed = 0.5;

            // Create default JSON file
            WriteJsonFile(0.5);
        }
    }

    public static void WriteJsonFile(double speed)
    {
        var config = new config
        {
            speed = speed,
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("./BepInEx/plugins/config.json", json);
    }

    public class config
    {
        [JsonPropertyName("speed")]
        public double speed { get; set; }
    }
}