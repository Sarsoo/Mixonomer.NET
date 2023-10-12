using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Mixonomer.Fire
{
    [FirestoreData]
    public class Tag
    {
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string tag_id { get; set; }
        [FirestoreProperty]
        public string username { get; set; }

        [FirestoreProperty]
        public DateTime last_updated { get; set; }

        [FirestoreProperty]
        public int count { get; set; }
        [FirestoreProperty]
        public double proportion { get; set; }

        [FirestoreProperty]
        public bool time_objects { get; set; }
        [FirestoreProperty]
        public string total_time { get; set; }
        [FirestoreProperty]
        public int total_time_ms { get; set; }
        [FirestoreProperty]
        public int total_user_scrobbles { get; set; }

        [FirestoreProperty]
        public IEnumerable<TagItem> tracks { get; set; }
        [FirestoreProperty]
        public IEnumerable<TagItem> albums { get; set; }
        [FirestoreProperty]
        public IEnumerable<TagItem> artists { get; set; }

        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }

        [FirestoreDocumentCreateTimestamp]
        public Timestamp CreateTime { get; set; }
        [FirestoreDocumentUpdateTimestamp]
        public Timestamp UpdateTime { get; set; }
        [FirestoreDocumentReadTimestamp]
        public Timestamp ReadTime { get; set; }
    }

    [FirestoreData]
    public class TagItem
    {
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string time { get; set; }
        [FirestoreProperty]
        public int time_ms { get; set; }
        [FirestoreProperty]
        public int count { get; set; }

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

