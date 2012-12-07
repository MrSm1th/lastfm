using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using lastfm.Services;
using System.Diagnostics;
using Daniel15.Sharpamp;

namespace lastfm
{
    [Serializable]
    public class TrackScrobble : IEquatable<TrackScrobble>
    {
        public long timestamp;
        public TrackInfo track;

        public TrackScrobble(long timestamp, TrackInfo track)
        {
            this.timestamp = timestamp;
            this.track = track;
        }

        public bool Equals(TrackScrobble other)
        {
            return object.Equals(this, other);
        }
    }

    public class Scrobbler
    {
        List<TrackScrobble> UnscrobbledTracks;
        string unscrobbledTracksFilename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\winamp\plugins\UnscrobbledTracks.bin";


        Timer Timer;
        Timer NowPlayingTimer;
        //DateTime LastChangeTrackTime;
        DateTime? CurrentTrackScrobbleTime;

        ScrobblingSettings Settings;
        //public Queue ScrobbleQueue { get; private set; }

        public TrackInfo CurrentTrack { get; private set; }
        public bool IsCurrentTrackScrobbled { get; private set; }

        bool IsTimerPaused;

        double _timeRemainingToScrobble;
        /// <summary>
        /// How much time left to scrobbling of the track (in milliseconds)
        /// </summary>
        public double TimeRemainingToScrobble
        {
            get
            {
                if (!IsTimerPaused && CurrentTrackScrobbleTime.HasValue)
                {
                    var res = (CurrentTrackScrobbleTime.Value - DateTime.Now).TotalSeconds * 1000;
                    // Timeout.Infinite = -1, pauses the timer;
                    // values less than -1 cause the timer to throw an ArgumentOutOfRangeException
                    if (res < 0) res = Timeout.Infinite;
                    return res;
                }
                else
                    return _timeRemainingToScrobble;
            }
        }

        public bool IsLoggedIn { get { return !string.IsNullOrEmpty(Settings.SessionKey); } }


        public Scrobbler(ScrobblingSettings s)
        {
            Settings = s;
            UnscrobbledTracks = new List<TrackScrobble>();
            Timer = new Timer(t_Elapsed);
            NowPlayingTimer = new Timer(np_Elapsed);
            IsTimerPaused = false;
            _timeRemainingToScrobble = Timeout.Infinite;
            Logger.LogMessage("Scrobbler initialized. Timer started");
            LfmServiceProxy.SessionKey = Settings.SessionKey;

            DeserializeUnscrobbledTracks();
        }

        ~Scrobbler()
        {
            SerializeUnscrobbledTracks();
        }

        void t_Elapsed(object state)
        {
            try
            {
                Scrobble();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        void np_Elapsed(object state)
        {
            UpdateNowPlaying();
        }

        void Scrobble()
        {
            if (!IsCurrentTrackScrobbled)
            {
                var trackScrobble = new TrackScrobble(Util.GetUnixTimestamp(), CurrentTrack);
                // we add track to this list and in case we get a valid response we'll remove it and try to scrobble remaining tracks (if any)
                UnscrobbledTracks.Add(trackScrobble);
                Track.ScrobbleAsync(CurrentTrack, () =>
                    {
                        UnscrobbledTracks.Remove(trackScrobble);

                        SendUnscrobbledTracks();
                    });
                ResetScrobbler();
                IsCurrentTrackScrobbled = true;
            }
        }

        void SendUnscrobbledTracks()
        {
            if (UnscrobbledTracks.Count > 0)
            {
                int i = 0;
                IEnumerable<TrackScrobble> tracks = null;
                while ((tracks = UnscrobbledTracks.Skip(i).Take(50)).Count() > 0)
                {
                    Track.ScrobbleAsync(tracks, (t) =>
                    {
                        UnscrobbledTracks = UnscrobbledTracks.Except(t).ToList();
                    });
                    i += 50;
                }
            }
        }

        void ResetScrobbler()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timeRemainingToScrobble = 0;
            //IsTimerPaused = true;
        }

        void UpdateNowPlaying()
        {
            Track.UpdateNowPlayingAsync(CurrentTrack);
            //System.Windows.Forms.MessageBox.Show(CurrentTrack.ToString(), "Now playing");
        }

        public void ResetCurrentTrack()
        {
            CurrentTrack = null;
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            NowPlayingTimer.Change(Timeout.Infinite, Timeout.Infinite);

        }

        public void ResetCurrentTrack(TrackInfo t)
        {
            CurrentTrack = t;
            Logger.LogMessage("Current track reset");

            if (Settings.ScrobblingEnabled && (t.Duration > 0 || Settings.DefaultScrobbleTime > 0))
            {
                var trackScrobbleTime = (t.Duration <= 0) ?
                    Settings.DefaultScrobbleTime * 60 * 1000 :
                    t.Duration / 100 * Settings.ScrobbleSongtimePercent; // [milliseconds]

                if (!IsTimerPaused)
                    CurrentTrackScrobbleTime = DateTime.Now.AddSeconds(trackScrobbleTime / 1000);
                _timeRemainingToScrobble = trackScrobbleTime;
                IsCurrentTrackScrobbled = false;

                // period of Timeout.Infinite means that the callback will be executed only once
                if (!IsTimerPaused)
                    Timer.Change(trackScrobbleTime, Timeout.Infinite);
            }
            else
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            if (Settings.UpdateNowPlaying && !IsTimerPaused)
                // update nowPlaying after 15 sec of playback
                NowPlayingTimer.Change(15000, Timeout.Infinite);
        }

        public void ReloadSettings(ScrobblingSettings s)
        {
            this.Settings = s;
            Logger.LogMessage("Settings reloaded:\n" + s.ToString());
        }

        public void PauseTimer()
        {
            // disable scrobblilg logic if track duration is 0 (last.fm data is often incomplete)
            if (CurrentTrack != null && CurrentTrack.Duration > 0)
            {
                // "pause" timer
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                NowPlayingTimer.Change(Timeout.Infinite, Timeout.Infinite);

                // calculate time remaining and set the private variable
                _timeRemainingToScrobble = TimeRemainingToScrobble;

                // from now on, TimeRemainingToScrobble will be returning
                // the same time we set in the previous line
                IsTimerPaused = true;
                Logger.LogMessage("Timer paused");
                Logger.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble);
            }
        }

        public void ResumeTimer()
        {
            if (CurrentTrack != null && CurrentTrack.Duration > 0)
            {
                Timer.Change((int)TimeRemainingToScrobble, Timeout.Infinite);
                CurrentTrackScrobbleTime = DateTime.Now.AddSeconds(TimeRemainingToScrobble / 1000);

                // TimeRemainingToScrobble will calculate time automatically
                IsTimerPaused = false;
                Logger.LogMessage("Timer resumed");
                Logger.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble);

                if (Settings.UpdateNowPlaying)
                    // update nowPlaying after 10 sec of playback
                    NowPlayingTimer.Change(10000, Timeout.Infinite);
            }
        }

        public void StopTimer()
        {
            if (CurrentTrack != null && CurrentTrack.Duration > 0)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                NowPlayingTimer.Change(Timeout.Infinite, Timeout.Infinite);

                _timeRemainingToScrobble = (double)CurrentTrack.Duration / 100 * Settings.ScrobbleSongtimePercent;
                IsTimerPaused = true;
                Logger.LogMessage("Timer stopped");
                Logger.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble.ToString());
            }
        }

        public bool SerializeUnscrobbledTracks()
        {
            if (UnscrobbledTracks.Count == 0) return true;

            System.IO.Stream s = null;
            try
            {
                var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                s = System.IO.File.OpenWrite(unscrobbledTracksFilename);
                b.Serialize(s, UnscrobbledTracks);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
            finally
            {
                if (s != null) s.Dispose();
            }
        }

        public bool DeserializeUnscrobbledTracks()
        {
            if (!System.IO.File.Exists(unscrobbledTracksFilename)) return true;

            System.IO.Stream s = null;
            try
            {
                s = System.IO.File.OpenRead(unscrobbledTracksFilename);
                var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                UnscrobbledTracks = (List<TrackScrobble>) b.Deserialize(s);
                s.Close();
                System.IO.File.Delete(unscrobbledTracksFilename);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
            finally
            {
                if (s != null) s.Dispose();
            }
        }
    }
}
