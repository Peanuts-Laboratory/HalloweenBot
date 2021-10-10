using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HalloweenBot.DiscordBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("spookystats")]
        public async Task ServerStats()
        {
            JObject obj = Database.GetServerData();
            EmbedBuilder candybuilder = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithDescription("<:GreenCandy:896448862963761152> Green Candy Found: " + obj["CandyReceieved"][0] + "\n" +
                                 "<:YellowCandy:896448997345087558> Yellow Candy Found: " + obj["CandyReceieved"][1] + "\n" +
                                 "<:RedCandy:896448997428961381> Red Candy Found: " + obj["CandyReceieved"][2] + "\n" +
                                 "<:BlueCandy:896448997550587915> Blue Candy Found: " + obj["CandyReceieved"][3] + "\n" +
                                 "<:PurpleCandy:896448997298954270> Purple Candy Found: " + obj["CandyReceieved"][4]
                );

            await Context.Channel.SendMessageAsync(text: null, embed: candybuilder.Build());
        }

        [Command("trickortreat")]
        public async Task TrickOrTreat()
        {
            if (!Database.UserRegistered(Context.User.Id))
                Database.CreateUser(BotMain.Client.GetUser(Context.User.Id));

            if (Database.OnCooldown(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync("You are still on cooldown!");
                return;
            }

            Database.SetUnixTime(Context.User.Id);

            Random rng = new Random();
            int candyTier = rng.Next(0, 5);

            int[] candy = new int[] { };
            switch (candyTier)
            {
                case 0:
                    await Context.Channel.SendMessageAsync("Boo! No treat for you");
                    return;
                case 1:
                    int greenCandy = rng.Next(1, 10);
                    candy = new int[] { greenCandy, 0, 0, 0, 0};
                    await Context.Channel.SendMessageAsync($"Ding Dong...\nYou have received {greenCandy} Green Candy");
                    break;
                case 2:
                    int g1 = rng.Next(1, 10);
                    int y1 = rng.Next(1, 2);
                    candy = new int[] { g1, y1, 0, 0, 0 };
                    await Context.Channel.SendMessageAsync($"Ding Dong...\nYou have received:\n**{g1}**: Green Candy\n**{y1}**: Yellow Candy!");
                    break;
                case 3:
                    int g2 = rng.Next(1, 15);
                    int y2 = rng.Next(1, 5);
                    int r1 = rng.Next(1, 2);
                    candy = new int[] { g2, y2, r1, 0, 0 };
                    await Context.Channel.SendMessageAsync($"Ding Dong...\nYou have received:\n**{g2}**: Green Candy\n**{y2}**: Yellow Candy\n**{r1}**: Red Candy\n");
                    break;
                case 4:
                    int g3 = rng.Next(1, 15);
                    int y3 = rng.Next(1, 10);
                    int r2 = rng.Next(1, 5);
                    int b1 = rng.Next(1, 2);
                    candy = new int[] { g3, y3, r2, b1, 0 };
                    await Context.Channel.SendMessageAsync($"Ding Dong...\nYou have received:\n**{g3}**: Green Candy\n**{y3}**: Yellow Candy\n**{r2}**: Red Candy\n**{b1}**: Blue Candy\n");
                    break;
                case 5:
                    int g4 = rng.Next(1, 15);
                    int y4 = rng.Next(1, 15);
                    int r3 = rng.Next(1, 10);
                    int b2 = rng.Next(1, 5);
                    int p1 = rng.Next(1, 2);
                    candy = new int[] { g4, y4, r3, b2, p1 };
                    await Context.Channel.SendMessageAsync($"Ding Dong...\nYou have received:\n**{g4}**: Green Candy\n**{y4}**: Yellow Candy\n**{r3}**: Red Candy\n**{b2}**: Blue Candy\n**{p1}**: Purple Candy");
                    break;
            }

            Database.UpdateCandyAmount(candy, Context.User.Id);


        }

        [Command("profile")]
        public async Task Profile()
        {
            if (!Database.UserRegistered(Context.User.Id))
                Database.CreateUser(BotMain.Client.GetUser(Context.User.Id));

            JObject obj = Database.GetUserData(Context.User.Id);

            EmbedFieldBuilder candyField = new EmbedFieldBuilder()
                .WithName("Candy Amount");
            candyField.Value = $"<:GreenCandy:896448862963761152> : **{obj["Candy"][0]}**\n" +
                               $"<:YellowCandy:896448997345087558> : **{obj["Candy"][1]}**\n" +
                               $"<:RedCandy:896448997428961381> : **{obj["Candy"][2]}**\n" +
                               $"<:BlueCandy:896448997550587915> : **{obj["Candy"][3]}**\n" +
                               $"<:PurpleCandy:896448997298954270> : **{obj["Candy"][4]}**\n";
            candyField.Build();


            EmbedBuilder profile = new EmbedBuilder()
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithTitle($"{Context.User.Username}'s Halloween Profile")
                .WithFields(candyField);

            await Context.Channel.SendMessageAsync(embed: profile.Build());

        }

        [Command("ping")]
        public async Task Ping() => await Context.Channel.SendMessageAsync("Boo!");
    }
}
