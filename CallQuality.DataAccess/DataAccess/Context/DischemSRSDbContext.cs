using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.Context;

public partial class DischemSRSDbContext : DbContext
{
    public DischemSRSDbContext()
    {
    }

    public DischemSRSDbContext(DbContextOptions<DischemSRSDbContext> options): base(options)
    {
    }
}
