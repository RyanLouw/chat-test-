using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using CallQuality.Core.Manager.AssessmentsManager.Models;


namespace CallQuality.Core.DataAccess.PRPDataAccess;

public interface IPRPDataAccess
{
    Task<List<InteractionResult>> GetPRPFullInteractionsAsync(DateTime specifiedDate, string extension);

}
