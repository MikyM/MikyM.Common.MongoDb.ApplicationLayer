using Autofac;
using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.MongoDb.DataAccessLayer.UnitOfWork;

namespace MikyM.Common.MongoDb.ApplicationLayer;

/// <summary>
/// Factory for <see cref="IMongoDbUnitOfWork"/>
/// </summary>
public class MongoDbDataServiceFactory : IMongoDbDataServiceFactory
{
    /// <summary>
    /// Inner scope
    /// </summary>
    private readonly ILifetimeScope _lifetimeScope;
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoDbUnitOfWorkFactory"/>
    /// </summary>
    /// <param name="lifetimeScope">Lifetime scope for this instance</param>
    public MongoDbDataServiceFactory(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    /// <inheritdoc />
    public IReadOnlyMongoDbDataService<TEntity> GetReadOnlyDataService<TEntity>() where TEntity : SnowflakeMongoDbEntity
        => _lifetimeScope.Resolve<IReadOnlyMongoDbDataService<TEntity>>();

    /// <inheritdoc />
    public IReadOnlyMongoDbDataService<TEntity> GetReadOnlyDataService<TEntity>(string database) where TEntity : SnowflakeMongoDbEntity
        => _lifetimeScope.ResolveNamed<IReadOnlyMongoDbDataService<TEntity>>(database);

    /// <inheritdoc />
    public ICrudMongoDbDataService<TEntity> GetCrudDataService<TEntity>() where TEntity : SnowflakeMongoDbEntity
        => _lifetimeScope.Resolve<ICrudMongoDbDataService<TEntity>>();

    /// <inheritdoc />
    public ICrudMongoDbDataService<TEntity> GetCrudDataService<TEntity>(string database) where TEntity : SnowflakeMongoDbEntity
        => _lifetimeScope.ResolveNamed<ICrudMongoDbDataService<TEntity>>(database);

    /// <inheritdoc />
    public TDataService GetDataService<TDataService>() where TDataService : class, IMongoDbDataServiceBase
        => _lifetimeScope.Resolve<TDataService>();

    /// <inheritdoc />
    public TDataService GetDataService<TDataService>(string database) where TDataService : class, IMongoDbDataServiceBase
        => _lifetimeScope.ResolveNamed<TDataService>(database);
}