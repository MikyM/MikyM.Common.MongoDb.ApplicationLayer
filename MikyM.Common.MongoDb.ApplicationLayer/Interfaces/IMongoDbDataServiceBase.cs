using MikyM.Common.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.DataAccessLayer.Context;

namespace MikyM.Common.MongoDb.ApplicationLayer.Interfaces;

/// <summary>
/// Base data service for MongoDb
/// </summary>
/// <typeparam name="TContext">Mongo DB context type</typeparam>
public interface IMongoDbDataServiceBase<TContext> : IDataServiceBase<TContext> where TContext : MongoDbContext
{
}
