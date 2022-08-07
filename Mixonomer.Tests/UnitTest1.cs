using System;
using Mixonomer.Fire;
using Xunit;

namespace Mixonomer.Tests
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

