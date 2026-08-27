using CallQuality.Core.DataAccess.ADUsersDataAccess;
using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Questions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

namespace CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;

public class CallQualityDataAccess: ICallQualityDataAccess
{
    private readonly CallQualityDbContext _context;



    public CallQualityDataAccess(CallQualityDbContext context)
    {
        _context = context;
    }

    public async Task<List<ManagerHomeOverviewRow>> GetManagerHomeOverviewAsync()
    {
        const string sql = "EXEC GetManagerHomeOverview";

        return await _context.Database
            .SqlQueryRaw<ManagerHomeOverviewRow>(sql)
            .ToListAsync();
    }


    public async Task<List<string?>> GetAssessorsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Assessments
            .Where(a => a.AssessedOn >= startDate && a.AssessedOn <= endDate && a.AssessedBy != null)
            .Select(a => a.AssessedBy)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<List<string?>> GetReassessorsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Assessments
            .Where(a => a.AssessedOn >= startDate && a.AssessedOn <= endDate && a.ReassessedBy != null)
            .Select(a => a.ReassessedBy)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();

    }

    public async Task<List<AssessorReportRow>> AssessorReportMonthAssessmentAsync(
    DateTime startDate, string assessor)
    {
        const string sql = "EXEC AssessorReportMonthAssessment @SpecifiedDate, @Assessor";

        var SpecifiedDate = new SqlParameter("@SpecifiedDate", startDate);
        var Assessor = new SqlParameter("@Assessor", assessor);

        return await _context.Database
            .SqlQueryRaw<AssessorReportRow>(sql, SpecifiedDate, Assessor)
            .ToListAsync();
    }

    public async Task<List<AssessorAccuracyReportRow>> GetAccuracyReportAsync(
     DateTime fromDate,
     DateTime toDate,
     string assessedBy)
    {
        const string sql = "EXEC Rpt_AccuracyReport @FromDate, @ToDate, @AssessedBy";

        var p1 = new SqlParameter("@FromDate", fromDate);
        var p2 = new SqlParameter("@ToDate", toDate);
        var p3 = new SqlParameter("@AssessedBy", assessedBy ?? (object)DBNull.Value);

        return await _context.Database
            .SqlQueryRaw<AssessorAccuracyReportRow>(sql, p1, p2, p3)
            .ToListAsync();
    }

    public async Task<List<AssessorTracking_Report>> GetAssessorTrackingReportAsync(
        DateTime start, DateTime end)
    {
        const string sql = "EXEC Rpt_AssessorTrackingReport @StartDate, @EndDate";

        var p1 = new SqlParameter("@StartDate", start);
        var p2 = new SqlParameter("@EndDate", end);

        return await _context.Database
            .SqlQueryRaw<AssessorTracking_Report>(sql, p1, p2)
            .ToListAsync();
    }

    public async Task<List<AssessorBreakdownPercentageResult>> AssessorBreakdown_PercentageAsync(
     DateTime fromDate, DateTime toDate, string assessor)
    {
        const string sql = "EXEC AssessorBreakdown_Count @Assessor, @FromDate, @ToDate";

        var Assessor = new SqlParameter("@Assessor", assessor);
        var FromDate = new SqlParameter("@FromDate", fromDate);
        var ToDate = new SqlParameter("@ToDate", toDate);

        return await _context.Database
            .SqlQueryRaw<AssessorBreakdownPercentageResult>(sql, Assessor, FromDate, ToDate)
            .ToListAsync();
    }

    public async Task<List<Questions>> GetQuestionWithTypesAsync()
    {
        try
        {
            return await _context.Questions
                      .AsNoTracking()
                      .Include(q => q.QuestionInType)
                          .ThenInclude(qt => qt.SubGroupType)
                              .ThenInclude(st => st.AssessmentType)
                      .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "error in GetQuestionWithTypesAsync");
            return [];
        }
    }

    public async Task<List<SubGroupType>> GetSubGroupTypesWithQuestionsAsync()
    {
        try
        {
            var subGroups = await _context.SubGroupType
                .AsNoTracking()

                .Include(sgt => sgt.AssessmentType)

                .Include(sgt => sgt.QuestionInType
                    .Where(qit =>
                        qit.Active == true &&
                        qit.Question != null))
                    .ThenInclude(qit => qit.Question)

                .Where(sgt => sgt.QuestionInType.Any(qit =>
                    qit.Active == true &&
                    qit.Question != null))

                .ToListAsync();

            return subGroups;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in GetSubGroupTypesWithQuestionsAsync");
            return [];
        }
    }

    public async Task<bool> UpdateQuestionAsync(Questions request)
    {
        if (request == null || request.QuestionId <= 0)
            return false;

        var question = await _context.Questions
            .Include(q => q.QuestionInType)
            .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

        if (question is null)
            return false;

        question.QuestionValue = request.QuestionValue;
        question.DefaultFeedback = request.DefaultFeedback;

        foreach (var incomingLink in request.QuestionInType)
        {
            var existing = question.QuestionInType
                .FirstOrDefault(qit => qit.SubGroupTypeId == incomingLink.SubGroupTypeId);

            if (existing != null)
            {
                existing.Active = incomingLink.Active;
                existing.Score = incomingLink.Score;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateQuestionOrderAsync(SubGroupType request)
    {
        if (request == null || request.SubGroupTypeId <= 0)
            return false;

        var orderMap = request.QuestionInType
            .Where(q => q.QuestionId.HasValue && q.QuestionId.Value > 0)
            .ToDictionary(
                q => q.QuestionId!.Value,
                q => q.OrderNumber
            );

        if (orderMap.Count == 0)
            return false;

        var questionIds = orderMap.Keys.ToList();

        var qits = await _context.QuestionInType
            .Where(x =>
                x.SubGroupTypeId == request.SubGroupTypeId &&
                x.QuestionId.HasValue &&
                questionIds.Contains(x.QuestionId.Value))
            .ToListAsync();

        foreach (var qit in qits)
        {
            qit.OrderNumber = orderMap[qit.QuestionId!.Value];
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CreateNewQuestionAsync(Questions request)
    {
        if (request is null)
            return false;

        if (string.IsNullOrWhiteSpace(request.QuestionValue))
            return false;

        var newQuestion = new Questions
        {
            QuestionValue = request.QuestionValue,
            DefaultFeedback = request.DefaultFeedback
        };

        _context.Questions.Add(newQuestion);

        await _context.SaveChangesAsync();

        var selected = request.QuestionInType
            .Where(x => x.SubGroupTypeId > 0 && x.Active == true)
            .ToList();

        if (selected.Count == 0)
            return true;

        var subGroupIds = selected
            .Select(x => x.SubGroupTypeId)
            .Distinct()
            .ToList();

        var maxOrders = await _context.QuestionInType
            .Where(qit => subGroupIds.Contains(qit.SubGroupTypeId))
            .GroupBy(qit => qit.SubGroupTypeId)
            .Select(g => new
            {
                SubGroupTypeId = g.Key,
                MaxOrder = g.Max(x => (int?)x.OrderNumber) ?? 0
            })
            .ToDictionaryAsync(x => x.SubGroupTypeId, x => x.MaxOrder);

        var links = selected.Select(x => new QuestionInType
        {
            QuestionId = newQuestion.QuestionId,
            SubGroupTypeId = x.SubGroupTypeId,
            Active = true,
            Score = x.Score,
            OrderNumber = (maxOrders.TryGetValue(x.SubGroupTypeId, out var max) ? max : 0) + 1
        }).ToList();

        _context.QuestionInType.AddRange(links);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> CreateSubGroupWithQuestionsAsync(SubGroupType request)
    {
        if (request is null)
            return false;

        if (string.IsNullOrWhiteSpace(request.SubGroupValue))
            return false;

        if (request.AssessmentTypeId <= 0)
            return false;

        var newSubGroup = new SubGroupType
        {
            SubGroupValue = request.SubGroupValue,
            AssessmentTypeId = request.AssessmentTypeId
        };

        await _context.SubGroupType.AddAsync(newSubGroup);
        await _context.SaveChangesAsync();

        var questionLinks = request.QuestionInType
            .Where(q => q.QuestionId.HasValue && q.QuestionId.Value > 0)
            .Select(q => new QuestionInType
            {
                QuestionId = q.QuestionId,
                SubGroupTypeId = newSubGroup.SubGroupTypeId,
                Active = q.Active,
                Score = q.Score
            })
            .ToList();

        if (questionLinks.Count > 0)
        {
            await _context.QuestionInType.AddRangeAsync(questionLinks);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<List<Questions>> GetAllQuestionsAsync()
    {
        return await _context.Questions
            .AsNoTracking()
            .Select(q => new Questions
            {
                QuestionId = q.QuestionId,
                QuestionValue = q.QuestionValue
            })
            .ToListAsync();
    }
    public async Task<List<AssessmentType>> GetAllAssessmentTypesAsync()
    {
        return await _context.AssessmentTypes
            .AsNoTracking()
            .Select(a => new AssessmentType
            {
                AssessmentTypeId = a.AssessmentTypeId,
                TypeName = a.TypeName
            })
            .ToListAsync();
    }



public async Task<List<AssessmentDateRangeResult>>
    GetAssessmentsByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
{
    var endDate = DateTime.Now;

    var startDateParameter = new SqlParameter(
        "@StartDate",
        SqlDbType.DateTime)
    {
        Value = startDate
    };

    var endDateParameter = new SqlParameter(
        "@EndDate",
        SqlDbType.DateTime)
    {
        Value = endDate
    };

    return await _context.Database
        .SqlQueryRaw<AssessmentDateRangeResult>(
            """
            EXEC dbo.GetAssessmentsByDateRange
                @StartDate,
                @EndDate
            """,
            startDateParameter,
            endDateParameter)
        .ToListAsync(cancellationToken);
}
public async Task<List<AssessmentType>> GetAssessmentTypesAsync()
    {
        return await _context.AssessmentTypes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<UsersInDepartment>> GetUsersByDepartmentAsync(string department)
    {
        var sql = "EXEC sp_GetUsersByDepartment @Department";
        var departments = new SqlParameter("@Department", department);
        return await _context.Database
            .SqlQueryRaw<UsersInDepartment>(sql, departments)
            .ToListAsync();
    }
    public async Task<List<Assessment>> GetOperatorAssessmentsAsync(
        string ext,
        DateOnly start,
        DateOnly end)
    {
        var startDate = start.ToDateTime(TimeOnly.MinValue);
        var endDate = end.ToDateTime(TimeOnly.MaxValue);

        return await _context.Assessments
            .AsNoTracking()
            .Include(a => a.AssessmentDetails)
                .ThenInclude(ad => ad.Question)
            .Where(a => a.Extension == ext &&
                        a.AssessedOn >= startDate &&
                        a.AssessedOn <= endDate)
            .OrderByDescending(a => a.AssessedOn)
            .ToListAsync();
    }


    public async Task<List<OperatorAssignment>> GetOperatorAssignmentReportAsync()
    {
        try
        {  
            return await _context.OperatorAssignment
            .AsNoTracking()
            .Include(oa => oa.Assessor)
            .Include(oa => oa.AssessorIdSecondaryNavigation)
            .OrderBy(oa => oa.Assessor!.AssessorName)
            .ToListAsync();

        }catch(Exception ex)
        {
            Log.Error(ex, "error on GetOperatorAssignmentReportAsync");
            return [];
        }
 
    }

    public async Task<List<TrainingRegister>> GetOperatorQuestionsMissedReportAsync(
    DateTime startDate,
    DateTime? endDate = null)
    {
        var sql = "EXEC Rpt_OperatorQuestionsMissed @StartDate, @EndDate";

        var p1 = new SqlParameter("@StartDate", startDate);
        var p2 = new SqlParameter("@EndDate", (object?)endDate ?? DBNull.Value);

        return await _context.Database
            .SqlQueryRaw<TrainingRegister>(sql, p1, p2)
            .ToListAsync();
    }


    public async Task<List<UserAdd>> GetManagersAsync()
    {
        var sql = "EXEC GetManagersList";

        return await _context.Database
            .SqlQueryRaw<UserAdd>(sql)
            .ToListAsync();
    }


    public async Task<List<UserAdd>> GetAllUsersTraining()
    {
        var sql = "EXEC GetAllUsersTraining";

        return await _context.Database
            .SqlQueryRaw<UserAdd>(sql)
            .ToListAsync();
    }

    public async Task<List<ADUser>> GetAllUsersAsync()
    {
        try
        {
            const string sql = "EXEC dbo.GetAllADUser";

            return await _context.Database
                .SqlQueryRaw<ADUser>(sql)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetAllUsers failed");
            return new List<ADUser>();
        }
    }

    public async Task<Assessment?> GetAssessmentViewAsync(int assessmentId)
    {
        try
        {
          return await _context.Assessments
                    .AsNoTracking()
                    .Include(x => x.AssessmentDetails)
                        .ThenInclude(d => d.Question)
                            .ThenInclude(q => q.QuestionInType)
                    .Include(x => x.CallDetails)
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);
        }catch(Exception ex)
        {
            Log.Error(ex, "GetAssessmentViewAsync failed for AssessmentId: {AssessmentId}", assessmentId);
            return null;

        }
    }
    public async Task<List<Feedback>> GetFeedbackForAssessmentAsync(int assessmentId)
    {
        var assessmentIdText = assessmentId.ToString();

        return await _context.Feedback
            .AsNoTracking()
            .Where(f => f.AssessmentsIncluded == assessmentIdText)
            .ToListAsync();
    }


    public async Task<List<AgentAssignedModel>> GetAgentsAssignedToAssessorAsync(string userGuid)
    {
        var sql = "EXEC GetAgentsAssignedToAssessor @UserGuid";
        var param = new SqlParameter("@UserGuid", userGuid);

        return await _context.Database
            .SqlQueryRaw<AgentAssignedModel>(sql, param)
            .ToListAsync();
    }






    // new interaction

    public async Task<List<SubGroupType>> GetSubGroupsAndQuestionsAsync(string typeName)
    {
        return await _context.SubGroupType
            .AsNoTracking()
            .Include(sg => sg.QuestionInType)
                .ThenInclude(qit => qit.Question)
            .Include(sg => sg.AssessmentType)
            .Where(sg => sg.AssessmentType != null &&
                         sg.AssessmentType.TypeName == typeName)
            .OrderBy(sg => sg.SubGroupTypeId)
            .ToListAsync();
    }


    public async Task<int> SaveAssessmentAsync(Assessment assessment)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var feedbackItems = assessment.Feedback.ToList();

            assessment.Feedback.Clear();

            Log.Information("Saving Assessment first...");

            _context.Assessments.Add(assessment);

            await _context.SaveChangesAsync();

            Log.Information("Assessment saved. AssessmentId: {AssessmentId}", assessment.AssessmentId);

            int assessmentId = assessment.AssessmentId;

            foreach (var feedback in feedbackItems)
            {
                feedback.AssessmentsIncluded = assessmentId.ToString();

                _context.Feedback.Add(feedback);
            }

            if (feedbackItems.Count > 0)
            {
                Log.Information("Saving Feedback items. Count: {Count}", feedbackItems.Count);

                await _context.SaveChangesAsync();

                Log.Information("Feedback saved.");
            }

            await tx.CommitAsync();

            return assessmentId;
        }
        catch (DbUpdateException ex)
        {
            await tx.RollbackAsync();

            Log.Error(ex,
                "SaveAssessmentAsync FAILED. Inner: {InnerMessage}",
                ex.InnerException?.Message);

            throw;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();

            Log.Error(ex, "SaveAssessmentAsync FAILED — Transaction rolled back");

            throw;
        }
    }
    public async Task<int?> GetAssessmentTypeIdByDepartmentAsync(string? department)
    {
        if (string.IsNullOrWhiteSpace(department))
            return null;

        var normalizedDepartment = department.Trim().ToUpper();

        return await _context.AssessmentTypes
            .Where(x => x.TypeName != null)
            .Where(x => x.TypeName!.Trim().ToUpper() == normalizedDepartment)
            .Select(x => (int?)x.AssessmentTypeId)
            .FirstOrDefaultAsync();
    }

    public async Task<Assessment?> GetAssessmentForReassessmentAsync(int assessmentId)
    {
        return await _context.Assessments
            .AsNoTracking()
            .Include(a => a.AssessmentDetails)
                .ThenInclude(d => d.Question)
            .Include(a => a.CallDetails)
            .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);
    }

    public async Task<string> GetAssessorNameFromAssessment(int assessmentID)
    {
        return await _context.Assessments
            .AsNoTracking()
            .Where(x => x.AssessmentId == assessmentID)
            .Select(x => x.AssessedBy)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SaveReassessmentAsync(Assessment request)
    {
        var assessment = await _context.Assessments
            .FirstOrDefaultAsync(a => a.AssessmentId == request.AssessmentId);

        if (assessment == null)
            return false;

        assessment.IsReassessed = request.IsReassessed;
        assessment.ReassessedBy = request.ReassessedBy;
        assessment.ReassessedOn = request.ReassessedOn;

        var detailRows = await _context.AssessmentDetails
            .Where(d => d.AssessmentId == request.AssessmentId)
            .ToListAsync();

        foreach (var existingDetail in detailRows)
        {
            var updatedDetail = request.AssessmentDetails
                .FirstOrDefault(x => x.QuestionId == existingDetail.QuestionId);

            if (updatedDetail == null)
                continue;

            existingDetail.IsNa = updatedDetail.IsNa;
            existingDetail.ReassessorAnswer = updatedDetail.ReassessorAnswer;
            existingDetail.ReassessorNote = updatedDetail.ReassessorNote;
        }

        assessment.ReassessmentScore = detailRows
            .Where(d =>
                d.IsNa != true &&
                d.ReassessorAnswer == true)
            .Sum(d => d.Score ?? 0);




        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<Assessor>> GetAllAssessorsAsync()
    {
        return await _context.Assessors
            .AsNoTracking()
            .OrderBy(a => a.AssessorName)
            .ToListAsync();
    }

    public async Task<List<OperatorAssignment>> GetOperatorAssignmentsAsync(int? assessorId = null)
    {
        var query = _context.OperatorAssignment
            .AsNoTracking();

        if (assessorId.HasValue)
        {
            query = query.Where(x => x.AssessorId == assessorId.Value);
        }

        return await query.ToListAsync();
    }

    private static string BuildChangeSummary(
    int? oldAssessorId,
    int? newAssessorId,
    int? oldSecondaryAssessorId,
    int? newSecondaryAssessorId,
    DateTime? oldStart,
    DateTime? newStart,
    DateTime? oldEnd,
    DateTime? newEnd)
    {
        var changes = new List<string>();

        if (oldAssessorId != newAssessorId)
            changes.Add($"AssessorId changed from '{oldAssessorId}' to '{newAssessorId}'");

        if (oldSecondaryAssessorId != newSecondaryAssessorId)
            changes.Add($"SecondaryAssessorId changed from '{oldSecondaryAssessorId}' to '{newSecondaryAssessorId}'");

        if (oldStart != newStart)
            changes.Add($"SecondaryStartDate changed from '{oldStart:yyyy-MM-dd}' to '{newStart:yyyy-MM-dd}'");

        if (oldEnd != newEnd)
            changes.Add($"SecondaryEndDate changed from '{oldEnd:yyyy-MM-dd}' to '{newEnd:yyyy-MM-dd}'");

        return changes.Count == 0
            ? "No changes"
            : string.Join("; ", changes);
    }

    public async Task<int> SaveAssignOperatorsAsync(
    int assessorId,
    List<string> operatorIds,
    string changedBy)
    {
        if (operatorIds == null || operatorIds.Count == 0)
            return 0;

        operatorIds = operatorIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (operatorIds.Count == 0)
            return 0;

        await using var transaction = await _context.Database.BeginTransactionAsync();

  

            var existingAssignments = await _context.OperatorAssignment
                .Where(x => operatorIds.Contains(x.OperatorId))
                .ToListAsync();

            var alreadyAssignedToThisAssessor = existingAssignments
                .Where(x => x.AssessorId == assessorId)
                .Select(x => x.OperatorId)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var assignmentsToRemove = existingAssignments
                .Where(x => x.AssessorId != assessorId)
                .ToList();

            if (assignmentsToRemove is { Count: > 0 })
            {
                var deleteLogs = assignmentsToRemove
                    .Select(x => new OperatorAssignmentChangeLog
                    {
                        OperatorAssignmentRowKey = x.RowKey,
                        OperatorId = x.OperatorId,

                        ActionType = "DELETE",

                        OldAssessorId = x.AssessorId,
                        NewAssessorId = null,

                        OldAssessorIdSecondary = x.AssessorIdSecondary,
                        NewAssessorIdSecondary = null,

                        OldSecondaryStartDate = x.SecondaryStartDate,
                        NewSecondaryStartDate = null,

                        OldSecondaryEndDate = x.SecondaryEndDate,
                        NewSecondaryEndDate = null,

                        ChangedBy = changedBy,
                        ChangedOn = DateTime.UtcNow,

                        ChangeSummary =
                            $"Operator '{x.OperatorId}' removed from assessor '{x.AssessorId}'"
                    })
                    .ToList();

                _context.OperatorAssignmentChangeLog.AddRange(deleteLogs);

                _context.OperatorAssignment.RemoveRange(assignmentsToRemove);

                await _context.SaveChangesAsync();
            }

            var toInsert = operatorIds
                .Where(opId =>
                    !alreadyAssignedToThisAssessor.Contains(opId))
                .Select(opId => new OperatorAssignment
                {
                    AssessorId = assessorId,
                    OperatorId = opId,
                    AssessorIdSecondary = null,
                    SecondaryStartDate = null,
                    SecondaryEndDate = null
                })
                .ToList();

            if (toInsert.Count == 0)
            {
                await transaction.CommitAsync();
                return 0;
            }
            _context.OperatorAssignment.AddRange(toInsert);


            await _context.SaveChangesAsync();

            var insertLogs = toInsert
                .Select(x => new OperatorAssignmentChangeLog
                {
                    OperatorAssignmentRowKey = x.RowKey,
                    OperatorId = x.OperatorId,

                    ActionType = "INSERT",

                    OldAssessorId = null,
                    NewAssessorId = x.AssessorId,

                    OldAssessorIdSecondary = null,
                    NewAssessorIdSecondary = x.AssessorIdSecondary,

                    OldSecondaryStartDate = null,
                    NewSecondaryStartDate = x.SecondaryStartDate,

                    OldSecondaryEndDate = null,
                    NewSecondaryEndDate = x.SecondaryEndDate,

                    ChangedBy = changedBy,
                    ChangedOn = DateTime.UtcNow,

                    ChangeSummary =
                        $"Operator '{x.OperatorId}' assigned to assessor '{x.AssessorId}'"
                })
                .ToList();

            _context.OperatorAssignmentChangeLog.AddRange(insertLogs);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return toInsert.Count;
    }



    public async Task DeleteAssignmentAsync(int rowKey, string changedBy)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var row = await _context.OperatorAssignment
            .FirstOrDefaultAsync(x => x.RowKey == rowKey);

        if (row == null)
            return;

        var log = new OperatorAssignmentChangeLog
        {
            OperatorAssignmentRowKey = row.RowKey,
            OperatorId = row.OperatorId,

            ActionType = "DELETE",

            OldAssessorId = row.AssessorId,
            NewAssessorId = null,

            OldAssessorIdSecondary = row.AssessorIdSecondary,
            NewAssessorIdSecondary = null,

            OldSecondaryStartDate = row.SecondaryStartDate,
            NewSecondaryStartDate = null,

            OldSecondaryEndDate = row.SecondaryEndDate,
            NewSecondaryEndDate = null,

            ChangedBy = changedBy,
            ChangedOn = DateTime.UtcNow,

            ChangeSummary = $"Operator assignment deleted. Operator '{row.OperatorId}' was assigned to assessor '{row.AssessorId}'"
        };

        _context.OperatorAssignmentChangeLog.Add(log);
        _context.OperatorAssignment.Remove(row);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task UpdateSecondaryAssignmentAsync(
    int rowKey,
    int? secondaryAssessorId,
    DateTime? start,
    DateTime? end,
    string changedBy)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var row = await _context.OperatorAssignment
            .FirstOrDefaultAsync(x => x.RowKey == rowKey);

        if (row == null)
            return;

        var oldSecondaryAssessorId = row.AssessorIdSecondary;
        var oldStart = row.SecondaryStartDate;
        var oldEnd = row.SecondaryEndDate;

        var hasChanges =
            oldSecondaryAssessorId != secondaryAssessorId ||
            oldStart != start ||
            oldEnd != end;

        if (!hasChanges)
            return;

        var log = new OperatorAssignmentChangeLog
        {
            OperatorAssignmentRowKey = row.RowKey,
            OperatorId = row.OperatorId,

            ActionType = "UPDATE",

            OldAssessorId = row.AssessorId,
            NewAssessorId = row.AssessorId,

            OldAssessorIdSecondary = oldSecondaryAssessorId,
            NewAssessorIdSecondary = secondaryAssessorId,

            OldSecondaryStartDate = oldStart,
            NewSecondaryStartDate = start,

            OldSecondaryEndDate = oldEnd,
            NewSecondaryEndDate = end,

            ChangedBy = changedBy,
            ChangedOn = DateTime.UtcNow,

            ChangeSummary = BuildChangeSummary(
                row.AssessorId,
                row.AssessorId,
                oldSecondaryAssessorId,
                secondaryAssessorId,
                oldStart,
                start,
                oldEnd,
                end)
        };

        row.AssessorIdSecondary = secondaryAssessorId;
        row.SecondaryStartDate = start;
        row.SecondaryEndDate = end;

        _context.OperatorAssignmentChangeLog.Add(log);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task UpdateAssignmentAsync(
     int rowKey,
     int assessorId,
     int? secondaryAssessorId,
     DateTime? start,
     DateTime? end,
     string changedBy)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var assignment = await _context.OperatorAssignment
            .FirstOrDefaultAsync(x => x.RowKey == rowKey);

        if (assignment == null)
        {
            throw new Exception($"Operator assignment with RowKey {rowKey} was not found.");
        }

        var oldAssessorId = assignment.AssessorId;
        var oldSecondaryAssessorId = assignment.AssessorIdSecondary;
        var oldStart = assignment.SecondaryStartDate;
        var oldEnd = assignment.SecondaryEndDate;

        var hasChanges =
            oldAssessorId != assessorId ||
            oldSecondaryAssessorId != secondaryAssessorId ||
            oldStart != start ||
            oldEnd != end;

        if (!hasChanges)
            return;

        var log = new OperatorAssignmentChangeLog
        {
            OperatorAssignmentRowKey = assignment.RowKey,
            OperatorId = assignment.OperatorId,

            ActionType = "UPDATE",

            OldAssessorId = oldAssessorId,
            NewAssessorId = assessorId,

            OldAssessorIdSecondary = oldSecondaryAssessorId,
            NewAssessorIdSecondary = secondaryAssessorId,

            OldSecondaryStartDate = oldStart,
            NewSecondaryStartDate = start,

            OldSecondaryEndDate = oldEnd,
            NewSecondaryEndDate = end,

            ChangedBy = changedBy,
            ChangedOn = DateTime.UtcNow,

            ChangeSummary = BuildChangeSummary(
                oldAssessorId,
                assessorId,
                oldSecondaryAssessorId,
                secondaryAssessorId,
                oldStart,
                start,
                oldEnd,
                end)
        };

        assignment.AssessorId = assessorId;
        assignment.AssessorIdSecondary = secondaryAssessorId;
        assignment.SecondaryStartDate = start;
        assignment.SecondaryEndDate = end;

        _context.OperatorAssignmentChangeLog.Add(log);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }


    public async Task<List<Operator_NumberOfAssessment_Report>> GetOperatorNumberOfAssessmentReportsAsync(
        DateTime month,
        Guid? assessorIdGuid = null)
    {
        var startDate = new DateTime(month.Year, month.Month, 1);

        var endDate = startDate.AddMonths(1).AddDays(-1);

        var sql = @"
        EXEC Rpt_OperatorNumberOfAssessmentReport 
            @StartDate, 
            @EndDate, 
            @AssessorIdGuid";

        var startParam = new SqlParameter("@StartDate", startDate);

        var endParam = new SqlParameter("@EndDate", endDate);

        var assessorParam = new SqlParameter("@AssessorIdGuid", SqlDbType.UniqueIdentifier)
        {
            Value = assessorIdGuid.HasValue
                ? assessorIdGuid.Value
                : DBNull.Value
        };

        return await _context.Database
            .SqlQueryRaw<Operator_NumberOfAssessment_Report>(
                sql,
                startParam,
                endParam,
                assessorParam)
            .ToListAsync();
    }

    public async Task<List<QuestionWrongStat>> GetQuestionWrongStatsRangeAsync(
        DateTime startDate,
        DateTime endDate,
        string? typeName = null)
    {
        var sql = "EXEC dbo.usp_QuestionWrongStats @StartDate, @EndDate, @TypeName";

        var startParam = new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate.Date };
        var endParam = new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate.Date };

        var typeParam = new SqlParameter("@TypeName", SqlDbType.NVarChar, 200)
        {
            Value = (object?)typeName ?? DBNull.Value
        };

        return await _context.Database
            .SqlQueryRaw<QuestionWrongStat>(sql, startParam, endParam, typeParam)
            .ToListAsync();
    }



}


