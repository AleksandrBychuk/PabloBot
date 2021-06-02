using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using System.IO;
using System.Diagnostics;

namespace PabloBot.Modules
{
    public class DefaultCommands : BaseCommandModule
    {
        public DefaultCommands()
        {
            
        }

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("pong").ConfigureAwait(false);
        }


        [Command("plus")]
        [Description("Складывает два числа. Пример: ?add 2 3.")]
        public async Task Plus(CommandContext ctx, [Description("Первое число")] int numberOne, [Description("Второе число")] int numberTwo)
        {
            await ctx.Channel.SendMessageAsync((numberOne + numberTwo).ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Роль бота должна быть выше роли которая выдается, роль можно поднять в списке всех ролей.
        /// </summary>
        [RequireOwner]
        [Command("give")]
        public async Task Give(CommandContext ctx)
        {
            var role = ctx.Guild.GetRole(711590788341170247);
            await ctx.Channel.SendMessageAsync("Success!").ConfigureAwait(false);
            await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Голосование",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var optionBuilder in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(optionBuilder).ConfigureAwait(false);
            }
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("Нет стабильного соединения Lavalink!");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            var channel = ctx.Member.Guild.GetChannel(ctx.Member.VoiceState.Channel.Id);

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Данный канал не является голосовым!");
                return;
            }


            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("Вас не в голосовом канале.");
                return;
            }

            await node.ConnectAsync(channel);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink не подключен.");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Ошибка поиска {search}.");
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.RespondAsync($"Now playing {track.Title}!");
        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var channel = ctx.Member.Guild.GetChannel(ctx.Member.VoiceState.Channel.Id);
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("Нет стабильного соединения Lavalink!");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Данный канал не является голосовым!");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink не подключен!");
                return;
            }

            await conn.DisconnectAsync();
        }
    }
}
