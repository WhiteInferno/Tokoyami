using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.EF.Music;

namespace Tokoyami.Business.Contract
{
    interface IMusicService
    {
        Task Create(Playlist entity);
        Playlist GetById(int id);
        IEnumerable<Playlist> GetAll();
        Task Remove(int id);
        Task Update(Playlist updatedEntity);
    }
}
