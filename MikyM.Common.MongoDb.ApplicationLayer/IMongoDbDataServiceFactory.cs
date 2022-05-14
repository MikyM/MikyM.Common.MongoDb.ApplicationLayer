using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.MongoDb.DataAccessLayer.UnitOfWork;

namespace MikyM.Common.MongoDb.ApplicationLayer;

/// <summary>
/// Factory for <see cref="IMongoDbUnitOfWork"/>
/// </summary>
public interface IMongoDbDataServiceFactory
{
    /// <summary>
    /// Gets a <see cref="IReadOnlyMongoDbDataService{TEntity}"/> for the default database
    /// </summary>
    /// <returns><see cref="IMongoDbUnitOfWork"/> for the default database</returns>
    IReadOnlyMongoDbDataService<TEntity> GetReadOnlyDataService<TEntity>() where TEntity : SnowflakeMongoDbEntity;
    /// <summary>
    /// Gets a <see cref="IReadOnlyMongoDbDataService{TEntity}"/> for the given database
    /// </summary>
    /// <param name="database">Name of the database to use</param>
    /// <returns><see cref="IMongoDbUnitOfWork"/> for the given database</returns>
    IReadOnlyMongoDbDataService<TEntity> GetReadOnlyDataService<TEntity>(string database) where TEntity : SnowflakeMongoDbEntity;
    /// <summary>
    /// Gets a <see cref="ICrudMongoDbDataService{TEntity}"/> for the default database
    /// </summary>
    /// <returns><see cref="IMongoDbUnitOfWork"/> for the default database</returns>
    ICrudMongoDbDataService<TEntity> GetCrudDataService<TEntity>() where TEntity : SnowflakeMongoDbEntity;
    /// <summary>
    /// Gets a <see cref="IReadOnlyMongoDbDataService{TEntity}"/> for the given database
    /// </summary>
    /// <param name="database">Name of the database to use</param>
    /// <returns><see cref="ICrudMongoDbDataService{TEntity}"/> for the given database</returns>
    ICrudMongoDbDataService<TEntity> GetCrudDataService<TEntity>(string database) where TEntity : SnowflakeMongoDbEntity;
    /// <summary>
    /// Gets a given data service
    /// </summary>
    /// <returns>MongoDb data service</returns>
    TDataService GetDataService<TDataService>() where TDataService : class, IMongoDbDataServiceBase;
    /// <summary>
    /// Gets a given data service for the given database
    /// </summary>
    /// <param name="database">Name of the database to use</param>
    /// <returns>MongoDb data service for the given database</returns>
    TDataService GetDataService<TDataService>(string database) where TDataService : class, IMongoDbDataServiceBase;
}