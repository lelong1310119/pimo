using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PismoWebInput.Core.Infrastructure.Common.Interfaces;
using PismoWebInput.Core.Infrastructure.Domain.Common;
using PismoWebInput.Core.Persistence.Contexts;

namespace PismoWebInput.Core.Persistence.Uow;

public class UnitOfWork : IUnitOfWork<EfContext>
{
    private bool _disposed;
    private IDbContextTransaction _transaction;

    public UnitOfWork(EfContext dataContext)
    {
        DataContext = dataContext;
    }

    public EfContext DataContext { get; private set; }

    public DbSet<T> Set<T>() where T : EntityBase
    {
        return DataContext.Set<T>();
    }

    public int SaveChanges()
    {
        return DataContext.SaveChanges();
    }

    public Task<int> SaveChangesAsync()
    {
        return DataContext.SaveChangesAsync();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return DataContext.SaveChangesAsync(cancellationToken);
    }

    public override string ToString()
    {
        return DataContext.ContextId.InstanceId.ToString();
    }

    #region Unit of Work Transactions

    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        _transaction = DataContext.Database.BeginTransaction(isolationLevel);
    }

    public void Commit()
    {
        _transaction?.Commit();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }

    #endregion

    #region Destructors

    public void Dispose()
    {
        _transaction?.Dispose();
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UnitOfWork()
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DataContext.Dispose();
                DataContext = null!;
            }

            _disposed = true;
        }
    }

    #endregion
}