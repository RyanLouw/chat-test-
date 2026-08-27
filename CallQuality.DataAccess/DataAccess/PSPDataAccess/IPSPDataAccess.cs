using CallQuality.Core.DataAccess.PSPDataAccess.Models;


namespace CallQuality.Core.DataAccess.PSPDataAccess;

public interface IPSPDataAccess
{

    Task<List<PSPInteractionsDTO>> GetPSPInteractionsAsync(DateTime startDate, DateTime endDate, string extension);
}
