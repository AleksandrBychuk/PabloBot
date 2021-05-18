using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using System.IO;
using Newtonsoft.Json;
using PabloBot.Services;
using Microsoft.Extensions.Logging;

namespace PabloBot
{
    class PabloBot
    {
        DiscordClient Client { get; set; }
        CommandsNextExtension Commands { get; set; }
        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("../../../config.json"))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
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

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient k, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
