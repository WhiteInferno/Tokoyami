using Microsoft.EntityFrameworkCore;
using Tokoyami.EF;
using Tokoyami.EF.Hangman.Entities;
using Tokoyami.EF.Music;

namespace Tokoyami.Context
{
    public class TokoyamiDbContext : DbContext
    {
        public TokoyamiDbContext() {}

        public TokoyamiDbContext(DbContextOptions<TokoyamiDbContext> options)
            : base(options) {}

        public DbSet<Emote> Emotes { get; set; }

        public DbSet<Word> Words { get; set; }

        public DbSet<Playlist> Playlists { get; set; }
    }
}
