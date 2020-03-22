using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokoyami.Business.Business;
using Tokoyami.Context;
using Tokoyami.Context.Configuration;
using Tokoyami.EF.Music;
using Xunit;

namespace Tokoyami.UnitTest
{
    public class MusicTest
    {
        private const string _cnString = "Server=DESKTOP-85T3VNI;Database=Tokoyami;Trusted_Connection=True;ConnectRetryCount=0";

        private TokoyamiDbContext GetContext()
        {
            var dbOption = new DbContextOptionsBuilder<TokoyamiDbContext>().UseSqlServer(_cnString).Options;
            var dbContext = new TokoyamiDbContext(dbOption);
            return dbContext;
        }

        [Fact]
        public async Task CreatePlaylist()
        {
            MusicService _service = new MusicService(new UnitOfWork(GetContext()));
            Playlist entity = new Playlist() { Author = "WhiteInferno#2755", Name="MyPlaylist", Urls="" };
            var task = _service.Create(entity);
            await task;
            Debug.WriteLine("The playlist has been added successfully!");
        }

        [Fact]
        public void GetPlaylist()
        {
            MusicService _service = new MusicService(new UnitOfWork(GetContext()));
            Playlist entity = _service.GetById(1);
            Assert.NotNull(entity);
        }

        [Fact]
        public void GetPlaylists()
        {
            MusicService _service = new MusicService(new UnitOfWork(GetContext()));
            List<Playlist> entities = _service.GetAll().ToList();
            Assert.NotNull(entities);
        }

        [Fact]
        public async Task UpdatePlaylist()
        {
            MusicService _service = new MusicService(new UnitOfWork(GetContext()));
            Playlist entity = new Playlist() { Id=1, Author = "WhiteInferno#2755", Name = "MyPlaylist", Urls = "https://youtu.be/2_R3Q5cgSsY|https://www.youtube.com/watch?v=FW5NKlh88ik" };
            var task = _service.Update(entity);
            await task;

            Debug.WriteLine("The playlist has been updated successfully!");
        }

        [Fact]
        public async Task DeletePlaylist()
        {
            MusicService _service = new MusicService(new UnitOfWork(GetContext()));
            var task = _service.Remove(1);
            await task;

            Debug.WriteLine("The playlist has been deleted successfully!");
        }
    }
}
