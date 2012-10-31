using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using lastfm.Services;
using System.Diagnostics;

namespace lastfm
{
    public class Scrobbler
    {
        Timer Timer;
        Timer NowPlayingTimer;
        //DateTime LastChangeTrackTime;
        DateTime? CurrentTrackScrobbleTime;

        ScrobblingSettings Settings;
        //public Queue ScrobbleQueue { get; private set; }

        public Track CurrentTrack { get; private set; }
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
                    // values less then -1 cause the timer to throw an ArgumentOutOfRangeException
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
            Timer = new Timer(t_Elapsed);
            NowPlayingTimer = new Timer(np_Elapsed);
            IsTimerPaused = false;
            Logger.I.LogMessage("Scrobbler initialized. Timer started");
            LfmServiceProxy.SessionKey = Settings.SessionKey;

//            Logger.I.LogMessage("Scrobbler initialized");
        }
        ~Scrobbler()
        {
            Timer.Dispose();
        }

        void t_Elapsed(object state)
        {
            Scrobble();
        }

        void np_Elapsed(object state)
        {
            UpdateNowPlaying();
        }

        void Scrobble()
        {
            //Logger.I.LogMessage("timer elapsed");
            if (!IsCurrentTrackScrobbled)
            {
                Track.ScrobbleAsync(CurrentTrack, CurrentTrack.IsChosenByUser);
                //System.Windows.Forms.MessageBox.Show("Scrobbled:\r\n" + CurrentTrack.GetInfo(), "Winamp Last.fm scrobbler");
                ResetScrobbler();
                IsCurrentTrackScrobbled = true;
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
            //System.Windows.Forms.MessageBox.Show(CurrentTrack.GetInfo(includeAlbum: true, includeDuration: true), "Now playing");
        }

        public void SetCurrentTrack(Track t)
        {
            CurrentTrack = t;
            Logger.I.LogMessage("current track reset");

            if (t.Duration > 0)
            {
                var trackScrobbleTime = t.Duration / 100 * Settings.ScrobbleSongtimePercent; // [milliseconds]
                if (!IsTimerPaused)
                    CurrentTrackScrobbleTime = DateTime.Now.AddSeconds(trackScrobbleTime / 1000);
                _timeRemainingToScrobble = trackScrobbleTime;
                IsCurrentTrackScrobbled = false;

                //IsTimerPaused = false;
                //LastChangeTrackTime = DateTime.Now;

                // period of Timeout.Infinite means that the callback will be executed only once
                if (!IsTimerPaused)
                    Timer.Change(trackScrobbleTime, Timeout.Infinite);
            }
            else
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                //CurrentTrack = null;
            }

            if (Settings.UpdateNowPlaying && !IsTimerPaused)
                // update nowPlaying after 10 sec of playback
                NowPlayingTimer.Change(10000, Timeout.Infinite);
        }

        //public void ReloadSettings(ScrobblingSettings s)
        //{
        //    this.settings = s;
        //}

        public void PauseTimer()
        {
            // disable scrobblilg logic if track duration is 0 (last.fm data is often incomplete)
            //if (CurrentTrack != null)
            //    Logger.I.LogMessage(string.Format("{0} - {1} [{2}]", CurrentTrack.Artist, CurrentTrack.Title, CurrentTrack.Duration));
            //else
            //    Logger.I.LogMessage("CurrentTrack == null");
            if (CurrentTrack.Duration > 0)
            {
                // "pause" timer
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                NowPlayingTimer.Change(Timeout.Infinite, Timeout.Infinite);

                // calculate time remaining and set the private variable
                _timeRemainingToScrobble = TimeRemainingToScrobble;
                //if (TimeRemainingToScrobble < 0)
                //{
                //    var msg = "Warning: TimeRemainingToScrobble is less than 0";
                //    Logger.I.LogMessage(msg);
                //    System.Windows.Forms.MessageBox.Show(msg, "Winamp Last.fm scrobbler");
                //    _timeRemainingToScrobble = 0;
                //}

                // from now on, TimeRemainingToScrobble will be returning
                // the same time we set in the previous line
                IsTimerPaused = true;
                Logger.I.LogMessage("timer paused");
                Logger.I.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble.ToString());
            }
        }

        public void ResumeTimer()
        {
            //if (CurrentTrack != null)
            //    Logger.I.LogMessage(string.Format("{0} - {1} [{2}]", CurrentTrack.Artist, CurrentTrack.Title, CurrentTrack.Duration));
            //else
            //    Logger.I.LogMessage("CurrentTrack == null"); 
            if (CurrentTrack.Duration > 0)
            {
                Timer.Change((int)TimeRemainingToScrobble, Timeout.Infinite);
                CurrentTrackScrobbleTime = DateTime.Now.AddSeconds(TimeRemainingToScrobble / 1000);

                // TimeRemainingToScrobble will be calculating time automatically
                IsTimerPaused = false;
                Logger.I.LogMessage("timer resumed");
                Logger.I.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble.ToString());

                if (Settings.UpdateNowPlaying)
                    // update nowPlaying after 10 sec of playback
                    NowPlayingTimer.Change(10000, Timeout.Infinite);
            }
        }

        public void StopTimer()
        {
            //if (CurrentTrack != null)
            //    Logger.I.LogMessage(string.Format("{0} - {1} [{2}]", CurrentTrack.Artist, CurrentTrack.Title, CurrentTrack.Duration));
            //else
            //    Logger.I.LogMessage("CurrentTrack == null");
            if (CurrentTrack.Duration > 0)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                NowPlayingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                // 
                _timeRemainingToScrobble = (double)CurrentTrack.Duration / 100 * Settings.ScrobbleSongtimePercent;
                IsTimerPaused = true;
                Logger.I.LogMessage("timer stopped");
                Logger.I.LogMessage("TimeRemainingToScrobble = " + TimeRemainingToScrobble.ToString());
            }
        }
    }
}
