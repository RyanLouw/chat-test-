namespace CallQuality.Core.Manager.AssessmentsManager.Models;

using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.Context.Entities;

public sealed class OperatorAssessmentsVM
{

    public List<AssessmentTypeVM> AssessmentTypes { get; set; } = new();


    public List<UsersInDepartmentVM> UsersInDepartments { get; set; } = new();


    public List<OperatorAssessmentVM> OperatorAssessments { get; set; } = new();
}

public class AssessmentTypeVM
{
    public AssessmentTypeVM()
    {
    }

    public AssessmentTypeVM(AssessmentType entity)
    {
        AssessmentTypeId = entity.AssessmentTypeId;
        TypeName = entity.TypeName ?? string.Empty;
    }

    public int AssessmentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
}

public class UsersInDepartmentVM
{

    public UsersInDepartmentVM()
    {
    }

    public UsersInDepartmentVM(UsersInDepartment entity)
    {
        ADUserId = entity.ADUserId;
        GivenName = entity.GivenName ?? string.Empty;
        Surname = entity.Surname ?? string.Empty;
        Department = entity.Department ?? string.Empty;
        EmployeeId = entity.EmployeeId ?? string.Empty;
        Extension = entity.Extension ?? string.Empty;
        AccountEnabled = entity.AccountEnabled;
    }

    public long ADUserId { get; set; }
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; }
}

public class OperatorAssessmentVM
{
    
    public OperatorAssessmentVM()
    {
    }

    public OperatorAssessmentVM(Assessment assessment, AssessmentDetail detail)
    {

        AssessmentID = assessment.AssessmentId;
        Extension = assessment.Extension ?? string.Empty;
        AssessedBy = assessment.AssessedBy ?? string.Empty;
        AssessedOn = assessment.AssessedOn;
        Assessment_Score = assessment.AssessmentScore;


        RowKey = detail.RowKey;
        QuestionValue = detail.Question?.QuestionValue ?? string.Empty;
        Score = detail.Score;

        AssessorAnswer = detail.AssessorAnswer;
        ReassessorAnswer = detail.ReassessorAnswer;
        ReassessorNote = detail.ReassessorNote ?? string.Empty;
        IsNA = detail.IsNa;
    }

    public int AssessmentID { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string AssessedBy { get; set; } = string.Empty;
    public DateTime? AssessedOn { get; set; }
    public int? Assessment_Score { get; set; }

    public int? RowKey { get; set; }
    public string QuestionValue { get; set; } = string.Empty;
    public int? Score { get; set; }

    public bool? AssessorAnswer { get; set; }
    public bool? ReassessorAnswer { get; set; }
    public string ReassessorNote { get; set; } = string.Empty;
    public bool? IsNA { get; set; }
}