using CallQuality.Core.DataAccess.ADUsersDataAccess;
using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.DataAccess.DischemPRPDataAccess;
using CallQuality.Core.DataAccess.DischemSRSDataAccess;
using CallQuality.Core.DataAccess.PRPDataAccess;
using CallQuality.Core.DataAccess.PSPDataAccess;
using CallQuality.Core.DataAccess.ThreeCXDataAccess;
using CallQuality.Core.Helpers;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.Core.Manager.QuestionsManager.Models;
using CallQuality.Core.Resources;
using Serilog;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CallQuality.Core.Manager.AssessmentsManager;

public class CallAssessmentManager : ICallAssessmentManager
{
    private readonly ICallQualityDataAccess _db;
    private readonly IDischemPRPDataAccess _dischemPRP;
    private readonly IDischemSRSDataAccess _dischemSRS;
    private readonly IPRPDataAccess _dbPRP;
    private readonly IPSPDataAccess _pspData;
   
    private readonly IThreeCXDataAccess _ThreeCXDataAccess;
    private readonly IADUsersDataAccess _adUsersDataAccess;
    private readonly IUserSession _userSession;
    private readonly IPSPAbbvieDataAccess _pabbvieDataAccess;

    private readonly EmailHelper _emailHelper;


    public CallAssessmentManager(
        ICallQualityDataAccess db,
        IDischemPRPDataAccess dischemPRP,
        IDischemSRSDataAccess dischemSRS,
        IPRPDataAccess dbPRP,
        IPSPDataAccess pSPDataAccess,
        IThreeCXDataAccess threeCXDataAccess,
        IADUsersDataAccess adUsersDataAccess, 
        IUserSession userSession,
        IPSPAbbvieDataAccess pSPAbbvieDataAccess,
        EmailHelper emailHelper


        )
    {
        _db = db;
        _dischemPRP = dischemPRP;
        _dischemSRS = dischemSRS;
        _dbPRP = dbPRP;
        _pspData = pSPDataAccess;
        _ThreeCXDataAccess = threeCXDataAccess;
        _adUsersDataAccess = adUsersDataAccess;
        _userSession = userSession;
        _pabbvieDataAccess = pSPAbbvieDataAccess;
        _emailHelper = emailHelper;
    }



    public async Task<ReAssessmentDTO> GetAssessmentByDateRangeAsync(
    DateTime startDate,
    CancellationToken cancellationToken = default)
    {
        var rows = await _db.GetAssessmentsByDateRangeAsync(startDate,cancellationToken);

        if (rows.Count == 0)
        {
            return new ReAssessmentDTO();
        }

        var assessments = rows
            .GroupBy(row => row.AssessmentId)
            .Select(group =>
            {
                var first = group.First();

                return new AssessmentDTO
                {
                    AssessmentId = first.AssessmentId,
                    AssessmentTypeId = first.AssessmentTypeId,
                    FamilyIdentifier = first.FamilyIdentifier,
                    PatientId = first.PatientId,
                    ScriptId = first.ScriptId,
                    SystemName = first.SystemName,
                    InteractionId = first.InteractionId,
                    ProfileNumber = first.ProfileNumber,
                    PharmacyGroup = first.PharmacyGroup,
                    PharmacyName = first.PharmacyName,
                    AssessmentScore = first.AssessmentScore,
                    AssessedBy = first.AssessedBy,
                    AssessedOn = first.AssessedOn,
                    IsReassessed = first.IsReassessed,
                    ReassessmentScore = first.ReassessmentScore,
                    ReassessedBy = first.ReassessedBy,
                    ReassessedOn = first.ReassessedOn,
                    Extension = first.Extension,
                    OperatorId = first.OperatorId,

                    AssessmentType = new AssessmentTypeDTO
                    {
                        AssessmentTypeId =
                            first.AssessmentTypeId ?? 0,

                        TypeName =
                            first.AssessmentTypeName ?? string.Empty,

                        ShowInFrontend =
                            first.ShowInFrontend ?? false
                    },

                    AssessmentDetails = group
                        .Where(row => row.QuestionId.HasValue)
                        .Select(row => new AssessmentDetailDTO
                        {
                            RowKey = row.RowKey ?? 0,
                            AssessmentId = row.AssessmentId,
                            QuestionId = row.QuestionId,
                            AssessorAnswer = row.AssessorAnswer,
                            ReassessorAnswer = row.ReassessorAnswer,
                            Score = row.Score,
                            ReassessorNote = row.ReassessorNote,
                            IsNa = row.IsNa ?? false,

                            Question = new QuestionDTO
                            {
                                QuestionId = row.QuestionId ?? 0,
                                QuestionValue =
                                    row.QuestionValue ?? string.Empty,
                                DefaultFeedback =
                                    row.DefaultFeedback ?? string.Empty
                            }
                        })
                        .ToList()
                };
            })
            .ToList();

        return new ReAssessmentDTO
        {
            AlreadyReassessed = assessments
                .Where(assessment =>
                    assessment.IsReassessed == true)
                .OrderByDescending(assessment =>
                    assessment.AssessedOn)
                .ThenByDescending(assessment =>
                    assessment.AssessmentId)
                .ToList(),

            NotReassessed = assessments
                .Where(assessment =>
                    assessment.IsReassessed != true)
                .OrderByDescending(assessment =>
                    assessment.AssessedOn)
                .ThenByDescending(assessment =>
                    assessment.AssessmentId)
                .ToList()
        };
    }

    public async Task<List<SubGroupTypeWithQuestionsDTO>> GetSubGroupsWithQuestionsByIdsAsync(List<int> selectedSubGroupIds)
    {
        if (selectedSubGroupIds is null or { Count: 0 })
        {
            return [];
        }

        var distinctIds = selectedSubGroupIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var subGroups =
            await _db.GetSubGroupTypesWithQuestionsAsync();

        return subGroups
            .Where(subGroup =>
                distinctIds.Contains(subGroup.SubGroupTypeId))
            .Select(subGroup =>
                new SubGroupTypeWithQuestionsDTO(subGroup))
            .ToList();
    }

    public async Task<PagedAssessmentVM> GetPagedAssessmentsAsync(
        DateTime startDate,
        int possiblePage,
        int reassessedPage,
        int pageSize,
        string? search,
        string? activeTab)
    {
        possiblePage = Math.Max(1, possiblePage);
        reassessedPage = Math.Max(1, reassessedPage);
        pageSize = Math.Clamp(pageSize, 1, 100);

        search = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        activeTab = string.Equals(
            activeTab,
            "reassessed",
            StringComparison.OrdinalIgnoreCase)
                ? "reassessed"
                : "possible";

        var assessments =
            await GetAssessmentByDateRangeAsync(startDate);
       
        IEnumerable <AssessmentDTO> possibleQuery =
            assessments.NotReassessed;

        IEnumerable<AssessmentDTO> reassessedQuery =
            assessments.AlreadyReassessed;

        if (!string.IsNullOrWhiteSpace(search))
        {
            possibleQuery = possibleQuery.Where(a =>
                MatchesSearch(a, search));

            reassessedQuery = reassessedQuery.Where(a =>
                MatchesSearch(a, search));
        }

        int possibleTotalCount = possibleQuery.Count();
        int reassessedTotalCount = reassessedQuery.Count();

        int possibleTotalPages = (int)Math.Ceiling(
            (double)possibleTotalCount / pageSize);

        int reassessedTotalPages = (int)Math.Ceiling(
            (double)reassessedTotalCount / pageSize);

        possiblePage = possibleTotalPages > 0
            ? Math.Min(possiblePage, possibleTotalPages)
            : 1;

        reassessedPage = reassessedTotalPages > 0
            ? Math.Min(reassessedPage, reassessedTotalPages)
            : 1;
        var pagedPossibleAssessments = possibleQuery
            .OrderByDescending(a => a.AssessmentId)
            .Skip((possiblePage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var pagedReassessedAssessments = reassessedQuery
            .OrderByDescending(a => a.AssessmentId)
            .Skip((reassessedPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedAssessmentVM
        {
            NotReassessed = pagedPossibleAssessments,
            Reassessment = pagedReassessedAssessments,

            PossibleCurrentPage = possiblePage,
            ReassessedCurrentPage = reassessedPage,

            PageSize = pageSize,

            TotalCountNotReassessment = possibleTotalCount,
            TotalCountReassessment = reassessedTotalCount,

            SearchTerm = search,
            ActiveTab = activeTab
        };
    }

    private static bool MatchesSearch(
        AssessmentDTO assessment,
        string search)
    {
        return ContainsIgnoreCase(assessment.PharmacyName, search)
               || ContainsIgnoreCase(assessment.AssessedBy, search)
               || ContainsIgnoreCase(
                   assessment.AssessmentType?.TypeName,
                   search)
               || ContainsIgnoreCase(
                   assessment.PharmacyGroup,
                   search)
               || ContainsIgnoreCase(
                   assessment.Extension,
                   search)
               || ContainsIgnoreCase(
                   assessment.FamilyIdentifier,
                   search);
    }


    private static bool ContainsIgnoreCase(
        string? value,
        string search)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase);
    }


    public async Task<OperatorAssessmentsVM> GetOperatorAssessmentsAsync(
        string? ext,
        string? department,
        DateOnly? start,
        DateOnly? end)
    {

        var assessmentTypes = await _db.GetAssessmentTypesAsync();

        if (string.IsNullOrWhiteSpace(department))
        {
            department = assessmentTypes.FirstOrDefault()?.TypeName ?? "Pharmacy";
        }

        var users = await _db.GetUsersByDepartmentAsync(department);

        List<OperatorAssessmentVM> assessments = new();

        if (!string.IsNullOrWhiteSpace(ext) && start.HasValue && end.HasValue)
        {
            var assessmentEntities = await _db.GetOperatorAssessmentsAsync(ext,start.Value,end.Value);

            assessments = assessmentEntities
                .SelectMany(a => a.AssessmentDetails
                    .Select(d => new OperatorAssessmentVM(a, d)))
                .OrderByDescending(x => x.AssessedOn)
                .ToList();
        }

        var model = new OperatorAssessmentsVM
        {

            AssessmentTypes = assessmentTypes
                .Select(x => new AssessmentTypeVM(x))
                .ToList(),


            UsersInDepartments = users
                .Select(x => new UsersInDepartmentVM(x))
                .ToList(),


            OperatorAssessments = assessments
        };

        return model;
    }




    public async Task<NewAssessmentVM> NewAssessment(
        AgentAssignedVM? agent,
        bool? ManulalAssessment)
    {
        var userId = _userSession.GetCurrentUserId();
        var agents = await GetAgentsAssignedToAssessorAsync(userId.ToString());

        var isManualAssessment = ManulalAssessment ?? false;

        List<InteractionResultVM> interactions = new();
        List<InteractionResultVM> random = new();
        List<PSPInteractionsVM> pspInteractions = new();
        List<PSPInteractionsVM> randomPsp = new();

        List<CallInteractionVM> combinedCalls = new();

        if (agent != null)
        {
            if (!isManualAssessment)
            {
                if (agent.Department == "PSP")
                {
                    pspInteractions = await HandlePSP(agent);

                    if (pspInteractions.Any())
                    {
                        randomPsp = pspInteractions
                            .OrderBy(_ => Guid.NewGuid())
                            .Take(5)
                            .ToList();
                    }
                }
                else
                {
                    interactions = await GetInteractions(agent) ?? new List<InteractionResultVM>();

                    if (interactions.Any())
                    {
                        random = interactions
                            .OrderBy(_ => Guid.NewGuid())
                            .Take(5)
                            .ToList();
                    }
                }
            }
            else
            {

                var day1Date = DateTime.Today.AddDays(-1);
                var day2Date = DateTime.Today;

                var day1 = await _ThreeCXDataAccess.LookupByExtensionAsync(agent.Extension, day1Date);
                var day2 = await _ThreeCXDataAccess.LookupByExtensionAsync(agent.Extension, day2Date);

                combinedCalls.AddRange(day1.Select(x => new CallInteractionVM(x)));
                combinedCalls.AddRange(day2.Select(x => new CallInteractionVM(x)));

            }
        }

        var model = new NewAssessmentVM
        {
            Agents = agents,
            Interactions = interactions,
            RandomInteractions = random,
            PSPInteractions = pspInteractions,
            RandomPSPInteractions = randomPsp,

            CallInteractions = combinedCalls,
            IsManulaAssessment = isManualAssessment,
            SelectedAgentId = null
        };

        return model;
    }



    public async Task<List<AgentAssignedVM>> GetAgentsAssignedToAssessorAsync(string userGuid)
    {
        var agents = await _db.GetAgentsAssignedToAssessorAsync(userGuid);

        return agents
            .Select(x => new AgentAssignedVM(x))
            .ToList();
    }


    public async Task<List<InteractionResultVM>> GetInteractions(AgentAssignedVM agent)
    {
        switch (agent.Department)
        {
            case "Dischem PRP":
                return await HandleDischemPRP(agent);

            case "Dischem SRS":
                return await HandleDischemSRS(agent);

            case "PRP":
                return await HandlePRP(agent);

            default:
                return new List<InteractionResultVM>();
        }
    }


    private async Task<List<InteractionResultVM>> HandleDischemPRP(AgentAssignedVM agent)
    {
        var date = DateTime.Now.AddHours(-20);
        var dt = (await _dischemPRP.GetDischemPRPFullInteractionsAsync(date, agent.Extension)).Select(x => new InteractionResultVM(x)).ToList();
        return dt;

    }

    private async Task<List<InteractionResultVM>> HandleDischemSRS(AgentAssignedVM agent)
    {
        var date = DateTime.Now.AddHours(-20);
        var dt = (await _dischemSRS.GetDischemSRSFullInteractionsAsync(date, agent.Extension)).Select(x => new InteractionResultVM(x)).ToList();
        return dt;
    }


    private async Task<List<InteractionResultVM>> HandlePRP(AgentAssignedVM agent)
    {
        var date = DateTime.Now.AddHours(-20);
        var dt = (await _dbPRP.GetPRPFullInteractionsAsync(date, agent.Extension)).Select(x => new InteractionResultVM(x)).ToList();

        return dt;
    }


    private async Task<List<PSPInteractionsVM>> HandlePSP(AgentAssignedVM agent)
    {
        var date = DateTime.Now.AddHours(-20);
        var endDate = DateTime.Now;

        var normalPspTask = _pspData.GetPSPInteractionsAsync(
            date,
            endDate,
            agent.Extension);


        var abbViePspTask = _pabbvieDataAccess.GetPSPInteractionsAsync(
            date,
            endDate,
            agent.Extension);

       

        await Task.WhenAll(normalPspTask, abbViePspTask);

        var interactions = await normalPspTask;
        var abbVieInteractions = await abbViePspTask;

        return interactions
            .Concat(abbVieInteractions)
            .Select(interaction => new PSPInteractionsVM(interaction))
            .ToList();
    }

    public async Task<AssessInteractionVM> BuildAssessInteractionAsync(
     InteractionResultVM? normal,
     PSPInteractionsVM? psp)
    {
        bool isPsp = psp != null;
        string extension = isPsp ? psp.Extension : normal?.Extension;
        string agentName = isPsp ? psp.AgentName : normal?.AgentName;
        string assessmentType = isPsp ? "PSP" : "PRP";
        string numberRaw = isPsp ? psp.CellNumber : normal?.CellNumber;

        var user = await _adUsersDataAccess.GetAdUserByExtensionAsync(extension);

        var subGroups = (await _db.GetSubGroupsAndQuestionsAsync(user.Department)).Select(x => new SubGroupVM(x)).ToList();


        var callList = await GetCallInfo(extension, numberRaw);


        var model = new AssessInteractionVM
        {
            IsPsp = isPsp,
            PspInteraction = psp,
            Interaction = normal,

            AgentExtension = extension,
            AgentName = agentName,

            CallInteraction = callList,
            SubGroup = subGroups,

            Score = 0,
            MaxScore = 0,
            AutoFeedback = string.Empty,
            AdditionalFeedback = string.Empty
        };

        return model;
    }

    public async Task<AssessInteractionVM> BuildcallInteractionAsync(
     CallInteractionVM normal)
    {
        if (normal != null)
        {
            bool isPsp = false;
            string extension = normal.Extension;
            string agentName = normal.CallerFullName;
         
            string numberRaw = normal.CallerNumber;
            var prpSubGroups = (await _db.GetSubGroupsAndQuestionsAsync("PRP")).Select(x => new SubGroupVM(x)).ToList();
            var pspSubGroups = (await _db.GetSubGroupsAndQuestionsAsync("PSP")).Select(x => new SubGroupVM(x)).ToList();

            var subGroups = prpSubGroups
                .Concat(pspSubGroups)
                .ToList();

            var callList = await GetCallInfo(extension, numberRaw);
            var normalInteraction = MapCallToInteractionResult(normal, agentName);

            var model = new AssessInteractionVM
            {
                IsPsp = isPsp,
                PspInteraction = null,
                Interaction = normalInteraction,

                AgentExtension = extension,
                AgentName = agentName,

                CallInteraction = callList,
                SubGroup = subGroups,

                Score = 0,
                MaxScore = 0,
                AutoFeedback = string.Empty,
                AdditionalFeedback = string.Empty
            };

            return model;
        }else
        {
            throw new InvalidOperationException("CallInteraction was null.");
        }


    }

    private static InteractionResultVM MapCallToInteractionResult(CallInteractionVM call, string? agentDisplayName = null)
    {
        if (call == null) throw new ArgumentNullException(nameof(call));

        return new InteractionResultVM
        {
            ContactID = null,
            FamilyIdentifier = null,
            OrderID = null,

            Extension = call.Extension ?? string.Empty,

            PharmacyGroup = null,
            PharmacyName = string.Empty,
            Profile = null,
            AgentName = agentDisplayName ?? call.CallerFullName ?? string.Empty,

            CellNumber = call.CallerNumber,
            HomeNumber = null,
            WorkNumber = null
        };
    }



    public string ScoreFeedback(AssessInteractionVM model)
    {
        if (model.SelectedQuestions == null || model.SelectedQuestions.Count == 0)
            return "";

        var feedbackBlocks = new List<string>();

        foreach (var q in model.SelectedQuestions)
        {
            if (model.Answers.TryGetValue(q.QuestionId, out var answer))
            {
                if (answer == "no")
                {
                    var question = System.Net.WebUtility.HtmlEncode(q.QuestionValue);
                    var feedback = !string.IsNullOrWhiteSpace(q.DefaultFeedback)
                        ? System.Net.WebUtility.HtmlEncode(q.DefaultFeedback)
                        : "Needs improvement.";

                    feedbackBlocks.Add($@"
                    <div style='margin-bottom:8px;'>
                        <strong>{question}</strong><br/>
                        {feedback}
                    </div>");
                }
            }
        }

        if (feedbackBlocks.Count == 0)
            return "<i>Great work! No issues recorded.</i>";

        return string.Join("", feedbackBlocks);
    }

    public async Task<List<CallInteractionVM>> GetCallInfo(string extension, string numberRaw)
    {
        var numbers = SanitizeNumbers(numberRaw)
            .Select(NormalizeNumber)
            .ToList();

        var today = DateTime.Now;
        var yesterday = today.AddDays(-1);



        var callsYesterday = await _ThreeCXDataAccess.LookupByExtensionAsync(extension, yesterday);
        var callsToday = await _ThreeCXDataAccess.LookupByExtensionAsync(extension, today);
        var allCalls = callsYesterday.Concat(callsToday).ToList();

        var matchedCalls = new List<CallInteractionVM>();

        foreach (var call in allCalls)
        {
            var callNumber = NormalizeNumber(call.CallerNumber ?? "");

            if (!numbers.Any() || numbers.Contains(callNumber))
            {
                matchedCalls.Add(new CallInteractionVM(call));
            }
        }

        return matchedCalls;
    }

    public async Task<string> GetDownloadUrlAsync(string recordingID)
    {
        var result = await _ThreeCXDataAccess.GetDownloadUrlAsync(recordingID);
        return result;
    }

    private List<string> SanitizeNumbers(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();

        return input
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();
    }
    private string NormalizeNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return "";

        return new string(number.Where(char.IsDigit).ToArray());
    }

    public async Task<int> SaveAssessmentAsync(AssessInteractionDTO model, ClaimsPrincipal user)
    {

        var userName = _userSession.GetUserName();

        var adUser = await _adUsersDataAccess.GetAdUserByExtensionAsync(model.AgentExtension);

        var users = await _db.GetAllUsersAsync();

        var agent = users.FirstOrDefault(u =>
            string.Equals(u.Extension, model.AgentExtension, StringComparison.OrdinalIgnoreCase));

        var teamlead = agent == null
            ? null
            : users.FirstOrDefault(u => u.ID == agent.Manager_ID);

        var department = agent?.Department?.Trim();

        int? assessmentTypeId = await _db.GetAssessmentTypeIdByDepartmentAsync(department);

        string combinedFeedback = EmailHelper.BuildCombinedFeedbackHtml(
            model.AutoFeedback,
            model.AdditionalFeedback
        );

        var assessment = new Assessment
        {
            ContactId = model.Interaction?.ContactID,
            OrderId = model.Interaction?.OrderID,
            ProfileNumber = model.Interaction?.Profile,
            Extension = model.AgentExtension,
            FamilyIdentifier = model.Interaction?.FamilyIdentifier,
            PharmacyName = model.Interaction?.PharmacyName,
            PharmacyGroup = model.Interaction?.PharmacyGroup,
            AssessedOn = DateTime.Now,
            IsManualAssessment = true,
            AssessmentScore = model.Score,
            AssessedBy = userName,
            OperatorId = adUser?.ID ?? agent?.ID,
            CorrectlyIdentified = null,
            AssessmentTypeId = assessmentTypeId
        };

        if (model.SelectedCall != null)
        {
            long parsedRecId = long.TryParse(
                model.SelectedCall.RecordingID,
                out var rid)
                    ? rid
                    : 0;

            var duration = ParseRecordingDuration(
                model.SelectedCall.DurationMinutes);

            assessment.CallDetails.Add(new CallDetail
            {
                NumberAssessedOn = model.SelectedCall.CallerNumber,

                RecordingLength = duration.FormattedDuration,
                RecordingMinutes = duration.Minutes,
                RecordingSeconds = duration.Seconds,

                RecordingUrl = model.SelectedCall.RecordingURL,
                RecordingId = parsedRecId,
                CallDate = model.SelectedCall.CallDateTime,
                DidAssessmentOn = true
            });
        }

        foreach (var q in model.SelectedQuestions)
        {
            model.Answers.TryGetValue(q.QuestionId, out string? answer);

            answer = answer?.Trim().ToLower();

            bool isNA = answer == "n/a" || answer == "na";

            bool? assessorAnswer = isNA
                ? null
                : answer == "yes";

            assessment.AssessmentDetails.Add(new AssessmentDetail
            {
                QuestionId = q.QuestionId,
                AssessorAnswer = assessorAnswer,
                IsNa = isNA,
                Score = q.Score
            });
        }

        assessment.Feedback.Add(new Feedback
        {
            FeedbackText = combinedFeedback,
            FeedbackSendOn = DateTime.Now,
            FeedbackSendBy = userName,
            FeedbackSendTo = model.AgentExtension
        });

        var assessmentId = await _db.SaveAssessmentAsync(assessment);
        TrySendAssessmentEmail( model, assessmentId, combinedFeedback, agent,teamlead);

        return assessmentId;
    }

    private void TrySendAssessmentEmail(
    AssessInteractionDTO model,
    int assessmentId,
    string combinedFeedback,
    ADUser? agent,
    ADUser? teamlead)
    {
        try
        {


            double score = (((double)model.Percentage / 100) * model.MaxScore).Truncate2();

            var emailRows = new List<EmailDisplayAssessmentRow>
        {
            new()
            {
                AssessedOn = DateTime.Now,
                score = score.ToString(),
                percentage = model.Percentage.ToString()
            }
        };

            var emailModel = new EmailDisplayAssessment
            {
                feedback = combinedFeedback,
                AssesmentId = assessmentId.ToString(),
                Percentage = model.Percentage.ToString(),
                rows = emailRows,
                Agent = agent,
                TeamLeader = teamlead
            };

            _emailHelper.SendEmail(emailModel);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Assessment {AssessmentId} saved, but email sending failed.",
                assessmentId);
        }
    }

    private sealed record RecordingDurationResult(
    string? FormattedDuration,
    int? Minutes,
    int? Seconds);
 
    private static RecordingDurationResult ParseRecordingDuration(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new RecordingDurationResult(
                null,
                null,
                null);
        }

        var cleanedValue = value
            .Trim()
            .Replace(":", " ");

        var minMatch = Regex.Match(
            cleanedValue,
            @"(?<minutes>\d+(?:\.\d+)?)\s*min",
            RegexOptions.IgnoreCase);

        var secMatch = Regex.Match(
            cleanedValue,
            @"(?<seconds>\d+)\s*sec",
            RegexOptions.IgnoreCase);

        decimal? minuteValue = null;
        var secondValue = 0;

        if (minMatch.Success &&
            decimal.TryParse(
                minMatch.Groups["minutes"].Value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var parsedMinutes))
        {
            minuteValue = parsedMinutes;
        }

        if (secMatch.Success &&
            int.TryParse(
                secMatch.Groups["seconds"].Value,
                out var parsedSeconds))
        {
            secondValue = parsedSeconds;
        }

        int? minutes = null;
        int? seconds = null;

        if (minuteValue.HasValue)
        {
            if (minuteValue.Value % 1 != 0)
            {
                var totalSeconds =
                    (int)Math.Round(minuteValue.Value * 60);

                minutes = totalSeconds / 60;
                seconds = totalSeconds % 60;
            }
            else
            {
                minutes = (int)minuteValue.Value;
                seconds = secondValue;
            }
        }
        else if (secMatch.Success)
        {
            minutes = 0;
            seconds = secondValue;
        }

        var formattedDuration =
            minutes.HasValue && seconds.HasValue
                ? $"{minutes.Value} min {seconds.Value} sec"
                : value.Trim();

        return new RecordingDurationResult(
            formattedDuration,
            minutes,
            seconds);
    }

    public async Task<ReassessmentDTO> GetReassessAsync(int assessmentId)
    {
        var assessment = await _db.GetAssessmentForReassessmentAsync(assessmentId);

        if (assessment == null)
            return new ReassessmentDTO();

        var email = await _adUsersDataAccess.GetUserEmailByExtensionAsync(
            assessment.Extension ?? string.Empty
        );

        return new ReassessmentDTO(assessment, email);
    }

    public async Task<ADUser?> GetAssessorEmailFromAssessment(
        int assessmentId)
    {   // assessor email
        var assessorName =
            await _db.GetAssessorNameFromAssessment(assessmentId);

        if (string.IsNullOrWhiteSpace(assessorName))
        {
            return null;
        }

        return await _adUsersDataAccess.GetUserByName(assessorName);
    }

    public async Task<bool> SaveReassessmentAsync(
      ReassessmentSaveRequest request)
    {
        var users = await GetFacilitatorDropdown();

        var currentUserEmail = _userSession.GetUserName();

        if (string.IsNullOrWhiteSpace(currentUserEmail))
        {
            return false;
        }


        var assessment = new Assessment
        {
            AssessmentId = request.AssessmentId,
            IsReassessed = true,
            ReassessedBy = currentUserEmail,
            ReassessedOn = DateTime.Now,

            AssessmentDetails = request.Answers.Select(answer =>
            {
                var questionId = answer.Key;
                var ans = answer.Value?.Trim().ToLowerInvariant();

                return new AssessmentDetail
                {
                    QuestionId = questionId,
                    IsNa = ans == "na",

                    ReassessorAnswer = ans == "na"
                        ? null
                        : ans == "yes",

                    ReassessorNote =
                        request.Notes.GetValueOrDefault(questionId)
                };
            }).ToList()
        };

        var saved = await _db.SaveReassessmentAsync(assessment);

        if (!saved)
        {
            return false;
        }

        var reassessment =
            await GetReassessAsync(request.AssessmentId);

        var assessorUser =
            await GetAssessorEmailFromAssessment(request.AssessmentId);

        if (reassessment is null)
        {
            Log.Error(
                "Could not send reassessment feedback: reassessment result was null. AssessmentID: {AssessmentID}",
                request.AssessmentId);

            return true;
        }

        if (assessorUser is null)
        {
            Log.Error(
                "Could not send reassessment feedback: assessor user was not found. AssessmentID: {AssessmentID}",
                request.AssessmentId);

            return true;
        }

        _emailHelper.SendReassessmentFeedbackEmail(
            reassessment,
            assessorUser);

        return true;
    }







    public async Task<List<UserAddVM>> GetFacilitatorDropdown()
    {
        var allUsers = await _db.GetAllUsersTraining();

        var facilitators = allUsers
       .Select(x => new UserAddVM(x))
       .OrderBy(x => x.DisplayName)
       .ToList();

        return facilitators;
    }


}
