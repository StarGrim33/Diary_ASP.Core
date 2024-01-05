using FonTech.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FonTech.DAL.Interceptors
{
    public class DataInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var dbContext = eventData.Context;

            if (dbContext == null)
                return base.SavingChanges(eventData, result);

            var entries = dbContext.ChangeTracker.Entries<IAuditable>();

            foreach(var entry in entries)
            {
                if(entry.State == Microsoft.EntityFrameworkCore.EntityState.Added) 
                {
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                }

                if(entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                {
                    entry.Property(x => x.LastModifiedAt).CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SavingChanges(eventData, result);
        }
    }
}
