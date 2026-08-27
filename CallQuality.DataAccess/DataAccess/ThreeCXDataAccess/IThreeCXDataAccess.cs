using CallQuality.Core.DataAccess.ThreeCXDataAccess.Models;

namespace CallQuality.Core.DataAccess.ThreeCXDataAccess
{
    public interface IThreeCXDataAccess
    {
        Task<List<CallInteraction>> LookupByExtensionAsync(string extension, DateTime date);
        Task<string?> GetDownloadUrlAsync(string recordingId);
    }
}
