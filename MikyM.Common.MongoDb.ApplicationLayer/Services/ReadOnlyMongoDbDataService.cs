using AutoMapper;
using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer;
using MikyM.Common.MongoDb.DataAccessLayer.Repositories;
using MikyM.Common.MongoDb.DataAccessLayer.UnitOfWork;
using MikyM.Common.Utilities.Results;
using MikyM.Common.Utilities.Results.Errors;

namespace MikyM.Common.MongoDb.ApplicationLayer.Services;

/// <summary>
/// Read-only data service
/// </summary>
/// <inheritdoc cref="IReadOnlyMongoDbDataService{TEntity}"/>
public class ReadOnlyMongoDbDataService<TEntity> : MongoDbDataServiceBase, IReadOnlyMongoDbDataService<TEntity>
    where TEntity : SnowflakeMongoDbEntity
{
    /// <summary>
    /// Creates a new instance of <see cref="IReadOnlyMongoDbDataService{TEntity}"/>
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/></param>
    /// <param name="uof">Instance of <see cref="IMongoDbUnitOfWork"/></param>
    public ReadOnlyMongoDbDataService(IMapper mapper, IMongoDbUnitOfWork uof) : base(mapper, uof)
    {
    }

    /// <summary>
    /// Gets the base MongoDbRepository for this data service
    /// </summary>
    protected virtual IRepositoryBase BaseMongoDbRepository => UnitOfWork.GetRepository<ReadOnlyMongoDbRepository<TEntity>>();
    /// <summary>
    /// Gets the read-only version of the <see cref="BaseMongoDbRepository"/> (essentially casts it for you)
    /// </summary>
    protected ReadOnlyMongoDbRepository<TEntity> ReadOnlyMongoDbMongoDbRepository =>
        (ReadOnlyMongoDbRepository<TEntity>)BaseMongoDbRepository;

    /// <inheritdoc />
    public virtual async Task<Result<TGetResult>> GetAsync<TGetResult>(string id, bool shouldProject = false) where TGetResult : class
    {
        var res = await GetAsync(id);
        return !res.IsDefined() ? Result<TGetResult>.FromError(new NotFoundError()) : Result<TGetResult>.FromSuccess(this.Mapper.Map<TGetResult>(res.Entity));
    }

    /// <inheritdoc />
    public virtual async Task<Result<TEntity>> GetAsync(string id)
    {
        var res = await ReadOnlyMongoDbMongoDbRepository.GetAsync(id);
        return res is null ? Result<TEntity>.FromError(new NotFoundError()) : Result<TEntity>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TGetResult>>> GetAllAsync<TGetResult>(bool shouldProject = false) where TGetResult : class
    {
        IReadOnlyList<TGetResult> res;
        if (shouldProject) res = await ReadOnlyMongoDbMongoDbRepository.GetAllAsync<TGetResult>();
        else res = Mapper.Map<IReadOnlyList<TGetResult>>(await ReadOnlyMongoDbMongoDbRepository.GetAllAsync());
        return Result<IReadOnlyList<TGetResult>>.FromSuccess(res);
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync()
    {
        var res = await ReadOnlyMongoDbMongoDbRepository.GetAllAsync();
        return Result<IReadOnlyList<TEntity>>.FromSuccess(res);
    }
}