using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PabloBot.Services.Models.Profiles.Services;
using PabloBot.Services.Models;
using PabloBot.Services.Models.Profiles;

namespace PabloBot.Modules
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx, DiscordMember member)
        {
            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            try
            {
                Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id);
                DiscordMember member = ctx.Guild.Members[profile.DiscordId];
                var profileEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{member.DisplayName}'s Profile",
                    ImageUrl = member.AvatarUrl
                };

                profileEmbed.AddField("Level", profile.Level.ToString());

                await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e);
            }
        }
    }
}
