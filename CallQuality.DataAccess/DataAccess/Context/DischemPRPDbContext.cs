using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.Context;

public partial class DischemPRPDbContext : DbContext
{
    public DischemPRPDbContext()
    {
    }

    public DischemPRPDbContext(DbContextOptions<DischemPRPDbContext> options): base(options)
    {
    }
}
