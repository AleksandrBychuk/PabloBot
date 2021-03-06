using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PabloBot.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PabloBot.Modules
{
    public class AdminCommands : BaseCommandModule
    {
        private readonly PabloContext _context;

        public AdminCommands(PabloContext context)
        {
            _context = context;
        }

        [Command("ban"), Description("Команда для бана пользователя.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Ban(CommandContext ctx, [DescriptionAttribute("Выбранный пользователь для бана.")] DiscordMember member)
        {
            await ctx.Guild.BanMemberAsync(member).ConfigureAwait(false);
            
        }

        [Command("unban"), Description("Команда для разбана пользователя.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Unban(CommandContext ctx, [DescriptionAttribute("Выбранный пользователь для разбана.")] DiscordMember member)
        {
            await ctx.Guild.UnbanMemberAsync(member).ConfigureAwait(false);

        }

        [Command("kick"), Description("Команда для кика пользователя.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Kick(CommandContext ctx, [DescriptionAttribute("Выбранный пользователь для кика с канала.")] DiscordMember member)
        {
            if (member.Guild != ctx.Guild)
                await ctx.Channel.SendMessageAsync("Пользователя нет в данном канале!").ConfigureAwait(false);
            else
                await member.RemoveAsync().ConfigureAwait(false);
        }

        [Command("bw"), Description("Команда для добавления слов в черный список.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Badwords(CommandContext ctx, [DescriptionAttribute("Слово для добавления.")] string word)
        {
            var dWord = await _context.Badwords.FirstOrDefaultAsync(b => b.Word == word).ConfigureAwait(false);
            if (dWord != null)
            {
                await ctx.RespondAsync("Слово есть уже в черном списке!").ConfigureAwait(false);
                return;
            }
            if (word == null)
            {
                await ctx.RespondAsync("Введите слово!").ConfigureAwait(false);
                return;
            }
            
            await _context.Badwords.AddAsync(new Badword { Word = word }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            await ctx.RespondAsync($"Слово /{word}/ успешно добавлено в черный список!").ConfigureAwait(false);
        }

        [Command("warn"), Description("Команда для выдачи варна пользователя.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Warn(CommandContext ctx, [DescriptionAttribute("Выбранный пользователь для выдачи варна.")] DiscordMember member, params string[] reason)
        {
            var userToWarn = await _context.Profiles.FirstOrDefaultAsync(p => p.DiscordId == member.Id && p.GuildId == member.Guild.Id);

            if (userToWarn != null)
            {
                var text = reason.Select(x => x.ToString());
                userToWarn.Warns += 1;
                await ctx.Channel.SendMessageAsync($"{member.Mention} получает предупреждение за {string.Join(" ", reason)}").ConfigureAwait(false);
                if (userToWarn.Warns >= 3)
                {
                    await ctx.Guild.BanMemberAsync(member).ConfigureAwait(false);
                    userToWarn.Warns = 0;
                }
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
                await ctx.Channel.SendMessageAsync("Пользователь не найден!").ConfigureAwait(false);

        }
    }
}
