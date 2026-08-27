using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

public class AgentAssignedModel
{
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
