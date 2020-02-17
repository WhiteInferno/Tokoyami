using Microsoft.EntityFrameworkCore;
using Tokoyami.EF;

namespace Tokoyami.Context
{
    public class TokoyamiDbContext : DbContext
    {
        public TokoyamiDbContext() {}

        public TokoyamiDbContext(DbContextOptions<TokoyamiDbContext> options)
            : base(options) {}

        public DbSet<Emote> Emotes { get; set; }
    }
}
