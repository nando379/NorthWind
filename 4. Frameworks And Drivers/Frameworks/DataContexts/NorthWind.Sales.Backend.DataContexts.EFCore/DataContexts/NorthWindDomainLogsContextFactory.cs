namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;

internal class NorthWindDomainLogsContextFactory :
    IDesignTimeDbContextFactory<NorthWindDomainLogsContext>
{
    public NorthWindDomainLogsContext CreateDbContext(string[] args)
    {
        IOptions<DBOptions> DbOptions =
            Microsoft.Extensions.Options.Options.Create(
                new DBOptions
                {
                    DomainLogsConnectionString =
                        Environment.GetEnvironmentVariable("DBOptions__DomainLogsConnectionString") ?? throw new InvalidOperationException("Set DBOptions__DomainLogsConnectionString before running EF tooling.")
                });
        return new NorthWindDomainLogsContext(DbOptions);
    }
}
