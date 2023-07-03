using System.Data;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    DbSet<T> Set<T>() where T : EntityBase;
    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    void Commit();
    void Rollback();
}

public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
{
    TContext DataContext { get; }
}