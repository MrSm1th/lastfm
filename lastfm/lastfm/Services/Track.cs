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
        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = true)]
        public static void Scrobble(TrackInfo t, bool chosenByUser)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.scrobble");
            parameters.Add("timestamp", Util.GetUnixTimestamp().ToString());
            if (!chosenByUser) parameters.Add("chosenByUser", "0");

            var resp = LfmServiceProxy.GetResponse(parameters);
        }

        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = true)]
        public static IAsyncResult ScrobbleAsync(TrackInfo t, bool chosenByUser)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.scrobble");
            parameters.Add("timestamp", Util.GetUnixTimestamp().ToString());
            if (!chosenByUser) parameters.Add("chosenByUser", "0");

            return LfmServiceProxy.GetResponseAsync(parameters, null);
        }

        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = true)]
        public static void UpdateNowPlaying(TrackInfo t)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.updateNowPlaying");
            var resp = LfmServiceProxy.GetResponse(parameters);
        }

        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = true)]
        public static IAsyncResult UpdateNowPlayingAsync(TrackInfo t)
        {
            var parameters = t.GetParametersDictionary();
            parameters.Add("method", "track.updateNowPlaying");
            return LfmServiceProxy.GetResponseAsync(parameters, null);
        }

        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = false)]
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

        [RequestParameters(lastfm.Services.HttpMethod.POST, AuthNeeded = false)]
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
        /*
        public Track(string artist, string title)
        {
            Artist = artist;
            Title = title;
            IsChosenByUser = true;
        }

        public Track(Daniel15.Sharpamp.TrackInfo s)
        {
            if (string.IsNullOrEmpty(s.Artist) && string.IsNullOrEmpty(s.Title))
                throw new ArgumentException("Artist and title must not be empty");

            Artist = s.Artist;
            Title = s.Title;
            Duration = 0;
            if (!string.IsNullOrEmpty(s.Duration)) long.TryParse(s.Duration, out _duration);
            Album = s.Album;
            Year = s.Year;
            IsChosenByUser = !s.IsRadioStream;
        }

        /// <summary>
        /// Gets the track artist
        /// </summary>
        public string Artist { get; private set; }

        /// <summary>
        /// Gets the track title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets the track album
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// Gets or sets the track release year
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Gets or sets the track duration in milliseconds
        /// </summary>
        public long Duration { get { return _duration; } set { _duration = value; } }
        long _duration;

        /// <summary>
        /// Gets or sets the track ordinal in album (if any)
        /// </summary>
        public int TrackNumber { get; set; }

        public string AlbumArtist { get; set; }

        /// <summary>
        /// Determines whether the track was chosen by user or not (for example radio stream)
        /// </summary>
        public bool IsChosenByUser { get; set; }

        /// <summary>
        /// Gets a link to the Last.fm page of the track
        /// </summary>
        public string LastFmUrl { get; private set; }

        public TimeSpan NaturalDuration
        {
            get
            {
                return new TimeSpan(Duration * 10000);
            }
        }

        public Dictionary<string, string> GetParametersDictionary()
        {
            var res = new Dictionary<string, string>()
            {
                { "artist", Artist },
                { "track", Title }
            };

            if (!string.IsNullOrEmpty(Album)) res.Add("album", Album);

            if (!string.IsNullOrEmpty(Year)) res.Add("year", Year);

            if (Duration > 0) res.Add("duration", (Duration / 1000).ToString());

            return res;
        }

        public string GetInfoString(bool includeAlbum = false,
                              bool includeDuration = false,
                              bool includeYear = false,
                              bool includeTrackNumber = false,
                              bool includeAlbumArtist = false)
        {
            var br = Environment.NewLine;
            var info = string.Format("{0} - {1}", Artist, Title);
            if (includeAlbum) info += br + "Album: " + Album;
            if (includeDuration) info += br + "Duration: " + Duration.ToString();
            if (includeYear) info += br + "Year: " + Year;
            if (includeAlbumArtist) info += br + "Album artist: " + AlbumArtist;
            if (includeTrackNumber) info += br + "Track number: " + TrackNumber.ToString();

            return info;
        }

        public override string ToString()
        {
            return GetInfoString();
        }
        */
    }
}
