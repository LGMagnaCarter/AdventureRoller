﻿using Microsoft.EntityFrameworkCore;

namespace AdventureRoller.DatabaseContext
{
    public partial class AdventurerollerdbContext : DbContext
    {
        public void RejectChanges()
        {
            RejectScalarChanges();
        }

        private void RejectScalarChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}
