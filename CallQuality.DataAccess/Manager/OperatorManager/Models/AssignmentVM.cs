
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.DataAccess.CallQualityDataAccess.Models;


namespace CallQuality.Core.Manager.OperatorAssignmentManager.Models;

public sealed class AssignmentVM
{

    public List<ADUserDTO> AllUsers { get; set; } = new();
    public List<CqManagerVM> Managers { get; set; } = new();
    public List<CqUserUnderManagerVM>? UsersUnderManager { get; set; } = null;
    public List<AssessorsDTO> Assessors { get; set; } = new();
    public List<OperatorAssignmentsDTO> OperatorAssignments { get; set; } = new();

    public List<ADUserDTO> AvailableOperators { get; set; } = new();

    public string? SelectedManagerId { get; set; }
    public int? SelectedAssessorId { get; set; }
    public List<string> SelectedOperatorIds { get; set; } = new();
}
