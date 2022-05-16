using AutoMapper;
using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer.Context;
using MikyM.Common.MongoDb.DataAccessLayer.UnitOfWork;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.MongoDb.ApplicationLayer.Services;

/// <summary>
/// Base data service
/// </summary>
/// <inheritdoc cref="IMongoDbDataServiceBase{TContext}"/>
public abstract class MongoDbDataServiceBase<TContext> : IMongoDbDataServiceBase<TContext> where TContext : MongoDbContext
{
    /// <summary>
    /// <see cref="IMapper"/> instance
    /// </summary>
    protected readonly IMapper Mapper;
    /// <summary>
    /// <see cref="IMongoDbUnitOfWork{TContext}"/> instance
    /// </summary>
    protected readonly IMongoDbUnitOfWork<TContext> UnitOfWork;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="MongoDbDataServiceBase{TContext}"/>
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/></param>
    /// <param name="uof">Instance of <see cref="IMongoDbUnitOfWork"/></param>
    protected MongoDbDataServiceBase(IMapper mapper, IMongoDbUnitOfWork<TContext> uof)
    {
        Mapper = mapper;
        UnitOfWork = uof;
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync(string auditUserId)
    {
        await UnitOfWork.CommitAsync(auditUserId);
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> CommitAsync()
    {
        await UnitOfWork.CommitAsync();
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public virtual async Task<Result> RollbackAsync()
    {
        await UnitOfWork.RollbackAsync();
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public TContext Context => UnitOfWork.Context;

    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    /// <summary>
    /// Dispose action
    /// </summary>
    /// <param name="disposing">Whether disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) UnitOfWork.Dispose();

        _disposed = true;
    }
}