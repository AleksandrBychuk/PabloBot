using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot.Modules
{
    public class DefaultCommands : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("pong").ConfigureAwait(false);
        }


        [Command("plus")]
        [Description("Складывает два числа. Пример: ?add 2 3.")]
        //[RequireRoles(RoleCheckMode.All, "Модерация")]
        public async Task Plus(CommandContext ctx, [Description("Первое число")] int numberOne, [Description("Второе число")] int numberTwo)
        {
            await ctx.Channel.SendMessageAsync((numberOne + numberTwo).ToString()).ConfigureAwait(false);
        }
    }
}
