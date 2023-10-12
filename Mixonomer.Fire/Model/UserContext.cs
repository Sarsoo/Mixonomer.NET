using System.Collections.Generic;

namespace Mixonomer.Fire.Model
{
    public class UserContext
    {
        public User User { get; set; }
        public IEnumerable<Playlist> Playlists { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}