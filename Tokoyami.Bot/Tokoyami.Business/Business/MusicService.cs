using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Business.Contract;
using Tokoyami.Context.Configuration;
using Tokoyami.EF.Music;

namespace Tokoyami.Business.Business
{
    public class MusicService : IMusicService
    {
        private readonly UnitOfWork unitOfWork;

        private readonly TokoyamiRepository<Playlist> playlistRepository;

        public MusicService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            playlistRepository = unitOfWork.playlistRepository;
        }

        public async Task Create(Playlist entity)
        {
            this.playlistRepository.Insert(entity);
            await this.unitOfWork.Save();
        }

        public Playlist GetById(int id) => this.playlistRepository.GetById(id);

        public IEnumerable<Playlist> GetAll() => this.playlistRepository.Get();

        public async Task Remove(int id)
        {
            this.playlistRepository.Delete(id);
            await this.unitOfWork.Save();
        }

        public async Task Update(Playlist updatedEntity)
        {
            this.playlistRepository.Update(updatedEntity);
            await this.unitOfWork.Save();
        }
    }
}
