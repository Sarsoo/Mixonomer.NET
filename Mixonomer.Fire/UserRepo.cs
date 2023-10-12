﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Mixonomer.Fire
{
    public class UserRepo
    {
        private static readonly string USER_COLLECTION = "spotify_users";

        private readonly FirestoreDb db;
        private readonly CollectionReference userCollection;

        public UserRepo(FirestoreDb db = null, string projectId = null)
        {
            this.db = db ?? FirestoreDb.Create(projectId);
            userCollection = this.db.Collection(USER_COLLECTION);
        }

        public IAsyncEnumerable<DocumentSnapshot> GetUsers()
        {
            return userCollection.StreamAsync();
        }

        public async Task<User> GetUser(string username)
        {
            var query = userCollection.WhereEqualTo("username", username.ToLower());
            var querySnapshot = await query.GetSnapshotAsync().ConfigureAwait(false);

            return querySnapshot.SingleOrDefault()?.ConvertTo<User>();
        }

        public Task<IAsyncEnumerable<DocumentSnapshot>> GetPlaylistDocs(User user)
        {
            var playlistCollection = db.Collection($"{USER_COLLECTION}/{user.Reference.Id}/playlists");

            return Task.FromResult(playlistCollection.StreamAsync());
        }

        public Task<IAsyncEnumerable<DocumentSnapshot>> GetTagDocs(User user)
        {
            var playlistCollection = db.Collection($"{USER_COLLECTION}/{user.Reference.Id}/tags");

            return Task.FromResult(playlistCollection.StreamAsync());
        }
    }
}

