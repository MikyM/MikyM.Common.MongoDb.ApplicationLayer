using Microsoft.Extensions.Options;
using MikyM.Common.ApplicationLayer;
using MikyM.Common.MongoDb.DataAccessLayer;

namespace MikyM.Common.MongoDb.ApplicationLayer;

/// <summary>
/// Registration extension configuration
/// </summary>
public sealed class MongoDbServiceApplicationConfiguration : DataServiceConfigurationBase, IOptions<MongoDbServiceApplicationConfiguration>
{
    internal MongoDbServiceApplicationConfiguration(ApplicationConfiguration config)
    {
        Config = config;
    }
    public ApplicationConfiguration Config { get; set; }

    /// <inheritdoc />
    public MongoDbServiceApplicationConfiguration Value => this;
}