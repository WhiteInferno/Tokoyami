using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tokoyami.Context;

namespace Tokoyami.Context.Configuration
{
    public class TokoyamiDbContextFactory : IDesignTimeDbContextFactory<TokoyamiDbContext>
    {
        public TokoyamiDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TokoyamiDbContext>();

            var configuration = AppConfiguration.Get(ContentDirectoryFinder.CalculateContentRootFolder());

            var connectionString = configuration.GetConnectionString("Default");

            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(TokoyamiDbContext).Assembly.GetName().Name));

            return new TokoyamiDbContext(builder.Options);
        }
    }
}