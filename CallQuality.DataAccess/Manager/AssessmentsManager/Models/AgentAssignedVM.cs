
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public class AgentAssignedVM
{
    public AgentAssignedVM()
    {
    }

    public AgentAssignedVM(AgentAssignedModel model)
    {
        ID_Guid = model.ID_Guid;
        DisplayName = model.DisplayName;
        Department = model.Department;
        JobTitle = model.JobTitle;
        Extension = model.Extension;
        Manager_ID = model.Manager_ID;
        MonthlyAssessmentCount = model.MonthlyAssessmentCount;
    }

    public Guid ID_Guid { get; set; }
    public string DisplayName { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public string Extension { get; set; }
    public string Manager_ID { get; set; }
    public int MonthlyAssessmentCount { get; set; }

    public string StatusColor
    {
        get
        {
            if (MonthlyAssessmentCount >= 8)
                return "Green";

            if (MonthlyAssessmentCount >= 4)
                return "Yellow";

            return "Red";
        }
    }
}