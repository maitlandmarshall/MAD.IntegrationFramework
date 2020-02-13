namespace MAD.IntegrationFramework.Database
{
    internal class MIFDbContextFactory<DbContext> : IMIFDbContextFactory<DbContext> where DbContext : MIFDbContext, new()
    {
        private AutomaticMigrationService automaticMigrationService;

        public MIFDbContextFactory(AutomaticMigrationService automaticMigrationService)
        {
            this.automaticMigrationService = automaticMigrationService;
        }

        public DbContext Create()
        {
            DbContext dbContext = new DbContext();

            this.automaticMigrationService.EnsureDatabaseUpToDate(dbContext);

            return dbContext;
        }
    }
}
