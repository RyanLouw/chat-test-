using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults;

namespace CallQuality.Core.Manager.ReportManager.Models;

public sealed class ManagerDashDTO
{
    public ManagerDashDTO()
    {
        Departments = new List<DepartmentViewModel>();
    }

    public ManagerDashDTO(IEnumerable<DepartmentViewModel> departments)
    {
        Departments = departments.ToList();
    }

    public List<DepartmentViewModel> Departments { get; set; }
}

public sealed class DepartmentViewModel
{
    public DepartmentViewModel()
    {
        Agents = new List<AgentViewModel>();
    }

    public DepartmentViewModel(
        string departmentName,
        IEnumerable<AgentViewModel> agents)
    {
        DepartmentName = departmentName;
        Agents = agents.ToList();
    }

    public string DepartmentName { get; set; } = string.Empty;

    public List<AgentViewModel> Agents { get; set; }
}

public sealed class AgentViewModel
{
    public AgentViewModel()
    {
    }

    public AgentViewModel(ManagerHomeOverviewRow row)
    {
        DisplayName = row.DisplayName;
        AssessmentsDone = row.AssessmnetsDone;
    }

    public string DisplayName { get; set; } = string.Empty;

    public int AssessmentsDone { get; set; }
}
