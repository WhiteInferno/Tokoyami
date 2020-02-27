using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.EF.Hangman.Entities;

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
