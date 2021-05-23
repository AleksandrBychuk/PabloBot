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

        [Command("warn"), Description("Команда для выдачи варна пользователя.")]
        [RequireRoles(RoleCheckMode.Any, "Администрация", "Модерация")]
        public async Task Warn(CommandContext ctx, [DescriptionAttribute("Выбранный пользователь для выдачи варна.")] DiscordMember member)
        {
            var userToWarn = await _context.Profiles.FirstOrDefaultAsync(p => p.DiscordId == member.Id && p.GuildId == member.Guild.Id);

            if (userToWarn != null)
            {
                userToWarn.Warns += 1;
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
