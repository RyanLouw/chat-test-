using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess;
using CallQuality.Core.Helpers;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.ExportManager;
using CallQuality.Core.Manager.TrainingManager.Models;
using CallQuality.DataAccess.DataAccess.TrainingRegisterDataAccess;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Data;



namespace CallQuality.Core.Manager.TrainingManager;

public class TrainingManager : ITrainingManager
{
    private readonly ITrainingRegisterDataAccess _tc;
    private readonly ICallQualityDataAccess _db;
    private readonly IUserSession _ua;
    private readonly IExportService _ex;



    public TrainingManager(ITrainingRegisterDataAccess tc, ICallQualityDataAccess db, IUserSession ua, IExportService ex)
    {
        _tc = tc;
        _db = db;
        _ua = ua;
        _ex = ex;
    }

    public async Task<TrainingVM> GetTrainingRegisterDataAsync()
    {
        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1);

        var userEntities = await _tc.GetTrainingRegisterUserDataAsync();

        var users = userEntities
            .Select(x => new TrainingUserTrainingVM(x))
            .OrderByDescending(x => x.TrainingDate)
            .ToList();

        var existingRegisterEntities = await _tc.GetAllTrainingRegistersAsync();

        var existingRegisters = existingRegisterEntities
            .Select(x => new ExistingRegisterItemVM(x))
            .OrderByDescending(x => x.CreatedOn)
            .ToList();

        var operatorEntities = await _db.GetOperatorAssignmentReportAsync();

        var operators = operatorEntities
            .Select(x => new OperatorAssignmentReportVM(x))
            .ToList();

        var suggestedTrainingEntities = await _db.GetOperatorQuestionsMissedReportAsync(
            startDate,
            endDate);

        var suggestedTrainings = suggestedTrainingEntities
              .GroupBy(x => new
              {
                  x.ID_GUID,
                  x.AgentName,
                  x.Extension,
                  x.Department,
                  x.AverageScore
              })
              .Select(group =>
              {
                  var vm = new TrainingRegisterSuggestedGroupedVM(group.First());

                  vm.MissedQuestions = group
                      .Where(x => !string.IsNullOrWhiteSpace(x.QuestionValue))
                      .Select(x => new MissedQuestion(x))
                      .ToList();

                  return vm;
              })
              .ToList();

        return new TrainingVM
        {
            UserTrainings = users,
            ExistingRegisters = existingRegisters,
            OperatorAssignments = operators,
            SuggestedTrainings = suggestedTrainings
        };
    }


    public async Task<TrainingDetailsPageVM> GetTrainingDetailsAsync(Guid? userId)
    {
        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var managers = await GetManagersAsync();
        var facilitators = await GetFacilitatorDropdown();
        var trainees = await GetFacilitatorDropdown();
        var durations = GetTimeDurationOptions();
        var groups = await GetGroupsDropdown();

        var newTrainingRegisterView = new NewTrainingRegisterVM
        {
            DueDate = DateTime.Today
        };

        var loggedInUserId = _ua.GetCurrentUserId();

        var loggedInUserIdString = loggedInUserId?.ToString();

        var loggedInFacilitator = facilitators.FirstOrDefault(f =>
            string.Equals(
                f.UserId?.Trim(),
                loggedInUserIdString?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (loggedInFacilitator != null)
        {
            newTrainingRegisterView.TrainingFacilitator = loggedInFacilitator;
        }

        if (userId != null)
        {
            var suggestedTrainingEntities = await _db.GetOperatorQuestionsMissedReportAsync(
                startDate,
                endDate);

            var suggestedTrainings = suggestedTrainingEntities
                .GroupBy(x => new
                {
                    x.ID_GUID,
                    x.AgentName,
                    x.Extension,
                    x.Department,
                    x.AverageScore
                })
                .Select(group =>
                {
                    var vm = new TrainingRegisterSuggestedGroupedVM(group.First());

                    vm.MissedQuestions = group
                        .Where(x => !string.IsNullOrWhiteSpace(x.QuestionValue))
                        .Select(x => new MissedQuestion(x))
                        .ToList();

                    return vm;
                })
                .ToList();

            var userSuggested = suggestedTrainings
                .FirstOrDefault(x => x.ID_GUID == userId.Value);

            if (userSuggested != null)
            {
                var description = string.Join(Environment.NewLine,
                    userSuggested.MissedQuestions.Select(q =>
                        $"• {q.QuestionValue} (Missed {q.MissedCount} time{(q.MissedCount == 1 ? "" : "s")})"));

                newTrainingRegisterView.TrainingDescription = description;

                newTrainingRegisterView.SelectedTraineeIds = new List<string>
        {
            userSuggested.ID_GUID.ToString()
        };
            }

            newTrainingRegisterView.TrainingTopic = "Call Quality Counselling";

            var firstDuration = durations.FirstOrDefault();

            if (firstDuration != null)
            {
                newTrainingRegisterView.TimeDuration = firstDuration.DurationValue;
            }
        }

        return new TrainingDetailsPageVM
        {
            Leaders = managers,
            Facilitators = facilitators,
            Trainees = trainees,
            Groups = groups,
            TimeDurations = durations,
            NewTraining = newTrainingRegisterView
        };
    }

    public async Task<byte[]> ExportTraining()
    {
        var modle = await GetTrainingRegisterDataAsync();
        return await _ex.DownloadTrainingReportExcelAsync(modle);

    }

    public async Task<List<UserAddVM>> GetManagersAsync()
    {
        var managerEntities = await _db.GetManagersAsync();

        var managers = managerEntities
            .Select(x => new UserAddVM(x))
            .OrderBy(x => x.DisplayName)
            .ToList();

        managers.Insert(0, new UserAddVM
        {
            UserId = "",
            DisplayName = "-- Please select a manager --"
        });

        return managers;
    }


    public async Task<List<AssessmentsManager.Models.UserAddVM>> GetFacilitatorDropdown()
    {
        var allUsers = (await _db.GetAllUsersTraining()).Select(x => new AssessmentsManager.Models.UserAddVM(x));

        var facilitators = allUsers
            .OrderBy(u => u.DisplayName)
            .ToList();

        return facilitators;
    }



    public List<DurationOption> GetTimeDurationOptions()
    {
        var durations = new List<DurationOption>();

        for (int minutes = 30; minutes <= 240; minutes += 30)
        {
            string text;

            if (minutes % 60 == 0)
            {
                text = $"{minutes / 60}-Hr";
            }
            else
            {
                text = $"{minutes / 60}-Hr : {minutes % 60}-Min";
            }

            durations.Add(new DurationOption
            {
                DurationValue = minutes,
                DurationText = text
            });
        }

        return durations;
    }


    public async Task<List<UserAddVM.Group>> GetGroupsDropdown()
    {
        var groupEntities = await _tc.GetGroupAsync();

        return groupEntities
            .Select(x => new UserAddVM.Group(x))
            .OrderBy(g => g.DisplayName)
            .ToList();
    }




    public async Task<List<string>> GetTraineesByLeaderIdAsync(string leaderId)
    {
        if (string.IsNullOrWhiteSpace(leaderId))
            return new List<string>();

        var users = await _db.GetAllUsersAsync();

        return users
            .Where(u => !string.IsNullOrWhiteSpace(u.Manager_ID))
            .Where(u => string.Equals(
                u.Manager_ID.Trim(),
                leaderId.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .Select(u => u.ID_Guid.ToString())
            .Where(adUserId => !string.IsNullOrWhiteSpace(adUserId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<List<string>> GetTraineesByGroupIdAsync(string Groupid)
    {
        var users = await _tc.GetGroupUsersAsync(Groupid);
        return users;
    }


    public async Task<bool> SaveTrainingRegisterAsync(TrainingDetailsPageVM model, IFormFileCollection? files)
    {
        List<string> uploadedFileUrls = new List<string>();
        if (files != null)
        {  uploadedFileUrls = await UploadFilesAsync(files); }
        var userid = _ua.GetCurrentUserId();
        var users = await GetFacilitatorDropdown();
        var username = _ua.GetUserName();
        var email = _ua.GetCurrentUserEmail();
        //var currentUser = users.FirstOrDefault(u =>
        //    u.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase)
        //);
        var trainees = model.NewTraining.SelectedTraineeIds;
        var selectedUsers = users
            .Where(u => trainees.Contains(u.UserId))
            .ToList();
        var dto = MapToDTO(model, users);
        var result = await _tc.SaveNewTrainingAsync(
            dto,
            selectedUsers,
            uploadedFileUrls,
            uploadedBy: username,
            uploadedByMail: email
        );
        return result;
    }

    private NewTrainingRegister MapToDTO(
    TrainingDetailsPageVM model,
    List<UserAddVM> users)
    {
        var facilitatorId = model.NewTraining.TrainingFacilitator.UserId;
        var facilitator = users.FirstOrDefault(u => u.UserId == facilitatorId);
        if (facilitator == null)
            throw new Exception("Facilitator not found in list.");

        return new NewTrainingRegister
        {
            Name = model.NewTraining.TrainingTopic,
            Description = model.NewTraining.TrainingDescription,
            TrainingDate = DateTime.Now,
            TrainingDueDate = model.NewTraining.DueDate,
            AddNewAssessment = false,
            selectedTimeDuration = model.NewTraining.TimeDuration.ToString(),
            trainingFacilitator = facilitator,
            trainingFacilitatorMail = facilitator.EmailAddress,
            FacilitatorSigned = "Signed",
            SystemID = string.IsNullOrEmpty(model.NewTraining.SystemId) ? null : Guid.Parse(model.NewTraining.SystemId),
            IsCallQuality = true
        };
    }



    private async Task<List<string>> UploadFilesAsync(IFormFileCollection files)
    {
        var uploadedFileUrls = new List<string>();

        if (files == null || files.Count == 0)
            return uploadedFileUrls;


        var user = _ua.GetUserName();
        var authToken = _ua.GetCurrentUserAuthToken();

        if (string.IsNullOrEmpty(authToken))
        {
            Log.Error("OID not found — cannot upload files.");
            return uploadedFileUrls;
        }

        foreach (var file in files)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            byte[] fileBytes = ms.ToArray();

            string fileUrl = await _tc.UploadTrainingFile(fileBytes, file.FileName, authToken);

            if (!string.IsNullOrEmpty(fileUrl))
            {
                uploadedFileUrls.Add(fileUrl);
            }
        }

        return uploadedFileUrls;
    }


}