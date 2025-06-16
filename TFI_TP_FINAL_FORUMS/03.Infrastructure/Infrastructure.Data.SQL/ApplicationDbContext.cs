using Core.Domain.IdentityModels;
using Core.Domain.Models;
using Core.Domain.Models.GenericEntityClass;
using Core.Domain.Views;
using CrossCutting.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly)
                .SetPropertyDefaultSqlValue("CreateDate", "getdate()")
                .SetPropertyDefaultValue<bool>("Active", true)
                .SetPropertyQueryFilter("Active", true)
                .ConfigureGenericProperties(typeof(GenericEntity));

            modelBuilder.Ignore<Users>();
            modelBuilder.Ignore<UsersClaims>();
            modelBuilder.Ignore<UsersLogin>();
            modelBuilder.Ignore<UsersRoles>();
            modelBuilder.Ignore<UsersForumModel>();
            modelBuilder.Ignore<UsersToken>();
            modelBuilder.Ignore<Roles>();
            modelBuilder.Ignore<RolesClaim>();
            modelBuilder.Ignore<RefreshToken>();

            modelBuilder.Entity<TopThreeUsersLastWeekView>()
                .HasNoKey()
                .ToView("vw_topThreeUsersLastWeek");

            modelBuilder.Entity<TopTenPublicationsLastWeekView>()
                .HasNoKey()
                .ToView("vw_topTenPublicationsLastWeek");

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetUpdateDateOnModifiedEntries();
            CheckDeleteRoleBase();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUpdateDateOnModifiedEntries();
            CheckDeleteRoleBase();
            return base.SaveChangesAsync(cancellationToken);
        }


        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null!;

            _currentTransaction = await Database.BeginTransactionAsync();

            return _currentTransaction;
        }

        public async Task CommitAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null!;
                }
            }
        }

        private void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null!;
                }
            }
        }


        #region Entries
        private void SetUpdateDateOnModifiedEntries()
        {
            var modifiedEntries = ChangeTracker
                .Entries()
                .Where(e => e.Metadata.FindProperty("UpdateDate") != null &&
                            e.State == EntityState.Modified);

            foreach (var modifiedEntry in modifiedEntries)
            {
                modifiedEntry.Property("UpdateDate").CurrentValue = DateTime.Now;
            }
        }

        private void CheckDeleteRoleBase()
        {
            try
            {
                var modifiedEntries = ChangeTracker
                                    .Entries()
                                    .Where(e => e.Metadata.FindProperty("NormalizedName") != null &&
                                                e.Entity is Roles &&
                                                (e.State == EntityState.Deleted ||
                                                e.State == EntityState.Modified)
                                                );

                foreach (var modifiedEntry in modifiedEntries)
                {
                    var role = modifiedEntry.Property("NormalizedName")?.CurrentValue?.ToString() ?? null;
                    if (role == "ADMINISTRADOR" || role == "USER")
                    {
                        throw new Exception($"No se puede eliminar/editar el rol {role} porque es un rol base del sistema.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
