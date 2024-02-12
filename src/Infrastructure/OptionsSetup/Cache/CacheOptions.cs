namespace CleanArchitechture.Infrastructure.OptionsSetup.Cache;

internal class CacheOptions
{
    public const string Settings = nameof(CacheOptions);

    public int SlidingExpiration { get; set; } = 10;
}
