using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.DischemSRSDataAccess;

public class DischemSRSDataAccess: IDischemSRSDataAccess
{

    private readonly DischemSRSDbContext _context;
    public DischemSRSDataAccess(DischemSRSDbContext context)
    {
        _context = context;
    }

    public async Task<List<InteractionResult>> GetDischemSRSFullInteractionsAsync(DateTime specifiedDate, string extension)
    {

        var sql = "EXEC GetDischemSRSFullInteraction @SpecifiedDate, @Extension";

        var dateParam = new SqlParameter("@SpecifiedDate", specifiedDate);
        var extParam = new SqlParameter("@Extension", extension ?? (object)DBNull.Value);

        return await _context.Database
            .SqlQueryRaw<InteractionResult>(sql, dateParam, extParam)
            .ToListAsync();
    }



}
