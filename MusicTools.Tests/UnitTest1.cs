using System;
using MusicTools.Fire;
using Xunit;

namespace MusicTools.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var repo = new UserRepo();

            var user = await repo.GetUser("andy");
        }
    }
}

