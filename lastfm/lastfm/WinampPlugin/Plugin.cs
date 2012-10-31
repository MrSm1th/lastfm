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

            Logger.LogMessage("plugin initialized");
        }

        public override void Config()
        {
            var f = new ConfigForm(scrobblingSettings);
            f.Show();
            f.FormClosed += f_FormClosed;
        }

        void f_FormClosed(object sender, FormClosedEventArgs e)
        {
            scrobbler = new Scrobbler(scrobblingSettings);
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

            eventsSubscribed = false;

            if (scrobblingSettings.ScrobblingEnabled && scrobbler.IsLoggedIn && !eventsSubscribed)
            {
                Winamp.SongChanged += Winamp_SongChanged;
                Winamp.SongRepeated += Winamp_SongRepeated;
                Winamp.StatusChanged += Winamp_StatusChanged;

                if (scrobblingSettings.DisplayErrorMessages)
                {
                    LfmServiceProxy.NetworkErrorOccured += LfmServiceProxy_NetworkErrorOccured;
                    LfmServiceProxy.LastfmErrorOccured += LfmServiceProxy_LastfmErrorOccured;
                    LfmServiceProxy.ErrorOccured += new LfmServiceProxy.ErrorEventHandler(LfmServiceProxy_ErrorOccured);
                }

                eventsSubscribed = true;
            }
        }

        void Winamp_SongRepeated(object sender, SongInfoEventArgs e)
        {
            //scrobbler.SetCurrentTrack(new Track(e.Song));
            //var msg = string.Format("Repeat:\r\n{0} - {1}", e.Song.Artist, e.Song.Title);
            var msg = "Repeat? " + e.Song.ToString();  
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

        void Winamp_SongChanged(object sender, SongInfoEventArgs e)
        {
            Logger.WriteEmptyLine();
            Logger.LogMessage("Track changed");

            //if (Winamp.Status != Status.Playing) return;

            if (!e.Song.HasMetadata)
            {
                Logger.LogMessage("No metadata available for playlist entry: " + e.Song.Filename);
                return;
            }

            var track = new Track(e.Song);
            //MessageBox.Show(string.Format("Current track: {0} [{1:m':'ss}]", track, track.NaturalDuration));

            if (track.Duration > 0)
            {
                ShowTrackInfo(track);
                scrobbler.SetCurrentTrack(track);
            }
            else
            {
                if (!(e.Song.IsRadioStream && !scrobblingSettings.TryToScrobbleRadio))
                {
                    try
                    {
                        Track.GetInfoAsync(e.Song.Artist, e.Song.Title, true, (t) =>
                            {
                                track = t;
                                t.IsChosenByUser = false;
                                ShowTrackInfo(t);
                                scrobbler.SetCurrentTrack(t);
                            });
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
                    }
                }
                else if (e.Song.IsRadioStream && !scrobblingSettings.TryToScrobbleRadio)
                    Logger.LogMessage("Radio scrobbling disabled");
            }
        }

        void ShowTrackInfo(Track t)
        {
            var info = string.Format("{0} - {1}", t.Artist, t.Title);
            Logger.LogMessage(info, "Track data");
            info += string.Format("\nDuration: {0}", t.Duration);
            info += string.Format("\nAlbum: {0}", t.Album);
            info += string.Format("\nYear: {0}", t.Year);
            //info += Environment.NewLine + string.Format("time: {0}ms", time);
            //System.Windows.Forms.MessageBox.Show(info, "Song info");
        }

        void LfmServiceProxy_LastfmErrorOccured(object sender, LfmServiceProxy.RequestErrorEventArgs e)
        {
            if (e.LastfmErrorCode == 6 && (e.Message.Contains("Artist not found") || e.Message.Contains("Track not found")))
            {
                //var trackInfo = string.Format("{0} - {1}", e.RequestParameters["artist"], e.RequestParameters["track"]);
                //Logger.LogError(e.Message + ": " + trackInfo);
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
            }
        }
    }
}
