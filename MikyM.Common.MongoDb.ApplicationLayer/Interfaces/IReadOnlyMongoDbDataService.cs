using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.MongoDb.DataAccessLayer.Context;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.MongoDb.ApplicationLayer.Interfaces;

/// <summary>
/// Read-only data service
/// </summary>
/// <typeparam name="TEntity">Type of the entity to create the service for, must derive from <see cref="SnowflakeMongoDbEntity"/></typeparam>
/// <typeparam name="TContext">Mongo DB context type</typeparam>
public interface IReadOnlyMongoDbDataService<TEntity, TContext> : IMongoDbDataServiceBase<TContext>
    where TEntity : SnowflakeMongoDbEntity where TContext : MongoDbContext
{
    /// <summary>
    /// Gets an entity based on given primary key values
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TEntity>> GetAsync(string id);

    /// <summary>
    /// Gets an entity based on given primary key values and maps it to another type
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method</param>
    /// <param name="id">Primary key value</param>
    /// <returns><see cref="Result"/> containing the result of this operation, with the found entity if any</returns>
    Task<Result<TGetResult>> GetAsync<TGetResult>(string id, bool shouldProject = false) where TGetResult : class;

    /// <summary>
    /// Gets all entities and maps them to another type
    /// </summary>
    /// <param name="shouldProject">Whether to use AutoMappers ProjectTo method</param>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false)
        where TGetResult : class;

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <returns><see cref="Result"/> with <see cref="IReadOnlyList{T}"/> containing the result of this operation, with the found entities if any</returns>
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync();
}