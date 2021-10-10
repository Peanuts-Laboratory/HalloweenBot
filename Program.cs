using HalloweenBot.DiscordBot;
using System;

namespace HalloweenBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.StartDatabase();

            new BotMain().RunBotAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}
