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
            };
            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

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

            Client.ConnectAsync();
        }
        private Task OnClientReady(DiscordClient k, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
