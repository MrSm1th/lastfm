/*
 * Sharpamp version 0.1 beta
 * $Id$
 * Copyright (C) 2009, Daniel Lo Nigro (Daniel15) <daniel at d15.biz>
 * 
 * This file is part of Sharpamp.
 * 
 * Sharpamp is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Sharpamp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with Sharpamp.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Daniel15.Sharpamp
{
    /// <summary>
    /// Information about a song.
    /// </summary>
    [Serializable]
    public class TrackInfo
    {
        public static bool TryParseFromPlaylistTitle(string playlistTitle, bool hasMetadata, ref TrackInfo track)
        {
            var songTitle = playlistTitle;
            if (!HasFileExtension(playlistTitle) && !hasMetadata) // radio stream
            {
                //track.IsChosenByUser = false;
                // strip out text in parentheses (which we believe to be a station name) at the end of string
                var streamName = Regex.Match(playlistTitle, @" (((?'Open'\()[^\(\)]*)+((?'Close-Open'\))[^\(\)]*)+)(?(Open)(?!))$");

                if (!string.IsNullOrEmpty(streamName.Value))
                {
                    songTitle = playlistTitle.Replace(streamName.Value, "");
                }
            }
            else // a file without an ID3 tag
                songTitle = StripOutFileExtension(songTitle);

            var match = Regex.Match(songTitle, @"(.+?) - (.+)");
            if (match.Success)
            {
                var artist = match.Groups[1].Value;
                var title = match.Groups[2].Value;
                track.Artist = artist;
                track.Title = title;
                track.HasMetadata = true;
                track.PlaylistTitle = playlistTitle;
                return true;
            }

            return false;
        }

        public static bool HasFileExtension(string playlistTitle)
        {
            var fileExts = string.Join("|", FileExts);
            var pattern = string.Format(@".*\.({0})", fileExts);

            // If a track has no ID3 tag, Winamp shows its filename with extension.
            return Regex.IsMatch(playlistTitle, pattern, RegexOptions.IgnoreCase);
        }

        static string StripOutFileExtension(string playlistTitle)
        {
            var pattern = string.Format(@"\.({0})$", string.Join("|", FileExts));
            return Regex.Replace(playlistTitle, pattern, "");
        }

        static string[] FileExts = new string[] { "mp3", "wav", "ogg", "wma", "flac", "wv", "aif", "aiff", "m4a" };



        public TrackInfo(string artist, string title)
        {
            Artist = artist;
            Title = title;
            //IsChosenByUser = true;
            Album = Year = AlbumArtist = Filename = PlaylistTitle = string.Empty;
        }



        /// <summary>
        /// Title of the song
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Artist of the song
        /// </summary>
        public string Artist { get; private set; }
        
        /// <summary>
        /// Album of the song
        /// </summary>
        public string Album { get; set; }
        
        /// <summary>
        /// Year of the song
        /// </summary>
        public string Year { get; set; }
        
        /// <summary>
        /// Gets or sets track duration in milliseconds
        /// </summary>
        public long Duration { get { return _duration; } set { _duration = value; } }
        long _duration;

        /// <summary>
        /// Gets or sets track ordinal in album (if any)
        /// </summary>
        public int TrackNumber { get; set; }

        public string AlbumArtist { get; set; }

        /// <summary>
        /// Determines whether the track was chosen by user or not (for example radio stream)
        /// </summary>
        public bool IsChosenByUser { get { return !IsRadioStream; } } // { get; set; }

        /// <summary>
        /// Gets a link to the Last.fm page of the track
        /// </summary>
        public string LastFmUrl { get; set; }

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

            if (!IsChosenByUser) res.Add("chosenByUser", "0");

            if (!string.IsNullOrEmpty(Album)) res.Add("album", Album);

            if (!string.IsNullOrEmpty(Year)) res.Add("year", Year);

            if (Duration > 0) res.Add("duration", (Duration / 1000).ToString());

            return res;
        }

        public Dictionary<string, string> GetParametersDictionary(int index)
        {
            var res = new Dictionary<string, string>()
            {
                { "artist[" + index + "]", Artist },
                { "track[" + index + "]", Title }
            };

            if (!IsChosenByUser) res.Add("chosenByUser[" + index + "]", "0");

            if (!string.IsNullOrEmpty(Album)) res.Add("album[" + index + "]", Album);

            if (!string.IsNullOrEmpty(Year)) res.Add("year[" + index + "]", Year);

            if (Duration > 0) res.Add("duration[" + index + "]", (Duration / 1000).ToString());

            return res;
        }

        public string GetInfoString(bool includeAlbum = false,
                              bool includeDuration = false,
                              bool includeYear = false,
                              bool includeTrackNumber = false,
                              bool includeAlbumArtist = false,
                              bool includeChosenByUser = false)
        {
            var br = Environment.NewLine;
            var info = string.Format("{0} - {1}", Artist, Title);
            if (includeDuration) info += string.Format(" [{0:0}:{1:00}]", (int)NaturalDuration.TotalMinutes, NaturalDuration.Seconds);
            if (includeAlbum) info += br + "Album: " + Album;
            if (includeYear) info += br + "Year: " + Year;
            if (includeAlbumArtist) info += br + "Album artist: " + AlbumArtist;
            if (includeTrackNumber) info += br + "Track number: " + TrackNumber;
            if (includeChosenByUser) info += br + "Chosen by user: " + IsChosenByUser;

            return info;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title) ?
                   string.Format("{0} [{1:0}:{2:00}]", GetInfoString(), (int)NaturalDuration.TotalMinutes, NaturalDuration.Seconds) :
                   PlaylistTitle;
        }

        /// <summary>
        /// Whether the Track has any metadata. If false, only the title will be
        /// available.
        /// </summary>
        public bool HasMetadata { get; internal set; }
        
        /// <summary>
        /// Filename of the song
        /// </summary>
        public string Filename { get; internal set; }

        public string PlaylistTitle { get; internal set; }

        public bool IsRadioStream
        {
            get
            {
                return Filename.Contains("http://") && !HasFileExtension(Filename);
            }
        }
    }
}
