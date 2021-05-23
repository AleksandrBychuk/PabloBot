using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot.Services.Models.Profiles
{
    public class Profile : Entity
    {
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }
        public int Xp { get; set; }
        public int Level => Xp / 100;
        public int Warns { get; set; }
    }
}
