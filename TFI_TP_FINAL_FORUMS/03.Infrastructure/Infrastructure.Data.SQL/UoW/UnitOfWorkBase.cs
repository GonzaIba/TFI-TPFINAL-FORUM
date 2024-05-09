using Core.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.UoW
{
    public class UnitOfWorkBase : IUnitOfWorkBase
    {
        public readonly ApplicationDbContext _appDbcontext;
        public readonly ApplicationGatewayDbContext _gatewayDbContext;

        public UnitOfWorkBase(ApplicationDbContext appDContext, ApplicationGatewayDbContext gatewayDbContext)
        {
            _appDbcontext = appDContext;
            _gatewayDbContext = gatewayDbContext;
        }

        public DbContext Context => _appDbcontext;


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _appDbcontext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            await _appDbcontext.Database.CommitTransactionAsync();
            await Task.CompletedTask;
        }
        public async Task RollbackTransactionAsync()
        {
            await _appDbcontext.Database.RollbackTransactionAsync();
            await Task.CompletedTask;
        }


        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _appDbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                return _appDbcontext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _appDbcontext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<bool> Complete()
        {
            return await _appDbcontext.SaveChangesAsync() > 0;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _appDbcontext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
