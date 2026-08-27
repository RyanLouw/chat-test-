using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.Context;

public partial class PRPDbContext : DbContext
{
    public PRPDbContext()
    {
    }

    public PRPDbContext(DbContextOptions<PRPDbContext> options): base(options)
    {
    }
}
