using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Daniel15.Sharpamp;

namespace lastfm.Services
{
    public class Track
    {
        [RequestParameters(HttpMethod.POST, AuthNeeded = true)]
        public static void Scrobble(TrackInfo t, bool chosenByUser)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.scrobble");
            parameters.Add("timestamp", Util.GetUnixTimestamp().ToString());
            if (!chosenByUser) parameters.Add("chosenByUser", "0");

            var resp = LfmServiceProxy.GetResponse(parameters);
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded = true)]
        public static IAsyncResult ScrobbleAsync(TrackInfo t, Action callback)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.scrobble");
            parameters.Add("timestamp", Util.GetUnixTimestamp().ToString());

            return LfmServiceProxy.GetResponseAsync(parameters, (doc) =>
                {
                    if (callback != null)
                        callback();
                });
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded=true)]
        public static IAsyncResult ScrobbleAsync(IEnumerable<TrackScrobble> tracks, Action<IEnumerable<TrackScrobble>> callback)
        {
            if (tracks.Count() > 50) throw new ArgumentException("Last.fm supports a maximum of 50 tracks");
            if (tracks.Count() == 0) return null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "track.scrobble");

            var i = 0;
            foreach (var trackScrobble in tracks)
            {
                var tp = trackScrobble.track.GetParametersDictionary(i);
                foreach (var p in tp)
                {
                    parameters.Add(p.Key, p.Value);
                }
                parameters.Add("timestamp[" + i + "]", trackScrobble.timestamp.ToString());
                i++;
            }

            return LfmServiceProxy.GetResponseAsync(parameters, (doc) =>
            {
                if (callback != null)
                    callback(tracks);

                var info = doc.Element("lfm").Element("scrobbles");
                var acc = info.Attribute("accepted").Value;
                var rej = info.Attribute("ignored").Value;

                Logger.LogMessage(string.Format("Batch scrobble info: accepted {0} tracks, ignored {1} tracks", acc, rej));
            });
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded = true)]
        public static void UpdateNowPlaying(TrackInfo t)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.updateNowPlaying");
            var resp = LfmServiceProxy.GetResponse(parameters);
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded = true)]
        public static IAsyncResult UpdateNowPlayingAsync(TrackInfo t)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.updateNowPlaying");
            return LfmServiceProxy.GetResponseAsync(parameters, null);
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded = false)]
        public static TrackInfo GetInfo(string artist, string title, bool autocorrect = false)
        {
            if (string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(title))
                throw new ArgumentException("Artist and title can't be empty");

            var parameters = new Dictionary<string, string>()
            {
                { "artist", artist },
                { "track", title }
            };
            if (autocorrect) parameters.Add("autocorrect", "1");
            parameters.Add("method", "track.getInfo");

            var res = LfmServiceProxy.GetResponse(parameters);
            System.Diagnostics.Debug.WriteLine(res.Document.ToString());
            return ReadInfoFromXDocument(res);
        }

        [RequestParameters(HttpMethod.POST, AuthNeeded = false)]
        public static IAsyncResult GetInfoAsync(string artist, string title, bool autocorrect, Action<TrackInfo> callback)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "artist", artist },
                { "track", title }
            };

            parameters.Add("method", "track.getInfo");
            if (autocorrect) parameters.Add("autocorrect", "1");

            return LfmServiceProxy.GetResponseAsync(parameters, (doc) =>
            {
                var track = ReadInfoFromXDocument(doc);
                callback(track);
            });
        }

        static TrackInfo ReadInfoFromXDocument(XDocument doc)
        {
            var root       = doc.Element("lfm");
            var track      = root.Element("track");
            var corrArtist = track.Element("artist").Element("name").Value;
            var corrTitle  = track.Element("name").Value;
            var duration   = track.Element("duration").Value;
            var url        = track.Element("url").Value;
            var album = "";
            if (track.Element("album") != null)
                album = track.Element("album").Element("title").Value;

            return new TrackInfo(corrArtist, corrTitle)
                {
                    Album = album,
                    Duration = int.Parse(duration),
                    LastFmUrl = url
                };
        }
    }
}
