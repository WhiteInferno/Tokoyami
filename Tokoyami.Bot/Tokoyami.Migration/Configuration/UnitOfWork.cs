using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.EF.Hangman.Entities;
using Tokoyami.EF.Music;

namespace Tokoyami.Context.Configuration
{
    public interface IUnitOfWork
    {
        Task Save();
    }

    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private TokoyamiDbContext context = null;
        private bool disposed = false;
        private TokoyamiRepository<Word> _wordRepository;
        private TokoyamiRepository<Playlist> _playlistRepository;

        public TokoyamiRepository<Word> wordRepository
        {
            get
            {
                if(this._wordRepository == null)
                {
                    this._wordRepository = new TokoyamiRepository<Word>(context);
                }
                return this._wordRepository;
            } 
        }

        public TokoyamiRepository<Playlist> playlistRepository
        {
            get
            {
                if(this._playlistRepository == null)
                {
                    this._playlistRepository = new TokoyamiRepository<Playlist>(context);
                }
                return this._playlistRepository;
            }
        }

        public UnitOfWork(TokoyamiDbContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
