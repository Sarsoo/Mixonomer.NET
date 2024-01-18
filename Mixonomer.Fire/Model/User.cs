using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Mixonomer.Fire;

[FirestoreData]
public class User
{
    [FirestoreProperty]
    public string access_token { get; set; }
    [FirestoreProperty]
    public string email { get; set; }
    [FirestoreProperty]
    public DateTime last_login { get; set; }
    [FirestoreProperty]
    public DateTime last_refreshed { get; set; }
    [FirestoreProperty]
    public DateTime last_keygen { get; set; }
    [FirestoreProperty]
    public string lastfm_username { get; set; }
    [FirestoreProperty]
    public bool locked { get; set; }
    [FirestoreProperty]
    public string password { get; set; }
    [FirestoreProperty]
    public string refresh_token { get; set; }
    [FirestoreProperty]
    public bool spotify_linked { get; set; }
    [FirestoreProperty]
    public int token_expiry { get; set; }
    [FirestoreProperty]
    public string type { get; set; }
    [FirestoreProperty]
    public string username { get; set; }
    [FirestoreProperty]
    public bool validated { get; set; }

    [FirestoreProperty]
    public IEnumerable<string> apns_tokens { get; set; }
    [FirestoreProperty]
    public bool notify { get; set; }
    [FirestoreProperty]
    public bool notify_admins { get; set; }
    [FirestoreProperty]
    public bool notify_playlist_updates { get; set; }
    [FirestoreProperty]
    public bool notify_tag_updates { get; set; }

    [FirestoreDocumentId]
    public DocumentReference Reference { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreateTime { get; set; }
    [FirestoreDocumentUpdateTimestamp]
    public Timestamp UpdateTime { get; set; }
    [FirestoreDocumentReadTimestamp]
    public Timestamp ReadTime { get; set; }
}
