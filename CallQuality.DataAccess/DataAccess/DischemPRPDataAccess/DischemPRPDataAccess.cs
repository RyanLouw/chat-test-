using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.DischemPRPDataAccess;

public class DischemPRPDataAccess : IDischemPRPDataAccess
{
    private readonly DischemPRPDbContext _context;
    public DischemPRPDataAccess(DischemPRPDbContext context)
    {
        _context = context;
    }

    public async Task<List<InteractionResult>> GetDischemPRPFullInteractionsAsync(DateTime specifiedDate, string extension)
    {
        var sql = "EXEC GetDischemPRPFullInteraction @SpecifiedDate, @Extension";

        var dateParam = new SqlParameter("@SpecifiedDate", specifiedDate);
        var extParam = new SqlParameter("@Extension", extension ?? (object)DBNull.Value);

        return await _context.Database
            .SqlQueryRaw<InteractionResult>(sql, dateParam, extParam)
            .ToListAsync();

    }
}
