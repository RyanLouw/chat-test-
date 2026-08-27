using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

public sealed class AssessorReportRow
{
    public DateTime Date { get; set; }
    public int Day { get; set; }
    public int DischemPRP_TimeListened { get; set; }
    public int DischemSRS_TimeListened { get; set; }
    public int PRP_TimeListened { get; set; }
    public int PSP_TimeListened { get; set; }
    public int AE_TimeListened { get; set; }
}
