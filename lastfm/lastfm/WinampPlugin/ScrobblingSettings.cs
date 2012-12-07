using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace lastfm
{
    public class ScrobblingSettings
    {
        //public string Username { get; set; }
        //public string Password { get; set; }

        static string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\winamp\plugins\scrobbling_settings.ini";


        public bool ScrobblingEnabled { get; set; }

        public int ScrobbleSongtimePercent { get; set; }

        /// <summary>
        /// Gets or sets scrobble time (in minutes) for tracks with unknown duration
        /// </summary>
        public int DefaultScrobbleTime { get; set; }

        public bool ScrobbleRadio { get; set; }

        /// <summary>
        /// Gets or sets value that indicates whether now playing info should be updated in user's profile
        /// </summary>
        public bool UpdateNowPlaying { get; set; }

        public bool DisplayErrorMessages { get; set; }

        /// <summary>
        /// Gets or sets Last.fm session key
        /// </summary>
        public string SessionKey { get; set; }

        public static ScrobblingSettings GetSettings()
        {
            if (ConfigFileExists())
                return ScrobblingSettings.LoadFromFile();
            else return GetDefaultSettings();
        }

        static ScrobblingSettings LoadFromFile()
        {
            FileStream s = null;
            try
            {
                s = new FileStream(FilePath, FileMode.Open);
                var x = new XmlSerializer(typeof(ScrobblingSettings));
                return x.Deserialize(s) as ScrobblingSettings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't read scrobbling settings from file. Loading default settings. Exception:");
                Debug.WriteLine(ex.Message);
                return GetDefaultSettings();
            }
            finally
            {
                if (s != null) s.Dispose();
            }
        }

        public static ScrobblingSettings GetDefaultSettings()
        {
            return new ScrobblingSettings()
            {
                ScrobblingEnabled = true,
                ScrobbleSongtimePercent = 75,
                ScrobbleRadio = false,
                UpdateNowPlaying = true,
                DisplayErrorMessages = true
            };
        }

        public bool SaveToFile()
        {
            FileStream s = null;
            try
            {
                File.Delete(FilePath);
                s = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
                var x = new XmlSerializer(typeof(ScrobblingSettings));
                x.Serialize(s, this);
                return true;
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Error while saving settings: " + ex.Message);
                return false;
            }
            finally
            {
                if (s != null) s.Dispose();
            }
        }

        static bool ConfigFileExists()
        {
            return File.Exists(FilePath);
        }

        public override string ToString()
        {
            var nl = Environment.NewLine;
            var str = "Scrobbling enabled: " + ScrobblingEnabled + nl
                    + "Scrobble songtime percent: " + ScrobbleSongtimePercent + nl
                    + "Default scrobble time: " + DefaultScrobbleTime + nl
                    + "Scrobble radio: " + ScrobbleRadio + nl
                    + "Update now playing info: " + UpdateNowPlaying + nl
                    + "Display error messages: " + DisplayErrorMessages;
            return str;
        }
    }
}
