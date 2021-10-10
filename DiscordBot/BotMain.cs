using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HalloweenBot.DiscordBot
{
    public class BotMain
    {
        private static DiscordSocketClient _client;
        private CommandService _commands;
        public static DiscordSocketClient Client => _client ?? (_client = new DiscordSocketClient());

        public async Task RunBotAsync()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(config);
            _client.Log += Log;

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            _client.MessageReceived += Client_MessageReceived;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            Program main = new Program();

            var Token = "ODk1ODY2MDA4MTE4NjM2NjA0.YV-yPA.JZdnFm3ukMYQKCbYLhvd8a5JW-Y";
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            await _client.SetStatusAsync(UserStatus.Online);



            await Task.Delay(-1);

        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {
            Program main = new Program();

            var Message = message as SocketUserMessage;
            if (_client != null && Message != null)
            {


                int args = 0;
                var ctx = new SocketCommandContext(_client, Message);
                var prefix = "+";
                if (Message.HasStringPrefix(prefix, ref args))
                {

                    var result = await _commands.ExecuteAsync(ctx, args, null);
                    if (ctx.Message == null || ctx.Message.Content == string.Empty)
                        return;
                    if (ctx.User.IsBot)
                        return;
                }


            }
        }
    }
}
