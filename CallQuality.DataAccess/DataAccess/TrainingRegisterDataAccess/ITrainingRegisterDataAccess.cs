using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.TrainingManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.DataAccess.TrainingRegisterDataAccess;

public interface ITrainingRegisterDataAccess
{
    Task<List<ExistingRegisterItem>> GetAllTrainingRegistersAsync();
    Task<List<UserAdd.Group>> GetGroupAsync();
    Task<List<string>> GetGroupUsersAsync(string groupId);
    Task<List<TrainingUserTraining>> GetTrainingRegisterUserDataAsync();
    Task<bool> SaveNewTrainingAsync(NewTrainingRegister trainingRegister, List<UserAddVM> trainees, List<string> fileNames, string uploadedBy, string uploadedByMail);
    Task<string> UploadTrainingFile(byte[] fileContent, string fileName, string authToken);
}
