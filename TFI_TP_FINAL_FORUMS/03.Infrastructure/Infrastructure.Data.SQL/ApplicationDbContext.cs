using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using CrossCutting.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly)
                .SetPropertyDefaultSqlValue("CreateDate", "getdate()")
                .SetPropertyDefaultValue<bool>("Active", true)
                .SetPropertyQueryFilter("Active", true)
                .ConfigureGenericProperties(typeof(GenericEntity));

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseMySQL("server=localhost;database=library;user=testUser;password=1234");
        //}

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
                throw ex;
            }
        }
    }
}
