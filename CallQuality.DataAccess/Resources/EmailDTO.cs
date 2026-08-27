using CallQuality.Core.DataAccess.Context.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.Resources;

public class EmailDisplayAssessment
{
    public string feedback { get; set; }

    public string AssesmentId { get; set; }

    public string Percentage { get; set; }
    public List<EmailDisplayAssessmentRow> rows { get; set; }
    public ADUser Agent { get; set; }
    public ADUser TeamLeader { get; set; }
}

public class EmailDisplayAssessmentRow
{
    public DateTime AssessedOn { get; set; }
    public string score { get; set; }
    public string percentage { get; set; }
}
public class EmailSettings
{
    public string Host { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromEmailPassword { get; set; } = string.Empty;

    public string FromEmailName { get; set; } = string.Empty;

    public string DemoEmails { get; set; } = string.Empty;

    public string ViewOperatorAssessmentURL { get; set; } = string.Empty;
}