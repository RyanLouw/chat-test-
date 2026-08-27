using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;

namespace CallQuality.Core.DataAccess.DischemPRPDataAccess
{
    public interface IDischemPRPDataAccess
    {
        Task<List<InteractionResult>> GetDischemPRPFullInteractionsAsync(DateTime specifiedDate, string extension);
    }
}