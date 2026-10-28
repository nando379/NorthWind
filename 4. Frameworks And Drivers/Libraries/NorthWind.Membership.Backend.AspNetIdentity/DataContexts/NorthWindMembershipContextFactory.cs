namespace NorthWind.Membership.Backend.AspNetIdentity.DataContexts;

internal class NorthWindMembershipContextFactory :
    IDesignTimeDbContextFactory<NorthWindMembershipContext>
{
    public NorthWindMembershipContext CreateDbContext(string[] args)
    {
        IOptions<MembershipDBOptions> DbOptions =
            Microsoft.Extensions.Options.Options.Create(
                new MembershipDBOptions()
                {
                    ConnectionString =
                        Environment.GetEnvironmentVariable("MembershipDBOptions__ConnectionString") ?? throw new InvalidOperationException("Set MembershipDBOptions__ConnectionString before running EF tooling.")
                });
        return new NorthWindMembershipContext(DbOptions);
    }
}
