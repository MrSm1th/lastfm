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
    public class TrackInfo
    {
        public static TrackInfo ParseFromPlaylistTitle(string playlistTitle, bool hasMetadata)
        {
            var songTitle = playlistTitle;
            if (HasFileExtension(playlistTitle) && !hasMetadata) // radio stream
            {
                // strip out text in parentheses (which we believe to be a station name) at the end of string
                var streamName = Regex.Match(playlistTitle, @" (((?'Open'\()[^\(\)]*)+((?'Close-Open'\))[^\(\)]*)+)(?(Open)(?!))$");

                if (!string.IsNullOrEmpty(streamName.Value))
                {
                    songTitle = playlistTitle.Replace(streamName.Value, "");
                }
            }
            if (Regex.IsMatch(songTitle, @".* - .*"))
            {
                songTitle = StripOutFileExtension(songTitle);
                var m = Regex.Match(songTitle, @"(.*) - (.*)");
                var artist = m.Groups[1].Value;
                var title = m.Groups[2].Value;
                return new TrackInfo(artist, title)
                {
                    HasMetadata = true,
                    PlaylistTitle = playlistTitle
                };
            }

            return new TrackInfo("", "")
            {
                PlaylistTitle = playlistTitle
            };
        }

        public static bool HasFileExtension(string playlistTitle)
        {
            var fileExts = string.Join("|", FileExts);

            // If a track has no ID3 tag, Winamp shows its filename with extension.
            if (Regex.IsMatch(playlistTitle, @".*\.[" + fileExts + "]", RegexOptions.IgnoreCase))
                return false;

            return true;
        }

        static string StripOutFileExtension(string playlistTitle)
        {
            var pattern = string.Format(@"\.({0})$", string.Join("|", FileExts));
            return Regex.Replace(playlistTitle, pattern, "");
        }

        static string[] FileExts = new string[] { "mp3", "wav", "ogg", "wma", "flac" };



        public TrackInfo(string artist, string title)
        {
            Artist = artist;
            Title = title;
            IsChosenByUser = true;
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
        /// Duration of the song
        /// </summary>
        //public string Duration { get; internal set; }

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
        public bool IsChosenByUser { get; set; }

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
            return string.Format("{0} [{1:m':'ss}]", GetInfoString(), NaturalDuration);
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
                return Filename.Contains("http://");
            }
        }
    }
}
