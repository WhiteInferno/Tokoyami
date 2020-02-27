using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tokoyami.Business.Business;
using Tokoyami.Context;
using Tokoyami.Context.Configuration;
using Xunit;

namespace Tokoyami.UnitTest
{
    public class HangmanTest
    {
        private const string _cnString = "Server=DESKTOP-85T3VNI;Database=Tokoyami;Trusted_Connection=True;ConnectRetryCount=0";

        private TokoyamiDbContext GetContext()
        {
            var dbOption = new DbContextOptionsBuilder<TokoyamiDbContext>().UseSqlServer(_cnString).Options;
            var dbContext = new TokoyamiDbContext(dbOption);
            return dbContext;
        }

        [Fact]
        public async Task CreateWordAsync()
        {
            HangmanService _service = new HangmanService(new UnitOfWork(GetContext()));
            var task = _service.Create("Yota");
            await task;
        }

        [Fact]
        public async Task DeleteWord()
        {
            try
            {
                HangmanService _service = new HangmanService(new UnitOfWork(GetContext()));
                await _service.Remove("Maguwhite");
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.False(false);
            }
        }

        [Fact]
        public void GetRandomWord()
        {
            try
            {
                HangmanService _service = new HangmanService(new UnitOfWork(GetContext()));
                var task = _service.GetRandom();
                Assert.NotNull(task);
                Debug.WriteLine(task.Descripcion);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.False(false);
            }
        }
    }
}
