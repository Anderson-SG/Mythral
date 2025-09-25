using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mythral.Server.Infrastructure.Options;
using StackExchange.Redis;

namespace Mythral.Server.Infrastructure.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration
                .GetSection(RedisOptions.SectionName)
                .GetValue<string>(nameof(RedisOptions.Configuration));
            options.InstanceName = "mythral:";
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redis = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(redis.Configuration);
        });

        return services;
    }
}
