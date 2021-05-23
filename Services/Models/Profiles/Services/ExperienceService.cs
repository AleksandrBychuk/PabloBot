using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot.Services.Models.Profiles.Services
{

    public interface IExperienceService
    {
        Task GrantXpAsync(ulong discordId, ulong guildId, int xpAmount);
    }
    public class ExperienceService : IExperienceService
    {
        public Task GrantXpAsync(ulong discordId, ulong guildId, int xpAmount)
        {
            throw new NotImplementedException();
        }
    }
}
