using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot.Services.Models.Profiles.Services
{
    public interface IProfileService
    {
        Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId);
    }
    public class ProfileService : IProfileService
    {
        private readonly PabloContext _context;

        public ProfileService(PabloContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId)
        {
            var profile = await ((IAsyncEnumerable<Profile>)_context.Profiles).Where(p => p.GuildId == guildId).FirstOrDefaultAsync(p => p.DiscordId == discordId).ConfigureAwait(false);

            if (profile != null) return profile;

            profile = new Profile { DiscordId = discordId, GuildId = guildId };
            _context.Add(profile);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return profile;
        }
    }
}
