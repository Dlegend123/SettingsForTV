using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#pragma warning disable CS8603

namespace SettingsForTV
{
    internal class Settings
    {
        public List<JObject> Modes;
        public List<JObject> Shortcuts;
        public List<JObject> Themes;

        public Settings(List<JObject> modes, List<JObject> shortcuts, List<JObject> themes)
        {
            Modes = modes;
            Shortcuts = shortcuts;
            Themes = themes;
        }

        public Settings(){}
        public void Save()
        {
            var settings = JsonConvert.SerializeObject(this);
            var writer = new StreamWriter("Settings.json", false);
            writer.Write(settings);
            writer.Close();
        }

        public void SetSettings()
        {
            try
            {
                var reader = new StreamReader("Settings.json");
                var fileContents = reader.ReadToEnd();
                var settings = JsonConvert.DeserializeObject<Settings>(fileContents);

                if (settings is null) return;

                Modes = settings.Modes;
                Shortcuts = settings.Shortcuts;
                Themes = settings.Themes;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static Settings GetSettings()
        {
            try
            {
                var reader = new StreamReader("Settings.json");
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Settings>(fileContents);
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }
    }
}
