using System;
using System.Diagnostics;
using System.Collections;
using System.Timers;
using System.Xml.Serialization;
using System.IO;
using System.Security;
using System.Net;
using System.Windows.Forms;
using Daniel15.Sharpamp;
using lastfm.Services;

namespace lastfm
{
    public class Plugin : GeneralPlugin
    {
        Scrobbler scrobbler;
        ScrobblingSettings scrobblingSettings;
        bool eventsSubscribed;

        public override string Name
        {
            get { return "Last.fm scrobbler"; }
        }

        public override void Initialize()
        {
            scrobblingSettings = ScrobblingSettings.GetSettings();

            Logger.LogMessage("Settings loaded");

            scrobbler = new Scrobbler(scrobblingSettings);

            SetEventHandlers();

            Logger.LogMessage("Plugin initialized");
        }

        public override void Config()
        {
            LfmServiceProxy.NetworkErrorOccured -= LfmServiceProxy_NetworkErrorOccured;
            LfmServiceProxy.LastfmErrorOccured -= LfmServiceProxy_LastfmErrorOccured;
            LfmServiceProxy.ErrorOccured -= LfmServiceProxy_ErrorOccured;

            var f = new ConfigForm(scrobblingSettings);
            f.Show();
            f.FormClosed += f_FormClosed;
        }

        void f_FormClosed(object sender, FormClosedEventArgs e)
        {
            scrobbler.ReloadSettings(scrobblingSettings);
            SetEventHandlers();
            //(sender as Form).FormClosed -= f_FormClosed;
        }

        void SetEventHandlers()
        {
            Winamp.SongChanged -= Winamp_SongChanged;
            Winamp.SongRepeated -= Winamp_SongRepeated;
            Winamp.StatusChanged -= Winamp_StatusChanged;
            LfmServiceProxy.NetworkErrorOccured -= LfmServiceProxy_NetworkErrorOccured;
            LfmServiceProxy.LastfmErrorOccured -= LfmServiceProxy_LastfmErrorOccured;
            LfmServiceProxy.ErrorOccured -= LfmServiceProxy_ErrorOccured;

            eventsSubscribed = false;

            if (scrobbler.IsLoggedIn && !eventsSubscribed)
            {
                Winamp.SongChanged += Winamp_SongChanged;
                Winamp.SongRepeated += Winamp_SongRepeated;
                Winamp.StatusChanged += Winamp_StatusChanged;

                if (scrobblingSettings.DisplayErrorMessages)
                {
                    LfmServiceProxy.NetworkErrorOccured += LfmServiceProxy_NetworkErrorOccured;
                    LfmServiceProxy.LastfmErrorOccured += LfmServiceProxy_LastfmErrorOccured;
                    LfmServiceProxy.ErrorOccured += LfmServiceProxy_ErrorOccured;
                }

                eventsSubscribed = true;
            }
        }

        void Winamp_SongRepeated(object sender, TrackInfoEventArgs e)
        {
            //scrobbler.SetCurrentTrack(new Track(e.Song));
            //var msg = string.Format("Repeat:\r\n{0} - {1}", e.Song.Artist, e.Song.Title);
            var msg = "Repeat? " + e.Track.ToString();  
            MessageBox.Show(msg, "Winamp Last.fm scrobbler");
        }

        void Winamp_StatusChanged(object sender, Daniel15.Sharpamp.StatusChangedEventArgs e)
        {
            if (scrobbler.CurrentTrack != null)
            {
                //Logger.LogMessage("status changed handler");
                if (e.Status == Status.Paused) scrobbler.PauseTimer();
                else if (e.Status == Status.Playing) scrobbler.ResumeTimer();
                else scrobbler.StopTimer(); // Status.Stopped
            }
            else
            {
                Logger.LogMessage("scrobbler.CurrentTrack == null");
            }
            //Logger.Instance.LogMessage(e.Status.ToString(), "Status");
            //System.Windows.Forms.MessageBox.Show(e.Status.ToString(), "Status");
        }

        void Winamp_SongChanged(object sender, TrackInfoEventArgs e)
        {
            Logger.WriteEmptyLine();
            Logger.LogMessage("Track changed");

            //if (Winamp.Status != Status.Playing) return;

            if (!e.Track.HasMetadata)
            {
                Logger.LogMessage("No metadata available for playlist entry: " + e.Track);
                return;
            }

            var track = e.Track;
            //MessageBox.Show(string.Format("Current track: {0} [{1:m':'ss}]", track, track.NaturalDuration));

            if (track.Duration <= 0)
            {
                Track.GetInfoAsync(e.Track.Artist, e.Track.Title, true, (t) =>
                {
                    t.Filename = track.Filename;
                    track = t;
                    //t.IsChosenByUser = false;
                    LogTrackInfo(track);
                    if (!(e.Track.IsRadioStream && !scrobblingSettings.ScrobbleRadio))
                    {
                        scrobbler.ResetCurrentTrack(track);
                    }
                });
            }
            else
            {
                LogTrackInfo(track);
                scrobbler.ResetCurrentTrack(track);
            }
        }

        void LogTrackInfo(TrackInfo t)
        {
            var info = t.GetInfoString(includeDuration: true, includeChosenByUser: true);
            Logger.LogMessage(info, "Track data");
            //System.Windows.Forms.MessageBox.Show(info, "Song info");
        }

        void LfmServiceProxy_LastfmErrorOccured(object sender, LfmServiceProxy.RequestErrorEventArgs e)
        {
            if (e.LastfmErrorCode == 6 && (e.Message.Contains("Artist not found") || e.Message.Contains("Track not found")))
            {
                return;
            }
            var msg = e.Message + Environment.NewLine + "Press 'Cancel' to disable further notifications";
            ShowErrorMessage("Last.fm error", msg);
        }

        void LfmServiceProxy_NetworkErrorOccured(object sender, LfmServiceProxy.RequestErrorEventArgs e)
        {
            var msg = "A network-related error occured:" + Environment.NewLine +
                      e.Message + Environment.NewLine + 
                      "Press 'Cancel' to disable further notifications";
            ShowErrorMessage("Error", msg);
        }

        void LfmServiceProxy_ErrorOccured(object sender, LfmServiceProxy.ErrorEventArgs e)
        {
            ShowErrorMessage("Error", e.Error.Message);
        }

        void ShowErrorMessage(string caption, string message)
        {
            var res = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) //TODO: save settings here
            {
                LfmServiceProxy.NetworkErrorOccured -= LfmServiceProxy_NetworkErrorOccured;
                LfmServiceProxy.LastfmErrorOccured -= LfmServiceProxy_LastfmErrorOccured;
                LfmServiceProxy.ErrorOccured -= LfmServiceProxy_ErrorOccured;
                scrobblingSettings.DisplayErrorMessages = false;
                scrobblingSettings.SaveToFile();
            }
        }
    }
}
