using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.MongoDb.ApplicationLayer.Interfaces;

/// <summary>
/// CRUD data service
/// </summary>
public interface ICrudMongoDbDataService<TEntity> : IReadOnlyMongoDbDataService<TEntity>
    where TEntity : SnowflakeMongoDbEntity
{
    /// <summary>
    /// Adds an entry
    /// </summary>
    /// <typeparam name="TPost">Type of the entry</typeparam>
    /// <param name="entry">Entry to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> with the Id of the newly created entity</returns>
    Task<Result<string>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null) where TPost : class;

    /// <summary>
    /// Adds a range of entries
    /// </summary>
    /// <typeparam name="TPost">Type of the entries</typeparam>
    /// <param name="entries">Entries to add</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> with <see cref="IEnumerable{T}"/> containing Ids of the newly created entities</returns>
    Task<Result<IEnumerable<string>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries, bool shouldSave = false,
        string? userId = null) where TPost : class;
    
    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteAsync(string id, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Deletes a range of entities
    /// </summary>
    /// <param name="ids">Ids of the entities to delete</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DeleteRangeAsync(IEnumerable<string> ids, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Disables an entity
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entry">Entry to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, string? userId = null)
        where TDisable : class;

    /// <summary>
    /// Disables an entity
    /// </summary>
    /// <param name="id">Id of the entity to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableAsync(string id, bool shouldSave = false, string? userId = null);

    /// <summary>
    /// Disables a range of entities
    /// </summary>
    /// <typeparam name="TDisable">Type of the entry</typeparam>
    /// <param name="entries">Entries to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false,
        string? userId = null) where TDisable : class;

    /// <summary>
    /// Disables a range of entities
    /// </summary>
    /// <param name="ids">Ids of the entities to disable</param>
    /// <param name="shouldSave">Whether to automatically call SaveChangesAsync() </param>
    /// <param name="userId">Optional Id of the user that is responsible for the changes</param>
    /// <returns><see cref="Result"/> of the operation</returns>
    Task<Result> DisableRangeAsync(IEnumerable<string> ids, bool shouldSave = false, string? userId = null);
}