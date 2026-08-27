using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.PRPDataAccess;


public class PRPDataAccess : IPRPDataAccess
{
    private readonly PRPDbContext _context;
    public PRPDataAccess(PRPDbContext context)
    {
        _context = context;
    }
    public async Task<List<InteractionResult>> GetPRPFullInteractionsAsync(DateTime specifiedDate, string extension)
    {
        var sql = "EXEC GetPRPFullInteraction @SpecifiedDate, @Extension";

        var dateParam = new SqlParameter("@SpecifiedDate", specifiedDate);
        var extParam = new SqlParameter("@Extension", extension ?? (object)DBNull.Value);

        return await _context.Database
            .SqlQueryRaw<InteractionResult>(sql, dateParam, extParam)
            .ToListAsync();
    }
}
