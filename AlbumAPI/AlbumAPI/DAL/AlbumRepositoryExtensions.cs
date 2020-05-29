using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumAPI.DAL
{
    public static class AlbumRepositoryExtensions
    {
        public static void AddAlbumRepository(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(ServiceProvider =>
            {
                var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Mongo");
                return new MongoClient(connectionString);
            });
            services.AddSingleton(ServiceProvider =>
            {
                var client = ServiceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase("Albums");
            });

            services.AddTransient<IAlbumRepository, AlbumRepository>();
            
            services.AddScoped<IAlbumRepository, AlbumRepository>();
            var pack = new ConventionPack
            {
                new ElementNameConvention(), };
            ConventionRegistry.Register("Conventions", pack, _ => true);
        }
    }
}
