using AutoMapper;
using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.MongoDb.DataAccessLayer.Context;
using MikyM.Common.MongoDb.DataAccessLayer.Repositories;
using MikyM.Common.MongoDb.DataAccessLayer.UnitOfWork;
using MikyM.Common.Utilities.Results;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MikyM.Common.MongoDb.ApplicationLayer.Services;

/// <summary>
/// CRUD data service
/// </summary>
/// <inheritdoc cref="ICrudMongoDbDataService{TEntity,TContext}"/>
public class CrudMongoDbDataService<TEntity, TContext> : ReadOnlyMongoDbDataService<TEntity, TContext>, ICrudMongoDbDataService<TEntity, TContext>
    where TEntity : SnowflakeMongoDbEntity where TContext : MongoDbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="CrudMongoDbDataService{TEntity,TContext}"/>
    /// </summary>
    /// <param name="mapper">Mapper instance</param>
    /// <param name="uof">Unit of work instance</param>
    public CrudMongoDbDataService(IMapper mapper, IMongoDbUnitOfWork<TContext> uof) : base(mapper, uof)
    {
    }

    /// <inheritdoc />
    protected override IRepositoryBase BaseMongoDbRepository => UnitOfWork.GetRepository<MongoDbRepository<TEntity>>();
    /// <summary>
    /// Gets the CRUD version of the <see cref="BaseMongoDbRepository"/> (essentially casts it for you)
    /// </summary>
    protected MongoDbRepository<TEntity> Repository => (MongoDbRepository<TEntity>)BaseMongoDbRepository;


    /// <inheritdoc />
    public virtual async Task<Result<string>> AddAsync<TPost>(TPost entry, bool shouldSave = false, string? userId = null) where TPost : class
    {
        if (entry  is null) throw new ArgumentNullException(nameof(entry));

        TEntity entity;
        if (entry is TEntity rootEntity)
        {
            entity = rootEntity;
            await Repository.AddAsync(entity);
        }
        else
        {
            entity = Mapper.Map<TEntity>(entry);
            await Repository.AddAsync(entity);
        }

        if (!shouldSave) 
            return string.Empty;

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);
        
        return entity.ID;
    }

    /// <inheritdoc />
    public virtual async Task<Result<IEnumerable<string>>> AddRangeAsync<TPost>(IEnumerable<TPost> entries,
        bool shouldSave = false, string? userId = null) where TPost : class
    {
        if (entries  is null) throw new ArgumentNullException(nameof(entries));

        List<TEntity> entities;

        if (entries is IEnumerable<TEntity> rootEntities)
        {
            entities = rootEntities.ToList();
            await Repository.AddRangeAsync(entities);
        }
        else
        {
            entities = Mapper.Map<List<TEntity>>(entries);
            await Repository.AddRangeAsync(entities);
        }

        if (!shouldSave) 
            return new List<string>();
        
        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return entities.Select(e => e.ID).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(string id, bool shouldSave = false, string? userId = null)
    {
        await Repository.DeleteAsync(id);

        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteRangeAsync(IEnumerable<string> ids, bool shouldSave = false, string? userId = null)
    {
        if (ids  is null) throw new ArgumentNullException(nameof(ids));

        await Repository.DeleteRangeAsync(ids);

        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync(string id, bool shouldSave = false, string? userId = null)
    {
        await Repository
            .DisableAsync(id);

        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableAsync<TDisable>(TDisable entry, bool shouldSave = false, string? userId = null) where TDisable : class
    {
        switch (entry)
        {
            case null:
                throw new ArgumentNullException(nameof(entry));
            case TEntity rootEntity:
                await Repository.DisableAsync(rootEntity);
                break;
            default:
                await Repository.DisableAsync(Mapper.Map<TEntity>(entry));
                break;
        }

        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync(IEnumerable<string> ids, bool shouldSave = false, string? userId = null)
    {
        if (ids  is null) throw new ArgumentNullException(nameof(ids));

        await Repository
            .DisableRangeAsync(ids);

        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> DisableRangeAsync<TDisable>(IEnumerable<TDisable> entries, bool shouldSave = false, string? userId = null)
        where TDisable : class
    {
        switch (entries)
        {
            case null:
                throw new ArgumentNullException(nameof(entries));
            case IEnumerable<TEntity> rootEntities:
                await Repository.DisableRangeAsync(rootEntities);
                break;
            default:
                await Repository
                    .DisableRangeAsync(Mapper.Map<IEnumerable<TEntity>>(entries));
                break;
        }
        
        if (!shouldSave)
            return Result.FromSuccess();

        if (userId is null)
            _ = await CommitAsync();
        else
            _ = await CommitAsync(userId);

        return Result.FromSuccess();
    }
}