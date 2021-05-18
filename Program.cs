using System;

namespace PabloBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new PabloBot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
