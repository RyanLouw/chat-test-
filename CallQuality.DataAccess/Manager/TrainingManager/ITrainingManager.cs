using CallQuality.Core.Manager.TrainingManager.Models;
using Microsoft.AspNetCore.Http;

namespace CallQuality.Core.Manager.TrainingManager
{
    public interface ITrainingManager
    {
        Task<TrainingVM> GetTrainingRegisterDataAsync();
        Task<TrainingDetailsPageVM> GetTrainingDetailsAsync(Guid? userId);
        Task<List<string>> GetTraineesByLeaderIdAsync(string leaderId);
        Task<List<string>> GetTraineesByGroupIdAsync(string Groupid);
        Task<bool> SaveTrainingRegisterAsync(TrainingDetailsPageVM model, IFormFileCollection files);
        Task<byte[]> ExportTraining();
    }
}
