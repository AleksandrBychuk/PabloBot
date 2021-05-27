using Microsoft.EntityFrameworkCore;
using PabloBot.Services.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot.Services.Models
{
    public class PabloContext : DbContext
    {
        public PabloContext(DbContextOptions<PabloContext> options) : base(options) { }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Badword> Badwords { get; set; }
    }
}
