using PismoWebInput.Core.Infrastructure.Common.Interfaces;
using PismoWebInput.Core.Persistence.Contexts;

namespace PismoWebInput.Core.Persistence.Uow;

public interface IEfUnitOfWork : IUnitOfWork<EfContext>
{
}

public class EfUnitOfWork : UnitOfWork, IEfUnitOfWork
{
    public EfUnitOfWork(EfContext dataContext)
        : base(dataContext)
    {
    }
}