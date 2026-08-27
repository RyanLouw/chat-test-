using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;

namespace CallQuality.Core.DataAccess.DischemSRSDataAccess
{
    public interface IDischemSRSDataAccess
    {
        Task<List<InteractionResult>> GetDischemSRSFullInteractionsAsync(DateTime specifiedDate, string extension);
    }
}