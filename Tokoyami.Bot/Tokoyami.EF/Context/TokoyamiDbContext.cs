using System.Data.Entity;
using Tokoyami.EF.Entities;

namespace Tokoyami
{
    public class TokoyamiDbContext : DbContext
    {
        public DbSet<Words> Words { get; set; }

        public DbSet<Emote> Emotes { get; set; }
    }
}
