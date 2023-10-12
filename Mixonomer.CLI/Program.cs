using System;
using System.Linq;
using Mixonomer.Fire;
using System.Threading.Tasks;
using Mixonomer.Fire.Extensions;

namespace Mixonomer.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repo = new UserRepo(projectId: "mixonomer-test");

            var userContext = await repo.GetUserContext("andy");

            Console.WriteLine(userContext.User);
        }
    }
}

