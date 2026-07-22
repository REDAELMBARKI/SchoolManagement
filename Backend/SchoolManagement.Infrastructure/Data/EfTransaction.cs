using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
            _transaction = _appDbContext.Database.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to roll back.")
            _transaction.CommitAsync();
            return Task.CompletedTask;
        }

        public Task RollbackTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to roll back.")
            _transaction.RollbackAsync();
            return Task.CompletedTask;
        }
    }
}
