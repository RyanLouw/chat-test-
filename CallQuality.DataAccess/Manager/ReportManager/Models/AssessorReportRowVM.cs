using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.Manager.ReportManager.Models;

public sealed class AssessorReportRowVM
{
    public AssessorReportRowVM()
    {
    }

    public AssessorReportRowVM(AssessorReportRow row)
    {
        Date = row.Date;
        Day = row.Day;
        DischemPRP_TimeListened = row.DischemPRP_TimeListened;
        DischemSRS_TimeListened = row.DischemSRS_TimeListened;
        PRP_TimeListened = row.PRP_TimeListened;
        PSP_TimeListened = row.PSP_TimeListened;
        AE_TimeListened = row.AE_TimeListened;
    }

    public DateTime Date { get; set; }
    public int Day { get; set; }
    public int DischemPRP_TimeListened { get; set; }
    public int DischemSRS_TimeListened { get; set; }
    public int PRP_TimeListened { get; set; }
    public int PSP_TimeListened { get; set; }
    public int AE_TimeListened { get; set; }
}
