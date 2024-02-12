using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CleanArchitechture.Infrastructure.OptionsSetup.Cache;

internal class CacheOptionsSetup(IConfiguration configuration) 
    : IConfigureOptions<CacheOptions>
{

    public void Configure(CacheOptions cacheOptions)
    {
        configuration.GetSection(CacheOptions.Settings).Bind(cacheOptions);
    }
}
