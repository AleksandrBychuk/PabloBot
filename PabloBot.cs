using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
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
using PabloBot.Services.Models;
using Microsoft.EntityFrameworkCore;
using PabloBot.Services.Models.Profiles;
using System.Data;

namespace PabloBot
{
    class PabloBot
    {
        public DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public VoiceNextExtension Voice { get; set; }      
        public InteractivityExtension Interactivity { get; set; }
        private readonly PabloContext _context;

        public PabloBot(IServiceProvider services)
        {
            _context = new PabloContext(services.GetRequiredService<DbContextOptions<PabloContext>>());
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

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                Services = services
            };

            var voiceConfig = new VoiceNextConfiguration
            {
                EnableIncoming = false,
                AudioFormat = AudioFormat.Default
            };

            var lavalink = Client.UseLavalink();

            Voice = Client.UseVoiceNext(voiceConfig);
            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<DefaultCommands>();
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<AdminCommands>();

            Client.ConnectAsync();
            LvlConn(lavalink, lavalinkConfig).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task LvlConn(LavalinkExtension lvl, LavalinkConfiguration conf)
        {
            await Task.Delay(5000);
            await lvl.ConnectAsync(conf);
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient s, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnMemberInvited(DiscordClient s, GuildMemberAddEventArgs e)
        {
            ProfileService profileService = new ProfileService(_context);
            await profileService.GetOrCreateProfileAsync(e.Member.Id, e.Guild.Id).ConfigureAwait(false);
            await e.Guild.GetChannel(711590425483411509).SendMessageAsync($"Привет {e.Member.Mention}, чувствуй себя как дома!").ConfigureAwait(false);
        }

        private async Task OnMemberUninvited(DiscordClient s, GuildMemberRemoveEventArgs e)
        {
            await e.Guild.GetChannel(711590425483411509).SendMessageAsync($"Пока {e.Member.Mention}. Надеюсь ты ещё вернешься к нам!").ConfigureAwait(false);
        }

        public async Task OnMessageReceived(DiscordClient s, MessageCreateEventArgs e)
        {
            var badWord = await ((IAsyncEnumerable<Badword>)_context.Badwords).ToListAsync().ConfigureAwait(false);
            foreach (var bw in badWord)
            {
                if (e.Message.Content.ToLower().Contains(bw.Word.ToString()))
                {
                    await e.Message.DeleteAsync();
                    var hardMessage = await e.Channel.SendMessageAsync($"{e.Author.Mention} не стоит писать подобные слова!").ConfigureAwait(false);
                    await Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(5000);
                        await e.Channel.DeleteMessageAsync(hardMessage).ConfigureAwait(false);
                    });
                }
            }
            await Task.Factory.StartNew(async () => 
            {
                if (!e.Author.IsBot)
                {
                    Profile profile = await ((IAsyncEnumerable<Profile>)_context.Profiles).Where(p => p.GuildId == e.Guild.Id).FirstOrDefaultAsync(p => p.DiscordId == e.Author.Id).ConfigureAwait(false);
                    if (profile != null)
                    {
                        profile.Xp += 10;
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            });
        }
    }
}
