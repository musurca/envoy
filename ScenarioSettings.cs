using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace WDS_Dispatches {
    public class ScenarioSettings {
        [JsonIgnore]
        public static int CURRENT_SETTINGS_VERSION = 2;

        [JsonIgnore]
        public static string ENVOY_VERSION_NUMBER = "1.04";

        public int SettingsVersion { get; set; }
        public int DispatchesPerLeader { get; set; }
        public int DispatchSpeed { get; set; }
        public int MinimumDispatchDelay { get; set; }
        public int ChanceDispatchDelay { get; set; }
        public int ChanceDispatchLost { get; set; }
        public bool UseChainOfCommand {  get; set; }
        public int InterdictionChance { get; set; }

        public ScenarioSettings() {
            // reasonable defaults in case of old settings file
            DispatchesPerLeader = 2;
            DispatchSpeed = 10;
            MinimumDispatchDelay = 1;
            ChanceDispatchLost = 2;
            ChanceDispatchDelay = 8;
            UseChainOfCommand = true;
            InterdictionChance = 30;
        }

        public bool Validate() {
            return SettingsVersion == CURRENT_SETTINGS_VERSION;
        }

        public void ToPreset(string preset) {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText("./presets/" + preset + ".preset", json);
        }

        public static string[] GetPresetNames() {
            string[] preset_files = Directory.GetFiles("./presets/", "*.preset");

            for(int i = 0; i < preset_files.Length; i++) {
                preset_files[i] = Path.GetFileNameWithoutExtension(
                    Path.GetFileName(preset_files[i])
                );
            } 

            return preset_files;
        } 

        public static ScenarioSettings FromPreset(string preset) {
            string[] preset_files = Directory.GetFiles("./presets/", "*.preset");
            
            for (int i = 0; i < preset_files.Length; i++) {
                string preset_name = Path.GetFileNameWithoutExtension(
                    Path.GetFileName(preset_files[i])
                );

                if(preset == preset_name) {
                    string json = File.ReadAllText(preset_files[i]);
                    ScenarioSettings settings = JsonConvert.DeserializeObject<ScenarioSettings>(json);

                    return settings;
                }
            }
            return new ScenarioSettings();
        }
    }
}
