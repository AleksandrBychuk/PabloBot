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
            try {

                if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
                {
                    await ctx.RespondAsync("Вас нет в голосовом канале.");
                    return;
                }

                await ctx.RespondAsync("Now playing Two Tone Rebel — E - Dubble").ConfigureAwait(false);

                await ctx.Member.VoiceState.Channel.ConnectAsync();

            }
            catch (Exception e) { Console.WriteLine(e); }
        }
    }
}
