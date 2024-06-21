using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel.Common;
using Simba.SharedKernel.Extensions;

namespace VO.Infrastructure.Persistence;

public class ApplicationWriteDbContext : ApplicationDbContext
{
    public ApplicationWriteDbContext(DbContextOptions options) : base(options)
    {
        SavingChanges += OnSavingChanges;
    }

    private void OnSavingChanges(object sender, SavingChangesEventArgs e)
    {
        _cleanString();
        ConfigureEntityDates();
    }

    private void _cleanString()
    {
        IEnumerable<EntityEntry> changedEntities =
            ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified);
        foreach (EntityEntry item in changedEntities)
        {
            IEnumerable<PropertyInfo> properties = item.Entity.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            foreach (PropertyInfo property in properties)
            {
                string val = (string)property.GetValue(item.Entity, null);

                if (val?.HasValue() ?? false)
                {
                    string newVal = val.Fa2En().FixPersianChars();
                    if (newVal == val)
                        continue;
                    property.SetValue(item.Entity, newVal, null);
                }
            }
        }
    }

    private void ConfigureEntityDates()
    {
        IEnumerable<ITimeModification> updatedEntities = ChangeTracker.Entries().Where(x =>
                x.Entity is ITimeModification && x.State == EntityState.Modified)
            .Select(x => x.Entity as ITimeModification);

        IEnumerable<ITimeModification> addedEntities = ChangeTracker.Entries().Where(x =>
            x.Entity is ITimeModification && x.State == EntityState.Added).Select(x => x.Entity as ITimeModification);

        foreach (ITimeModification entity in updatedEntities)
            if (entity != null)
                entity.ModifiedDate = DateTime.Now;

        foreach (ITimeModification entity in addedEntities)
            if (entity != null)
            {
                entity.CreatedTime = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
            }
    }
}