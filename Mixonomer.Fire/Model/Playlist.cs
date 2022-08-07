using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Mixonomer.Fire
{
    [FirestoreData]
    public class Playlist
    {
        [FirestoreProperty]
        public string uri { get; set; }
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string type { get; set; }

        [FirestoreProperty]
        public bool include_recommendations { get; set; }
        [FirestoreProperty]
        public int recommendation_sample { get; set; }
        [FirestoreProperty]
        public bool include_library_tracks { get; set; }

        [FirestoreProperty]
        public IEnumerable<string> parts { get; set; }
        [FirestoreProperty]
        public IEnumerable<DocumentReference> playlist_references { get; set; }
        [FirestoreProperty]
        public bool shuffle { get; set; }

        [FirestoreProperty]
        public string sort { get; set; }
        [FirestoreProperty]
        public string description_overwrite { get; set; }
        [FirestoreProperty]
        public string description_suffix { get; set; }

        [FirestoreProperty]
        public DateTime last_updated { get; set; }

        [FirestoreProperty]
        public int lastfm_stat_count { get; set; }
        [FirestoreProperty]
        public int lastfm_stat_album_count { get; set; }
        [FirestoreProperty]
        public int lastfm_stat_artist_count { get; set; }

        [FirestoreProperty]
        public double lastfm_stat_percent { get; set; }
        [FirestoreProperty]
        public double lastfm_stat_album_percent { get; set; }
        [FirestoreProperty]
        public double lastfm_stat_artist_percent { get; set; }

        [FirestoreProperty]
        public DateTime lastfm_stat_last_refresh { get; set; }

        [FirestoreProperty]
        public bool add_last_month { get; set; }
        [FirestoreProperty]
        public bool add_this_month { get; set; }

        [FirestoreProperty]
        public int day_boundary { get; set; }

        [FirestoreProperty]
        public bool include_spotify_owned { get; set; }

        [FirestoreProperty]
        public string chart_range { get; set; }
        [FirestoreProperty]
        public int chart_limit { get; set; }

        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }

        [FirestoreDocumentCreateTimestamp]
        public Timestamp CreateTime { get; set; }
        [FirestoreDocumentUpdateTimestamp]
        public Timestamp UpdateTime { get; set; }
        [FirestoreDocumentReadTimestamp]
        public Timestamp ReadTime { get; set; }
    }
}

