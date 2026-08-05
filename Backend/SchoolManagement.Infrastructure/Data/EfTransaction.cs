using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Infrastructure.Data
{
    public class EfTransaction : ITransaction
    {

        AppDbContext _appDbContext;
        IDbContextTransaction? _transaction;

        public EfTransaction(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task BeginTransactionAsync()
        {
            return BeginAsync();
        }

        public Task CommitTransactionAsync()
        {
            return CommitAsync();
        }

        public Task RollbackTransactionAsync()
        {
            return RollbackAsync();
        }

        private async Task BeginAsync()
        {
            _transaction = await _appDbContext.Database.BeginTransactionAsync();
        }

        private async Task CommitAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to commit.");

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        private async Task RollbackAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to roll back.");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
