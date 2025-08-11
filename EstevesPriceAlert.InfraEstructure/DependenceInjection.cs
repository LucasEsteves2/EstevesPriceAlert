using EstevesPriceAlert.Core.Repositories;
using EstevesPriceAlert.InfraEstructure.Persistence;
using EstevesPriceAlert.InfraEstructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace EstevesPriceAlert.InfraEstructure
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return configuration.GetSection("Mongo").Get<MongoDbOptions>();
            });


            services.AddSingleton<IMongoClient>(sp => {
                var options = sp.GetService<MongoDbOptions>();

                return new MongoClient(options.ConnectionString);
            });

            services.AddTransient(sp =>
            {
                var options = sp.GetRequiredService<MongoDbOptions>();
                var mongoClient = sp.GetRequiredService<IMongoClient>();

                return mongoClient.GetDatabase(options.Database);
            });


            return services;
        }
    }
}