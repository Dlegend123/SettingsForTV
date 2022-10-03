using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SettingsForTV
{
    internal class Settings
    {
        public JObject Display;
        public JObject Mode;
        public JObject Shortcuts;
        public JObject Theme;

        public Settings(JObject display, JObject mode, JObject shortcuts, JObject theme)
        {
            Display = display;
            Mode = mode;
            Shortcuts = shortcuts;
            Theme = theme;
        }


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

                Display = settings.Display;
                Mode = settings.Mode;
                Shortcuts = settings.Shortcuts;
                Theme = settings.Theme;
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
