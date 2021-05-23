using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using System.IO;
using Newtonsoft.Json;
using PabloBot.Services;
using Microsoft.Extensions.Logging;
using PabloBot.Modules;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PabloBot.Services.Models.Profiles.Services;
using DSharpPlus.Entities;
using System.Threading;

namespace PabloBot
{
    class PabloBot
    {
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public InteractivityExtension Interactivity { get; set; }

        public PabloBot(IServiceProvider services)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = sr.ReadToEnd();
                }
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            };
            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageReceived;
            Client.GuildMemberAdded += OnMemberInvited;
            Client.GuildMemberRemoved += OnMemberUninvited;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<DefaultCommands>();
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<AdminCommands>();

            Client.ConnectAsync();
        }

        private Task OnClientReady(DiscordClient s, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnMemberInvited(DiscordClient s, GuildMemberAddEventArgs e)
        {
            try
            {
                await e.Guild.GetChannel(711590425483411509).SendMessageAsync($"Здарова {e.Member.Mention}, и пошёл нахуй!").ConfigureAwait(false);
            }
            catch (Exception d)
            {
                Console.WriteLine("ОШИБКА С ЗАХОДОМ: " + d);
            }
        }

        private async Task OnMemberUninvited(DiscordClient s, GuildMemberRemoveEventArgs e)
        {
            try
            {
                await e.Guild.GetChannel(711590425483411509).SendMessageAsync($"Пока {e.Member.Mention}, и пошёл нахуй!").ConfigureAwait(false);
            }
            catch (Exception d)
            {
                Console.WriteLine("ОШИБКА С ВЫХОДОМ: " + d);
            }
        }

        private async Task OnMessageReceived(DiscordClient s, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().Contains("pidor"))
            {
                await e.Message.DeleteAsync();
                var hardMessage = await e.Channel.SendMessageAsync($"{e.Author.Mention} stop fucking write bad words. Fucking slut!").ConfigureAwait(false);
                await Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(5000);
                    await e.Channel.DeleteMessageAsync(hardMessage).ConfigureAwait(false);
                });
            }
        }
    }
}
